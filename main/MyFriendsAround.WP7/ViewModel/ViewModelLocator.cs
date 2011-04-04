/*
  In App.xaml:
  <Application.Resources>
      <vm:ViewModelLocatorTemplate xmlns:vm="clr-namespace:MyFriendsAround.WP7.ViewModel"
                                   x:Key="Locator" />
  </Application.Resources>
  
  In the View:
  DataContext="{Binding Source={StaticResource Locator}, Path=ViewModelName}"
  
  OR (WPF only):
  
  xmlns:vm="clr-namespace:MyFriendsAround.WP7.ViewModel"
  DataContext="{Binding Source={x:Static vm:ViewModelLocatorTemplate.ViewModelNameStatic}}"
*/

using MyFriendsAround.WP7.Helpers.Navigation;
using MyFriendsAround.WP7.Utils;
using NetworkDetection;

namespace MyFriendsAround.WP7.ViewModel
{
    /// <summary>
    /// This class contains static references to all the view models in the
    /// application and provides an entry point for the bindings.
    /// <para>
    /// Use the <strong>mvvmlocatorproperty</strong> snippet to add ViewModels
    /// to this locator.
    /// </para>
    /// <para>
    /// In Silverlight and WPF, place the ViewModelLocatorTemplate in the App.xaml resources:
    /// </para>
    /// <code>
    /// &lt;Application.Resources&gt;
    ///     &lt;vm:ViewModelLocatorTemplate xmlns:vm="clr-namespace:MyFriendsAround.WP7.ViewModel"
    ///                                  x:Key="Locator" /&gt;
    /// &lt;/Application.Resources&gt;
    /// </code>
    /// <para>
    /// Then use:
    /// </para>
    /// <code>
    /// DataContext="{Binding Source={StaticResource Locator}, Path=ViewModelName}"
    /// </code>
    /// <para>
    /// You can also use Blend to do all this with the tool's support.
    /// </para>
    /// <para>
    /// See http://www.galasoft.ch/mvvm/getstarted
    /// </para>
    /// <para>
    /// In <strong>*WPF only*</strong> (and if databinding in Blend is not relevant), you can delete
    /// the Main property and bind to the ViewModelNameStatic property instead:
    /// </para>
    /// <code>
    /// xmlns:vm="clr-namespace:MyFriendsAround.WP7.ViewModel"
    /// DataContext="{Binding Source={x:Static vm:ViewModelLocatorTemplate.ViewModelNameStatic}}"
    /// </code>
    /// </summary>
    public class ViewModelLocator
    {

        /// <summary>
        /// Initializes a new instance of the ViewModelLocator class.
        /// </summary>
        public ViewModelLocator()
        {
            if (ViewModelBase.IsInDesignModeStatic)
            {
                // Create design time view models
            }
            else
            {
                //Register PageNavigation - only if not design time
                Container.Instance.RegisterInstance(typeof(PageNavigation), "PageNavigation");

                // Create run time view models
            }

        }

        /// <summary>
        /// Gets the Main property.
        /// </summary>
        public MainViewModel Main
        {
            get
            {
                MainViewModel mainViewModel = GetViewModel<MainViewModel>(Constants.VM_MAIN);
                return mainViewModel;
            }
        }

        /// <summary>
        /// Gets the About property.
        /// </summary>
        public SettingsViewModel Settings
        {
            get
            {
                SettingsViewModel aboutViewModel = GetViewModel<SettingsViewModel>(Constants.VM_SETTINGS);
                return aboutViewModel;
            }
        }


        /// <summary>
        /// Cleans up all the resources.
        /// </summary>
        public void Cleanup()
        {
            MainViewModel mainViewModel = GetViewModel<MainViewModel>(Constants.VM_MAIN);
            mainViewModel.Cleanup();
            SettingsViewModel aboutViewModel = GetViewModel<SettingsViewModel>(Constants.VM_SETTINGS);
            aboutViewModel.Cleanup();
        }



        #region Local Helpers

        public static T GetViewModel<T>(string key) where T : ViewModelBase
        {
            // Create a new view model
            T vm = Container.Instance.Resolve<T>(key);
            //Assign the Context from PageNavigation to Context property of the ViewModelBase
            vm.Context = vm.PageNav.CurrentContext;

            return vm;
        }

        #endregion
    }
}