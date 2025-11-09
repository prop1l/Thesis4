using System.ComponentModel;
using System.Runtime.CompilerServices;
using ThesisCourse_4.Services;

namespace ThesisCourse_4.MVVM.ViewModels
{
    public class WelcomeViewModels : ThemedViewModelBase
    {
        public WelcomeViewModels(IThemeService themeService) : base(themeService){}



    }
}
