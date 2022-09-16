using DrawingProj.Services;
using System;
using Windows.System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace DrawingProj.Controls
{
    /// <summary>
    /// User project list item
    /// </summary>
    public sealed partial class ProjectsPageItemControl : UserControl
    {
        #region Public Fields

        /// <summary>
        /// Dependency property <see cref="SketchImagePath"/>
        /// </summary>
        public static readonly DependencyProperty SketchImagePathProperty =
            DependencyProperty.Register("SketchImagePath", typeof(Uri), typeof(ProjectsPageItemControl), new PropertyMetadata(default));

        /// <summary>
        /// Dependency property <see cref="SketchName"/>
        /// </summary>
        public static readonly DependencyProperty SketchNameProperty =
            DependencyProperty.Register("SketchName", typeof(string), typeof(ProjectsPageItemControl), new PropertyMetadata(default));

        #endregion Public Fields

        #region Public Properties

        /// <summary>
        /// Gets or sets path to sketch image
        /// </summary>
        public Uri SketchImagePath
        {
            get { return (Uri)GetValue(SketchImagePathProperty); }
            set { SetValue(SketchImagePathProperty, value); }
        }

        /// <summary>
        ///  Gets or sets display name of sketch
        /// </summary>
        public string SketchName
        {
            get { return (string)GetValue(SketchNameProperty); }
            set { SetValue(SketchNameProperty, value); }
        }

        #endregion Public Properties

        #region Public Constructors

        /// <summary>
        /// Initializes an instance of a class <see cref="ProjectsPageItemControl"/>
        /// </summary>
        public ProjectsPageItemControl()
        {
            this.InitializeComponent();
        }

        #endregion Public Constructors

        #region Private Methods

        /// <summary>
        /// Handles the click event on the "Rename" item
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RenameFlyoutItem_Click(object sender, RoutedEventArgs e)
        {
            NameSketchTextBlock.Visibility = Visibility.Collapsed;
            RenameTextBox.Visibility = Visibility.Visible;
            RenameTextBox.Focus(FocusState.Programmatic);
            RenameTextBox.SelectionStart = RenameTextBox.Text.Length;
        }

        /// <summary>
        /// Handles the KeyDown event on the textbox
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RenameTextBox_KeyDown(object sender, Windows.UI.Xaml.Input.KeyRoutedEventArgs e)
        {
            if (e.Key == VirtualKey.Enter)
            {
                RenameTextBox.Visibility = Visibility.Collapsed;
                NameSketchTextBlock.Visibility = Visibility.Visible;
            }
        }

        /// <summary>
        /// Handles the lost focus event on the textbox
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void RenameTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            NameSketchTextBlock.Visibility = Visibility.Visible;
            RenameTextBox.Visibility = Visibility.Collapsed;
            bool isSuccess = await ServiceLocator.ProjectsPageViewModel.TryRenameSketch(RenameTextBox.Text);
            if (isSuccess == false)
            {
                RenameTextBox.Text = SketchName;
            }
        }

        #endregion Private Methods
    }
}