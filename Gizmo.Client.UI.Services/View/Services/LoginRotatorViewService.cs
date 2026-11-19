using System.Globalization;
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
        private const int VideoResyncEveryMs = 2_000;
        private const double SeekToleranceSeconds = 0.15;
        private const double MinimumTickSeconds = 0.05;
        private const int ClockSampleCount = 4;
        private const int ClockSampleTimeoutMs = 500;
        private const double ClockRefineThresholdMs = 40;
        private const double ClockRefreshThresholdMs = 80;
        private const int ClockRefreshMinutes = 5;
        private const string ServerTimeHeader = "X-Server-Time";

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
        private long _acceptedRttTicks = long.MaxValue;
        private bool _hasClockSample;
        private DateTime _nextClockSyncUtc = DateTime.MaxValue;
        private int _clockSurveys;
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

        private bool TryCreateClockUri(out Uri uri)
        {
            var serverUri = _networkOptions.CurrentValue.ServerUri;
            if (!string.IsNullOrWhiteSpace(serverUri) && Uri.TryCreate(serverUri, UriKind.Absolute, out uri!))
                return true;

            uri = null!;
            Logger.LogWarning("Login rotator clock sync skipped, server uri is not set.");
            return false;
        }

        private async Task SurveyClockAsync(int version, CancellationToken cancellationToken, bool initial)
        {
            Interlocked.Increment(ref _clockSurveys);

            try
            {
                if (!TryCreateClockUri(out var uri))
                {
                    if (initial)
                        ShowIfCurrent(version);
                    return;
                }

                var showed = false;
                var anySample = false;
                ClockSample? periodicBest = null;
                Exception? failure = null;

                using var handler = new HttpClientHandler
                {
                    ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
                };
                using var client = new HttpClient(handler)
                {
                    Timeout = TimeSpan.FromMilliseconds(ClockSampleTimeoutMs)
                };

                for (var sampleIndex = 0; sampleIndex < ClockSampleCount; sampleIndex++)
                {
                    if (cancellationToken.IsCancellationRequested || !IsTimeline(version))
                        break;

                    ClockSample? sample;
                    try
                    {
                        sample = await TrySampleClockAsync(client, uri, cancellationToken).ConfigureAwait(false);
                    }
                    catch (Exception ex) when (ex is not OperationCanceledException)
                    {
                        failure = ex;
                        continue;
                    }

                    if (sample == null)
                        continue;

                    anySample = true;

                    if (!initial)
                    {
                        if (periodicBest == null || sample.Value.RttTicks < periodicBest.Value.RttTicks)
                            periodicBest = sample;
                        continue;
                    }

                    // The first answer starts the reel. A later sample replaces it only when the
                    // round trip is no worse and the offset moved by more than the refine threshold.
                    if (!TryAcceptClock(version, sample.Value, ClockRefineThresholdMs, acceptFirst: !showed))
                        continue;

                    showed = true;
                    ShowCurrent();
                }

                if (!initial && periodicBest != null && TryAcceptClock(version, periodicBest.Value, ClockRefreshThresholdMs, acceptFirst: false))
                    ShowCurrent();

                if (initial && !showed)
                    ShowIfCurrent(version);

                if (!anySample && !cancellationToken.IsCancellationRequested && IsTimeline(version))
                {
                    if (failure != null)
                        Logger.LogWarning(failure, "Login rotator clock sync failed for {serverUri}.", uri);
                    else
                        Logger.LogWarning("Login rotator clock sync failed for {serverUri}.", uri);
                }
            }
            finally
            {
                lock (_itemsLock)
                {
                    if (version == _timelineVersion)
                        _nextClockSyncUtc = DateTime.UtcNow.AddMinutes(ClockRefreshMinutes);
                }

                Interlocked.Decrement(ref _clockSurveys);
            }
        }

        private async Task<ClockSample?> TrySampleClockAsync(HttpClient client, Uri uri, CancellationToken cancellationToken)
        {
            using var timeout = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            timeout.CancelAfter(ClockSampleTimeoutMs);

            try
            {
                using var request = new HttpRequestMessage(HttpMethod.Get, uri);
                var started = DateTime.UtcNow;
                using var response = await client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, timeout.Token).ConfigureAwait(false);
                var finished = DateTime.UtcNow;

                if (!TryReadServerUtc(response, out var serverUtc))
                    return null;

                // The header is the moment the server sent the response. Half of the round trip places that moment on our clock.
                var rttTicks = (finished - started).Ticks;
                if (rttTicks < 0)
                    rttTicks = 0;

                var localAtServer = started + TimeSpan.FromTicks(rttTicks / 2);
                return new ClockSample(serverUtc - localAtServer, rttTicks);
            }
            catch (OperationCanceledException) when (!cancellationToken.IsCancellationRequested)
            {
                return null;
            }
        }

        private static bool TryReadServerUtc(HttpResponseMessage response, out DateTime serverUtc)
        {
            if (response.Headers.TryGetValues(ServerTimeHeader, out var values))
            {
                var raw = values.FirstOrDefault();
                if (long.TryParse(raw, NumberStyles.Integer, CultureInfo.InvariantCulture, out var ticks)
                    && ticks > DateTime.MinValue.Ticks
                    && ticks <= DateTime.MaxValue.Ticks)
                {
                    serverUtc = new DateTime(ticks, DateTimeKind.Utc);
                    return true;
                }
            }

            if (response.Headers.Date is DateTimeOffset serverDate)
            {
                serverUtc = serverDate.UtcDateTime;
                return true;
            }

            serverUtc = default;
            return false;
        }

        private bool IsTimeline(int version)
        {
            lock (_itemsLock)
                return !_disposed && version == _timelineVersion;
        }

        private void ShowIfCurrent(int version)
        {
            if (!IsTimeline(version))
                return;

            ShowCurrent();
        }

        private bool TryAcceptClock(int version, ClockSample sample, double thresholdMs, bool acceptFirst)
        {
            lock (_itemsLock)
            {
                if (_disposed || version != _timelineVersion)
                    return false;

                if (acceptFirst && !_hasClockSample)
                {
                    StoreClock(sample);
                    return true;
                }

                if (_hasClockSample && sample.RttTicks > _acceptedRttTicks)
                    return false;

                if (!_hasClockSample || sample.RttTicks < _acceptedRttTicks)
                    _acceptedRttTicks = sample.RttTicks;

                var deltaMs = Math.Abs((sample.Offset - _serverClockOffset).TotalMilliseconds);
                if (deltaMs <= thresholdMs)
                {
                    _hasClockSample = true;
                    return false;
                }

                StoreClock(sample);
                return true;
            }
        }

        private void StoreClock(ClockSample sample)
        {
            _serverClockOffset = sample.Offset;
            _acceptedRttTicks = sample.RttTicks;
            _hasClockSample = true;
            Logger.LogInformation("Login rotator server clock offset is {offsetMs} ms, round trip {rttMs} ms.",
                (int)sample.Offset.TotalMilliseconds,
                (int)(sample.RttTicks / TimeSpan.TicksPerMillisecond));
        }

        private void TryStartClockRefresh(TimerSignal? signal)
        {
            if (Volatile.Read(ref _clockSurveys) > 0)
                return;

            int version;
            lock (_itemsLock)
            {
                if (_disposed || _slots.Count == 0)
                    return;

                if (signal != null && signal.Timeline != _timelineVersion)
                    return;

                if (DateTime.UtcNow < _nextClockSyncUtc)
                    return;

                // Hold the slot until this survey finishes, so the 2 s video tick cannot start a second one.
                _nextClockSyncUtc = DateTime.MaxValue;
                version = _timelineVersion;
            }

            _ = RefreshClockAsync(version);
        }

        private async Task RefreshClockAsync(int version)
        {
            try
            {
                await SurveyClockAsync(version, CancellationToken.None, initial: false).ConfigureAwait(false);
            }
            catch (Exception ex) when (ex is not OperationCanceledException)
            {
                Logger.LogWarning(ex, "Login rotator clock sync failed.");
            }
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

                ArmTimer(remainingSeconds, item.IsVideo);
            }

            if (changed)
                RaiseViewStateChanged();
        }

        private void StopTimer()
        {
            Interlocked.Increment(ref _timerGeneration);
            _rotatateTimer?.Dispose();
            _rotatateTimer = null;
        }

        private void ArmTimer(double remainingSeconds, bool isVideo)
        {
            StopTimer();

            if (_disposed)
                return;

            var capMs = isVideo ? VideoResyncEveryMs : ResyncEveryMs;
            var delayMs = (int)Math.Min(capMs, Math.Max(MinimumTickSeconds * 1000, Math.Round(remainingSeconds * 1000)));
            _rotatateTimer = new Timer(OnTimer, new TimerSignal(_timerGeneration, _timelineVersion), delayMs, Timeout.Infinite);
        }

        private void OnTimer(object? state)
        {
            var signal = state as TimerSignal;

            try
            {
                ShowCurrent(signal);
                TryStartClockRefresh(signal);
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
                _hasClockSample = false;
                _acceptedRttTicks = long.MaxValue;

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

            await SurveyClockAsync(version, cancellationToken, initial: true).ConfigureAwait(false);
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

        private readonly record struct ClockSample(TimeSpan Offset, long RttTicks);

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

                // Media Foundation first: one Windows API on every client, and closer to the duration WebView2 plays.
                if (TryGetMediaFoundationSeconds(path, out seconds))
                    return true;

                return TryGetShellSeconds(path, out seconds);
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
