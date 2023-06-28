using Gizmo.UI.View.States;
using Microsoft.Extensions.DependencyInjection;

namespace Gizmo.Client.UI.View.States
{
    [Register(Scope = RegisterScope.Transient)]
    public sealed class PasswordTooltipViewState : ViewStateBase
    {
        #region PROPERTIES

        public int? MinimumLengthRule { get; internal set; }

        public int? MaximumLengthRule { get; internal set; }

        public bool HasLowerCaseCharactersRule { get; internal set; }

        public bool HasUpperCaseCharactersRule { get; internal set; }

        public bool HasNumbersRule { get; internal set; }

        public bool HasSpecialCharactersRule { get; internal set; }

        public int TotalRules { get; internal set; }

        public bool LengthRulePassed { get; internal set; }

        public bool LowerCaseCharactersRulePassed { get; internal set; }

        public bool UpperCaseCharactersRulePassed { get; internal set; }

        public bool NumbersRulePassed { get; internal set; }

        public bool SpecialCharactersRulePassed { get; internal set; }

        public int PassedRules { get; internal set; }

        public string ErrorMessage { get; internal set; } = string.Empty;

        #endregion
    }
}
