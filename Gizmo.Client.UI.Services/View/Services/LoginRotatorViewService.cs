using System.Runtime.InteropServices;
using Gizmo.Client.Options;
using Gizmo.Client.UI.View.States;
using Gizmo.UI.View.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Gizmo.Client.UI.View.Services
{
    [Register()]
    [Route(ClientRoutes.LoginRoute)]
    public sealed class LoginRotatorViewService : ViewStateServiceBase<LoginRotatorViewState>
    {
        #region CONSTRUCTOR
        public LoginRotatorViewService(LoginRotatorViewState viewState,
            IGizmoClient gizmoClient,
            ILogger<LoginRotatorViewService> logger,
            IServiceProvider serviceProvider,
            IOptionsMonitor<LoginRotatorOptions> loginRotatorOptions,
            IOptionsMonitor<ClientNetworkOptions> networkOptions) : base(viewState, logger, serviceProvider)
        {
            _gizmoClient = gizmoClient;
            _loginRotatorOptions = loginRotatorOptions;
            _networkOptions = networkOptions;
        }
        #endregion

        #region STATIC FIELDS

        private const int ResyncEveryMs = 30_000;
        private const double SeekToleranceSeconds = 0.5;
        private const double MinimumTickSeconds = 0.05;
        private const int ClockTimeoutMs = 3_000;

        private static readonly string[] IMAGE_EXTENSIONS = new string[]
        {
            ".JPG",
            ".PNG",
            ".BMP"
        };

        private static readonly string[] VIDEO_EXTENSIONS = new string[]
        {
            ".M4V",
            ".MP4",
            ".WMV",
            ".AVI",
            ".WEBM",
            ".MKV"
        };

        private static readonly string[] ALL_EXTENSIONS = IMAGE_EXTENSIONS.Union(VIDEO_EXTENSIONS).ToArray();

        #endregion

        #region FIELDS
        private readonly IGizmoClient _gizmoClient;
        private readonly IOptionsMonitor<LoginRotatorOptions> _loginRotatorOptions;
        private readonly IOptionsMonitor<ClientNetworkOptions> _networkOptions;
        private readonly object _itemsLock = new object();
        private List<RotatorSlot> _slots = new();
        private double _cycleSeconds;
        private TimeSpan _serverClockOffset;
        private Timer? _rotatateTimer;
        private int _timerGeneration;
        private int _timelineVersion;
        private bool _disposed;
        #endregion

        #region FUNCTIONS

        private double GetSlideSeconds()
        {
            // A missing interval used to fall back to 6 seconds. Pictures use that. A video uses it only when its duration cannot be read.
            if (_loginRotatorOptions.CurrentValue.RotateEvery <= 0)
                return 6;

            return _loginRotatorOptions.CurrentValue.RotateEvery;
        }

        private List<RotatorSlot> BuildSlots(string rotateFolder)
        {
            var slots = new List<RotatorSlot>();

            if (!Directory.Exists(rotateFolder))
            {
                Logger.LogError("Login rotator directory {directory} not found.", rotateFolder);
                return slots;
            }

            var slideSeconds = GetSlideSeconds();
            var startSeconds = 0d;

            foreach (var relativePath in GetRelativePaths(rotateFolder)
                         .OrderBy(path => path, StringComparer.OrdinalIgnoreCase)
                         .ThenBy(path => path, StringComparer.Ordinal))
            {
                var isVideo = VIDEO_EXTENSIONS.Any(extension => relativePath.EndsWith(extension, StringComparison.OrdinalIgnoreCase));
                var durationSeconds = slideSeconds;

                if (isVideo)
                {
                    var fullPath = Path.Combine(rotateFolder, relativePath);
                    if (LoginRotatorDuration.TryGetSeconds(fullPath, out var mediaSeconds) && mediaSeconds > 0)
                        durationSeconds = mediaSeconds;
                    else
                        Logger.LogError("Login rotator duration unreadable for {file}, using the slide interval.", relativePath);
                }

                slots.Add(new RotatorSlot(new LoginRotatorItemViewState()
                {
                    MediaPath = Path.Combine("https://", "rotator", relativePath).Replace('\\', '/'),
                    IsVideo = isVideo
                }, durationSeconds, startSeconds));

                startSeconds += durationSeconds;
            }

            return slots;

            static IEnumerable<string> GetRelativePaths(string root)
            {
                int rootLength = root.Length + (root[^1] == '\\' ? 0 : 1);

                foreach (string path in Directory.GetFiles(root, "*.*", SearchOption.AllDirectories)
                                       .Where(fileName => ALL_EXTENSIONS.Any(extension => fileName.EndsWith(extension, StringComparison.InvariantCultureIgnoreCase))))
                {
                    yield return path.Remove(0, rootLength);
                }
            }
        }

        private async Task<TimeSpan> SyncServerClockAsync(CancellationToken cancellationToken)
        {
            var serverUri = _networkOptions.CurrentValue.ServerUri;
            if (string.IsNullOrWhiteSpace(serverUri) || !Uri.TryCreate(serverUri, UriKind.Absolute, out var uri))
            {
                Logger.LogWarning("Login rotator clock sync skipped, server uri is not set.");
                return TimeSpan.Zero;
            }

            var offset = TimeSpan.Zero;
            var gotDate = false;

            try
            {
                using var timeout = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
                timeout.CancelAfter(ClockTimeoutMs);

                using var handler = new HttpClientHandler
                {
                    ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
                };
                using var client = new HttpClient(handler)
                {
                    Timeout = TimeSpan.FromMilliseconds(ClockTimeoutMs)
                };
                using var request = new HttpRequestMessage(HttpMethod.Get, uri);

                var started = DateTime.UtcNow;
                using var response = await client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, timeout.Token).ConfigureAwait(false);
                var finished = DateTime.UtcNow;

                if (response.Headers.Date is not DateTimeOffset serverDate)
                {
                    Logger.LogWarning("Login rotator clock sync got no Date header from {serverUri}.", serverUri);
                    return TimeSpan.Zero;
                }

                // Date is when the server sent the response. Half of the round trip places that moment on our clock.
                // Keep the value even if disposing the unread body throws.
                var localAtServer = started + TimeSpan.FromTicks((finished - started).Ticks / 2);
                offset = serverDate.UtcDateTime - localAtServer;
                gotDate = true;
                Logger.LogInformation("Login rotator server clock offset is {offsetMs} ms.", (int)offset.TotalMilliseconds);
            }
            catch (OperationCanceledException) when (!cancellationToken.IsCancellationRequested)
            {
                if (gotDate)
                    return offset;

                Logger.LogWarning("Login rotator clock sync timed out for {serverUri}.", serverUri);
                return TimeSpan.Zero;
            }
            catch (Exception ex) when (ex is not OperationCanceledException)
            {
                if (gotDate)
                    return offset;

                Logger.LogWarning(ex, "Login rotator clock sync failed for {serverUri}.", serverUri);
                return TimeSpan.Zero;
            }

            return gotDate ? offset : TimeSpan.Zero;
        }

        private bool TryLocate(out RotatorSlot slot, out double intoSeconds, out double remainingSeconds)
        {
            slot = default;
            intoSeconds = 0;
            remainingSeconds = 0;

            if (_slots.Count == 0 || _cycleSeconds <= 0)
                return false;

            var cycleTicks = (long)Math.Round(_cycleSeconds * TimeSpan.TicksPerSecond);
            if (cycleTicks <= 0)
                return false;

            var positionTicks = (DateTime.UtcNow + _serverClockOffset).Ticks % cycleTicks;
            if (positionTicks < 0)
                positionTicks += cycleTicks;

            var positionSeconds = positionTicks / (double)TimeSpan.TicksPerSecond;

            for (var i = 0; i < _slots.Count; i++)
            {
                var candidate = _slots[i];
                var end = candidate.StartSeconds + candidate.DurationSeconds;
                if (positionSeconds < end || i == _slots.Count - 1)
                {
                    slot = candidate;
                    intoSeconds = positionSeconds - candidate.StartSeconds;
                    if (intoSeconds < 0)
                        intoSeconds = 0;
                    else if (intoSeconds > candidate.DurationSeconds)
                        intoSeconds = candidate.DurationSeconds;

                    remainingSeconds = end - positionSeconds;
                    if (remainingSeconds < MinimumTickSeconds)
                        remainingSeconds = MinimumTickSeconds;

                    return true;
                }
            }

            return false;
        }

        private void ShowCurrent(TimerSignal? signal = null)
        {
            var changed = false;

            lock (_itemsLock)
            {
                if (_disposed)
                    return;

                if (signal != null && (signal.Generation != _timerGeneration || signal.Timeline != _timelineVersion))
                    return;

                if (!TryLocate(out var slot, out var intoSeconds, out var remainingSeconds))
                {
                    StopTimer();
                    return;
                }

                var item = slot.Item;
                var position = item.IsVideo ? intoSeconds : 0;
                var sameSlide = ReferenceEquals(ViewState.CurrentItem, item);

                // The clock owns the index. A repeated pass over the same picture does nothing.
                // A video is seeked again only when the playhead we published is off by more than the tolerance;
                // the player itself ignores a seek that it is already close to.
                if (!sameSlide)
                {
                    item.PositionSeconds = position;
                    ViewState.CurrentItem = item;
                    ViewState.IsEnabled = _loginRotatorOptions.CurrentValue.Enabled;
                    changed = true;
                }
                else if (item.IsVideo && Math.Abs(item.PositionSeconds - position) >= SeekToleranceSeconds)
                {
                    item.PositionSeconds = position;
                    changed = true;
                }

                ArmTimer(remainingSeconds);
            }

            if (changed)
                DebounceViewStateChanged();
        }

        private void StopTimer()
        {
            Interlocked.Increment(ref _timerGeneration);
            _rotatateTimer?.Dispose();
            _rotatateTimer = null;
        }

        private void ArmTimer(double remainingSeconds)
        {
            StopTimer();

            if (_disposed)
                return;

            var delayMs = (int)Math.Min(ResyncEveryMs, Math.Max(MinimumTickSeconds * 1000, Math.Round(remainingSeconds * 1000)));
            _rotatateTimer = new Timer(OnTimer, new TimerSignal(_timerGeneration, _timelineVersion), delayMs, Timeout.Infinite);
        }

        private void OnTimer(object? state)
        {
            try
            {
                ShowCurrent(state as TimerSignal);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Failed to adjust rotator item based on the server clock.");
            }
        }

        private void ClearTimeline()
        {
            StopTimer();
            _slots = new List<RotatorSlot>();
            _cycleSeconds = 0;
            ViewState.CurrentItem = null;
            ViewState.IsEnabled = false;
        }

        #endregion

        #region OVERRIDES

        protected override async Task OnNavigatedIn(NavigationParameters navigationParameters, CancellationToken cancellationToken = default)
        {
            if (!_loginRotatorOptions.CurrentValue.Enabled)
                return;

            var rotateFolder = _gizmoClient.GetCurrentRotatorPath();
            if (string.IsNullOrEmpty(rotateFolder))
                return;

            List<RotatorSlot> slots;
            try
            {
                slots = BuildSlots(rotateFolder);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Failed to initialize login rotator.");
                return;
            }

            var cycleSeconds = 0d;
            foreach (var slot in slots)
                cycleSeconds += slot.DurationSeconds;

            int version;
            var hadItem = false;

            lock (_itemsLock)
            {
                hadItem = ViewState.CurrentItem != null || ViewState.IsEnabled;
                version = ++_timelineVersion;
                _serverClockOffset = TimeSpan.Zero;

                if (slots.Count == 0)
                {
                    ClearTimeline();
                }
                else
                {
                    StopTimer();
                    _slots = slots;
                    _cycleSeconds = cycleSeconds;
                }
            }

            if (slots.Count == 0)
            {
                if (hadItem)
                    DebounceViewStateChanged();
                return;
            }

            var offset = await SyncServerClockAsync(cancellationToken).ConfigureAwait(false);

            lock (_itemsLock)
            {
                if (_disposed || version != _timelineVersion || cancellationToken.IsCancellationRequested)
                    return;

                _serverClockOffset = offset;
            }

            ShowCurrent();
        }

        protected override void OnDisposing(bool isDisposing)
        {
            lock (_itemsLock)
            {
                _disposed = true;
                StopTimer();
            }

            base.OnDisposing(isDisposing);
        }

        #endregion

        private readonly record struct RotatorSlot(LoginRotatorItemViewState Item, double DurationSeconds, double StartSeconds);

        private sealed class TimerSignal
        {
            public TimerSignal(int generation, int timeline)
            {
                Generation = generation;
                Timeline = timeline;
            }

            public int Generation { get; }
            public int Timeline { get; }
        }
    }

    static class LoginRotatorDuration
    {
        // PKEY_Media_Duration and MF_PD_DURATION, both in 100-nanosecond units.
        private static readonly Guid MediaDurationFormat = new("64440490-4C8B-11D1-8B70-080036B11A03");
        private static readonly Guid MediaFoundationDuration = new("6c990d33-bb8e-477a-8598-0d5d96fcd88a");
        private const int MediaDurationId = 3;
        private const uint OpenSlowItem = 0x10;
        private const uint COINIT_MULTITHREADED = 0;
        private const uint MediaFoundationVersion = 0x00020070;
        private const uint MediaFoundationStartupNoSocket = 0x1;
        private const uint MediaSource = 0xFFFFFFFF;
        private const ushort VT_I8 = 20;
        private const ushort VT_UI8 = 21;
        private static readonly int PropVariantNativeSize = IntPtr.Size == 8 ? 24 : 16;

        public static bool TryGetSeconds(string path, out double seconds)
        {
            seconds = 0;
            if (!OperatingSystem.IsWindows())
                return false;

            var initialized = false;

            try
            {
                var hrInit = CoInitializeEx(IntPtr.Zero, COINIT_MULTITHREADED);
                initialized = hrInit >= 0;

                if (TryGetShellSeconds(path, out seconds))
                    return true;

                return TryGetMediaFoundationSeconds(path, out seconds);
            }
            catch
            {
                seconds = 0;
                return false;
            }
            finally
            {
                if (initialized)
                    CoUninitialize();
            }
        }

        private static bool TryGetShellSeconds(string path, out double seconds)
        {
            seconds = 0;
            IPropertyStore? store = null;
            var value = IntPtr.Zero;

            try
            {
                var iid = typeof(IPropertyStore).GUID;
                var hr = SHGetPropertyStoreFromParsingName(path, IntPtr.Zero, OpenSlowItem, ref iid, out store);
                if (hr < 0 || store == null)
                    return false;

                value = AllocPropVariant();
                var key = new PropertyKey(MediaDurationFormat, MediaDurationId);
                hr = store.GetValue(ref key, value);
                if (hr < 0)
                    return false;

                return TryReadHundredNanoseconds(value, out seconds);
            }
            catch
            {
                seconds = 0;
                return false;
            }
            finally
            {
                FreePropVariant(value);
                if (store != null && OperatingSystem.IsWindows())
                    Marshal.ReleaseComObject(store);
            }
        }

        private static bool TryGetMediaFoundationSeconds(string path, out double seconds)
        {
            seconds = 0;
            IMFSourceReader? reader = null;
            var started = false;
            var value = IntPtr.Zero;

            try
            {
                var hr = MFStartup(MediaFoundationVersion, MediaFoundationStartupNoSocket);
                if (hr < 0)
                    return false;

                started = true;
                hr = MFCreateSourceReaderFromURL(path, IntPtr.Zero, out reader);
                if (hr < 0 || reader == null)
                    return false;

                value = AllocPropVariant();
                var attribute = MediaFoundationDuration;
                hr = reader.GetPresentationAttribute(MediaSource, ref attribute, value);
                if (hr < 0)
                    return false;

                return TryReadHundredNanoseconds(value, out seconds);
            }
            finally
            {
                FreePropVariant(value);
                if (reader != null && OperatingSystem.IsWindows())
                    Marshal.ReleaseComObject(reader);
                if (started)
                    MFShutdown();
            }
        }

        private static bool TryReadHundredNanoseconds(IntPtr value, out double seconds)
        {
            seconds = 0;
            var vt = (ushort)Marshal.ReadInt16(value, 0);
            if (vt != VT_UI8 && vt != VT_I8)
                return false;

            var ticks = Marshal.ReadInt64(value, 8);
            if (ticks <= 0)
                return false;

            seconds = ticks / (double)TimeSpan.TicksPerSecond;
            return seconds > 0;
        }

        private static IntPtr AllocPropVariant()
        {
            var pointer = Marshal.AllocCoTaskMem(PropVariantNativeSize);
            for (var offset = 0; offset < PropVariantNativeSize; offset += 4)
                Marshal.WriteInt32(pointer, offset, 0);

            return pointer;
        }

        private static void FreePropVariant(IntPtr value)
        {
            if (value == IntPtr.Zero)
                return;

            PropVariantClear(value);
            Marshal.FreeCoTaskMem(value);
        }

        [DllImport("ole32.dll", ExactSpelling = true)]
        private static extern int CoInitializeEx(IntPtr pvReserved, uint dwCoInit);

        [DllImport("ole32.dll", ExactSpelling = true)]
        private static extern void CoUninitialize();

        [DllImport("shell32.dll", CharSet = CharSet.Unicode, PreserveSig = true)]
        private static extern int SHGetPropertyStoreFromParsingName(
            string pszPath,
            IntPtr pbc,
            uint flags,
            ref Guid riid,
            [MarshalAs(UnmanagedType.Interface)] out IPropertyStore? propertyStore);

        [DllImport("ole32.dll", ExactSpelling = true)]
        private static extern int PropVariantClear(IntPtr pvar);

        [DllImport("mfplat.dll", ExactSpelling = true)]
        private static extern int MFStartup(uint version, uint flags);

        [DllImport("mfplat.dll", ExactSpelling = true)]
        private static extern int MFShutdown();

        [DllImport("mfreadwrite.dll", ExactSpelling = true, CharSet = CharSet.Unicode, PreserveSig = true)]
        private static extern int MFCreateSourceReaderFromURL(
            string url,
            IntPtr attributes,
            [MarshalAs(UnmanagedType.Interface)] out IMFSourceReader? reader);

        [ComImport]
        [Guid("886d8eeb-8cf2-4446-8d02-cdba1dbdcf99")]
        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        private interface IPropertyStore
        {
            [PreserveSig] int GetCount(out int count);
            [PreserveSig] int GetAt(int index, IntPtr key);
            [PreserveSig] int GetValue(ref PropertyKey key, IntPtr value);
            [PreserveSig] int SetValue(ref PropertyKey key, IntPtr value);
            [PreserveSig] int Commit();
        }

        [ComImport]
        [Guid("70ae66f2-c809-4e4f-8915-bdcb406b7993")]
        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        private interface IMFSourceReader
        {
            [PreserveSig] int GetStreamSelection(uint streamIndex, [MarshalAs(UnmanagedType.Bool)] out bool selected);
            [PreserveSig] int SetStreamSelection(uint streamIndex, [MarshalAs(UnmanagedType.Bool)] bool selected);
            [PreserveSig] int GetNativeMediaType(uint streamIndex, uint mediaTypeIndex, out IntPtr mediaType);
            [PreserveSig] int GetCurrentMediaType(uint streamIndex, out IntPtr mediaType);
            [PreserveSig] int SetCurrentMediaType(uint streamIndex, IntPtr reserved, IntPtr mediaType);
            [PreserveSig] int SetCurrentPosition(ref Guid timeFormat, IntPtr position);
            [PreserveSig] int ReadSample(uint streamIndex, uint controlFlags, IntPtr actualStreamIndex, IntPtr streamFlags, IntPtr timestamp, IntPtr sample);
            [PreserveSig] int Flush(uint streamIndex);
            [PreserveSig] int GetServiceForStream(uint streamIndex, ref Guid service, ref Guid iid, out IntPtr instance);
            [PreserveSig] int GetPresentationAttribute(uint streamIndex, ref Guid attribute, IntPtr value);
        }

        [StructLayout(LayoutKind.Sequential, Pack = 4)]
        private struct PropertyKey
        {
            public PropertyKey(Guid formatId, int propertyId)
            {
                fmtid = formatId;
                pid = propertyId;
            }

            public Guid fmtid;
            public int pid;
        }
    }
}
