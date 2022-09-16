using IoC;
using DrawingProj.Services;
using DrawingProj.View;
using DrawingProj.ViewModels;
using System.Collections.Generic;
using System.Threading.Tasks;
using Utils.Navigation;
using Utils.Navigation.Interfaces;
using Windows.Globalization;
using Windows.UI.Xaml.Controls;

namespace DrawingProj
{
    public sealed partial class MainPage : Page
    {
        #region Private Fields

        /// <summary>
        /// Represents navigation service.
        /// </summary>
        private static INavigationService _navigator;

        /// <summary>
        /// Represents router for navigation.
        /// </summary>
        private static IRouter _router;

        #endregion Private Fields

        #region Public Constructors

        /// <summary>
        /// Initializes an instance of a class <see cref="MainPage"/>
        /// </summary>
        public MainPage()
        {
            this.InitializeComponent();

#if DEBUG
            ChangeLanguage();
#endif

            _router = new Router();
            RegisterRoutes(_router);

            ConfigureNavigation();
        }

        #endregion Public Constructors

        #region Private Methods

        /// <summary>
        /// Setting up navigation and switching to <see cref="ProjectsPage"/>
        /// </summary>
        private void ConfigureNavigation()
        {
            _navigator = ServicesContainer.GetService<NavigationService>();
            _navigator.Configure(_router, MainFrame);

            ProjectsPageViewModel projectsPageVM = ServiceLocator.ProjectsPageViewModel;
            _navigator.NavigateTo(projectsPageVM, default);
        }

        /// <summary>
        /// Executed registration of services collection.
        /// </summary>
        /// <param name="router">Router.</param>
        private void RegisterRoutes(IRouter router)
        {
            ProjectsPageViewModel projectsPageVM = ServicesContainer.GetService<ProjectsPageViewModel>();
            router.RegisterRoute<ProjectsPage>(projectsPageVM);
            SketchPageViewModel sketchPageVM = ServicesContainer.GetService<SketchPageViewModel>();
            router.RegisterRoute<SketchPage>(sketchPageVM);
        }

        /// <summary>
        /// Add ComboBox to change languages ​​while debugging
        /// </summary>
        private void ChangeLanguage()
        {
            ComboBox combobox = new ComboBox();
            Canvas.SetZIndex(combobox, 100);
            combobox.Items.Clear();
            combobox.IsTabStop = false;
            foreach (string item in new List<string>() { "en-Us", "de-DE", "es-ES", "fr-FR", "it-IT", "ja-JP", "ko-KR", "pt-PT", "zh-CN" })
            {
                combobox.Items.Add(item);
            }
            combobox.SelectionChanged += Combobox_SelectionChanged;
            Grid gr = Content as Grid;

            gr.Children.Add(combobox);
        }

        /// <summary>
        /// Handles the language selection event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void Combobox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string text = (sender as ComboBox).SelectedItem as string;

            if (text != null)
            {
                ApplicationLanguages.PrimaryLanguageOverride = text;
                await Task.Delay(1000);
                Frame.Navigate(GetType());
            }
        }

        #endregion Private Methods
    }
}