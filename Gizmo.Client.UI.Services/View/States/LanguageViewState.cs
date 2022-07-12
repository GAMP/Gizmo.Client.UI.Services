using Gizmo.UI.View.States;
using Microsoft.Extensions.DependencyInjection;
using System.ComponentModel;

namespace Gizmo.Client.UI.View.States
{
    [Register(Scope = RegisterScope.Transient)]
    public sealed class LanguageViewState : ViewStateBase
    {
        #region FIELDS
        private string _nativeName = string.Empty;
        private string _threeLetterName = string.Empty;
        private int _lcid = 0;
        #endregion

        #region PROPERTIES

        /// <summary>
        /// Gets native language name, eg English,Русский etc
        /// </summary>
        [DefaultValue(DefaultValues.EMPTY_STRING_VALUE)]
        public string NativeName
        {
            get { return _nativeName; }
            internal set { SetProperty(ref _nativeName, value); }
        }

        /// <summary>
        /// Gets three letter name.
        /// </summary>
        [DefaultValue(DefaultValues.EMPTY_STRING_VALUE)]
        public string ThreeLetterName
        {
            get { return _threeLetterName; }
            internal set { SetProperty(ref _threeLetterName, value); }
        }

        /// <summary>
        /// Gets windows language code id.
        /// </summary>
        [DefaultValue(0)]
        public int LCID
        {
            get { return _lcid; }
            internal set
            {
                SetProperty(ref _lcid, value);
            }
        }

        #endregion
    }
}
