using DrawingProj.Controls;
using DrawingProj.Services;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace DrawingProj.View
{
    /// <summary>
    /// Page with projects
    /// </summary>
    public sealed partial class ProjectsPage : Page
    {
        #region Public Constructors

        /// <summary>
        /// Initializes an instance of a class <see cref="ProjectsPage"/>
        /// </summary>
        public ProjectsPage()
        {
            this.InitializeComponent();
        }

        #endregion Public Constructors

        #region Private Methods

        /// <summary>
        /// Handles the right click event on the GridView item
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MainGrid_RightTapped(object sender, Windows.UI.Xaml.Input.RightTappedRoutedEventArgs e)
        {
            ProjectsPageItemControl selectedGrid = sender as ProjectsPageItemControl;
            var context = selectedGrid?.DataContext;

            ProjectsGridVew.SelectedItem = context;
        }

        /// <summary>
        /// Handles the left click event on the GridView item
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MainGrid_Tapped(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            ProjectsPageItemControl selectedGrid = sender as ProjectsPageItemControl;
            var context = selectedGrid?.DataContext;

            ProjectsGridVew.SelectedItem = context;

            ServiceLocator.ProjectsPageViewModel.OpenSketch();
        }

        #endregion Private Methods
    }
}