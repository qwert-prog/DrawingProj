using IoC;

#if !DEBUG
using Microsoft.AppCenter;
using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;
#endif

using DrawingProj.Model;
using DrawingProj.Services;
using DrawingProj.View;
using DrawingProj.ViewModels;
using Serilog;
using System;
using System.IO;
using System.Threading.Tasks;
using Utils.Helpers;
using Utils.Navigation;
using Utils.Services;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.Storage;
using Windows.UI.Core.Preview;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace DrawingProj
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    public sealed partial class App : Application
    {
        #region Public Constructors

        /// <summary>
        /// Initializes the singleton application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App()
        {
            this.InitializeComponent();
            this.Suspending += OnSuspending;
            RequestedTheme = ApplicationTheme.Light;
        }

        #endregion Public Constructors

        #region Protected Methods

        /// <summary>
        /// Initialize application
        /// </summary>
        /// <param name="e">Details about the launch request and process.</param>
        protected override async void OnLaunched(LaunchActivatedEventArgs e)
        {
            if (e.PreviousExecutionState != ApplicationExecutionState.Running)
            {
                bool loadState = (e.PreviousExecutionState == ApplicationExecutionState.Terminated);
                ExtendedSplash extendedSplash = new ExtendedSplash(e.SplashScreen, loadState);
                Window.Current.Content = extendedSplash;
            }

#if !DEBUG
            AppCenter.Start("0ad0dac7-3ede-4940-9a5e-d3c0c3114287",
            typeof(Analytics), typeof(Crashes));
#endif

            Window.Current.Activate();

            ConfigurateServices();
            await ProjectPageViewModelInitialize();
            await AssetsServiceInitializeAsync();

            LaunchApp(e);
        }

        #endregion Protected Methods

        #region Private Methods

        /// <summary>
        /// Handles the click event on the application's close button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void App_CloseRequested(object sender, SystemNavigationCloseRequestedPreviewEventArgs e)
        {
            var deferral = e.GetDeferral();
            SketchPageViewModel sketchPageViewModel = ServiceLocator.SketchPageViewModel;
            Sketch currentSketch = sketchPageViewModel.CurrentSketch;
            ProjectsPageViewModel projectsPageViewModel = ServiceLocator.ProjectsPageViewModel;
            await projectsPageViewModel.SetSketchesListInLocalFolder();
            projectsPageViewModel.WriteSketchesCanvasPathInLocalSettings();
            if (currentSketch != null)
            {
                await LocalStorageFileHelper.SaveSketch(currentSketch.ImagePath);
            }
            deferral.Complete();
        }

        /// <summary>
        /// Initialize assetsservice
        /// </summary>
        /// <returns></returns>
        private async Task AssetsServiceInitializeAsync()
        {
            AssetsService _assetsService = ServicesContainer.GetService<AssetsService>();
            _assetsService.AssetsUpdated += ServicesContainer.GetService<ListToolsService>().AssetsService_AssetsUpdated;
            await _assetsService.InitializeAsync();
        }

        /// <summary>
        /// Configurate service container
        /// </summary>
        private void ConfigurateServices()
        {
            ServicesContainer.AddSingletonService<NavigationService>();
            ServicesContainer.AddSingletonService<AssetsService>();
            ServicesContainer.AddSingletonService<ListToolsService>();
            ServicesContainer.AddSingletonService<RenderUIElementService>();

            // View models
            ServicesContainer.AddSingletonService<ProjectsPageViewModel>();
            ServicesContainer.AddSingletonService<SketchPageViewModel>();

            ServicesContainer.BuildServices();
        }

        /// <summary>
        /// Invoked when the application is launched normally by the end user.  Other entry points
        /// will be used such as when the application is launched to open a specific file.
        /// </summary>
        /// <param name="e">Details about the launch request and process.</param>
        private void LaunchApp(LaunchActivatedEventArgs e)
        {
            Frame rootFrame = Window.Current.Content as Frame;

            // Do not repeat app initialization when the Window already has content,
            // just ensure that the window is active
            if (rootFrame == null)
            {
                // Create a Frame to act as the navigation context and navigate to the first page
                rootFrame = new Frame();

                rootFrame.NavigationFailed += OnNavigationFailed;

                if (e.PreviousExecutionState == ApplicationExecutionState.Terminated)
                {
                    //TODO: Load state from previously suspended application
                }

                // Place the frame in the current Window
                Window.Current.Content = rootFrame;
            }

            if (e.PrelaunchActivated == false)
            {
                if (rootFrame.Content == null)
                {
                    // When the navigation stack isn't restored navigate to the first page,
                    // configuring the new page by passing required information as a navigation
                    // parameter
                    rootFrame.Navigate(typeof(MainPage), e.Arguments);
                }
                SystemNavigationManagerPreview.GetForCurrentView().CloseRequested += App_CloseRequested;
                // Ensure the current window is active
                Window.Current.Activate();
            }
        }

        /// <summary>
        /// Invoked when Navigation to a certain page fails
        /// </summary>
        /// <param name="sender">The Frame which failed navigation</param>
        /// <param name="e">Details about the navigation failure</param>
        private void OnNavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            throw new Exception("Failed to load Page " + e.SourcePageType.FullName);
        }

        /// <summary>
        /// Invoked when application execution is being suspended.  Application state is saved
        /// without knowing whether the application will be terminated or resumed with the contents
        /// of memory still intact.
        /// </summary>
        /// <param name="sender">The source of the suspend request.</param>
        /// <param name="e">Details about the suspend request.</param>
        private void OnSuspending(object sender, SuspendingEventArgs e)
        {
            var deferral = e.SuspendingOperation.GetDeferral();
            //TODO: Save application state and stop any background activity
            deferral.Complete();
        }

        /// <summary>
        /// Method fot initialize sketches list in projects page
        /// </summary>
        /// <returns></returns>
        private async Task ProjectPageViewModelInitialize()
        {
            ProjectsPageViewModel projectsPageViewModel = ServiceLocator.ProjectsPageViewModel;
            await projectsPageViewModel.InitializeSketchesListAsync();
        }

        #endregion Private Methods
    }
}