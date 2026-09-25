using Gizmo.Web.Api.Models;

namespace Gizmo.Client.UI.Services
{
    internal static class UserAchievementMapper
    {
        /// <param name="filesBaseUrl">Absolute files endpoint base ending with "/files/".</param>
        internal static UserAchievement Map(UserAchievementModel source, string filesBaseUrl) =>
            new()
            {
                AchievementId          = source.AchievementId,
                Name                   = source.Name,
                Description            = source.Description,
                ImageUrl               = source.ImageGuid is null ? null : filesBaseUrl + source.ImageGuid.Value.ToString("N"),
                ImageIsSvg             = source.ImageMimeType?.Contains("svg", StringComparison.OrdinalIgnoreCase) == true,
                Unit                   = AchievementEnumMapper.Map(source.Unit),
                Range                  = AchievementEnumMapper.Map(source.Range),
                TargetValue            = source.TargetValue,
                MaxCompletionsPerRange = source.MaxCompletionsPerRange,
                IsHidden               = source.IsHidden,
                State                  = AchievementEnumMapper.Map(source.State),
                TotalCompletions       = source.TotalCompletions,
                InstanceStart          = source.InstanceStart,
                InstanceEnd            = source.InstanceEnd,
                InstanceCompletions    = source.InstanceCompletions,
                CurrentValue           = source.CurrentValue,
                Progress               = source.Progress,
            };
    }
}
