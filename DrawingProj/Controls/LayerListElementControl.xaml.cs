using DrawingProj.Services;
using Windows.ApplicationModel.Resources;
using Windows.System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace DrawingProj.Controls
{
    /// <summary>
    /// Control for view layer list item
    /// </summary>
    public sealed partial class LayerListElementControl : UserControl
    {
        #region Public Fields

        /// <summary>
        /// Dependency property <see cref="IsLock"/>
        /// </summary>
        public static readonly DependencyProperty IsLockProperty =
            DependencyProperty.Register("IsLock", typeof(bool), typeof(LayerListElementControl), new PropertyMetadata(false));

        /// <summary>
        /// Dependency property <see cref="IsVisible"/>
        /// </summary>
        public static readonly DependencyProperty IsVisibleProperty =
            DependencyProperty.Register("IsVisible", typeof(bool), typeof(LayerListElementControl), new PropertyMetadata(true));

        /// <summary>
        /// Dependency property <see cref="LayerName"/>
        /// </summary>
        public static readonly DependencyProperty LayerNameProperty =
            DependencyProperty.Register("LayerName", typeof(string), typeof(LayerListElementControl), new PropertyMetadata(default));

        /// <summary>
        /// Dependency property <see cref="PreviewImageSource"/>
        /// </summary>
        public static readonly DependencyProperty PreviewImageSourceProperty =
            DependencyProperty.Register("PreviewImageSource", typeof(ImageSource), typeof(LayerListElementControl), new PropertyMetadata(null));

        #endregion Public Fields

        #region Public Properties

        /// <summary>
        /// Gets or sets bool flag that this layer is locked
        /// </summary>
        public bool IsLock
        {
            get { return (bool)GetValue(IsLockProperty); }
            set { SetValue(IsLockProperty, value); }
        }

        /// <summary>
        /// Gets or sets bool flag that this layer is visible
        /// </summary>
        public bool IsVisible
        {
            get { return (bool)GetValue(IsVisibleProperty); }
            set { SetValue(IsVisibleProperty, value); }
        }

        /// <summary>
        /// Gets or sets display name of layer instance
        /// </summary>
        public string LayerName
        {
            get { return (string)GetValue(LayerNameProperty); }
            set { SetValue(LayerNameProperty, value); }
        }

        /// <summary>
        /// Gets or sets resource for displaying drawn lines on the layer preview
        /// </summary>
        public ImageSource PreviewImageSource
        {
            get { return (ImageSource)GetValue(PreviewImageSourceProperty); }
            set { SetValue(PreviewImageSourceProperty, value); }
        }

        #endregion Public Properties

        #region Public Constructors

        /// <summary>
        /// Initializes an instance of a class <see cref="LayerListElementControl"/>
        /// </summary>
        public LayerListElementControl()
        {
            this.InitializeComponent();
            Loaded += LayerListElementControl_Loaded;
        }

        #endregion Public Constructors

        #region Private Methods

        /// <summary>
        /// Handles the end of control initialization event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LayerListElementControl_Loaded(object sender, RoutedEventArgs e)
        {
            var resourceLoader = ResourceLoader.GetForCurrentView();
            ShowCommand.Text = resourceLoader.GetString(IsVisible ? "Hide" : "Show");
            LockCommand.Text = resourceLoader.GetString(IsLock ? "Unblock" : "Block");
        }

        /// <summary>
        /// Handles the click event on the lock layer command
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LockCommand_Click(object sender, RoutedEventArgs e)
        {
            IsLock = !IsLock;
            var resourceLoader = ResourceLoader.GetForCurrentView();
            LockCommand.Text = resourceLoader.GetString(IsLock ? "Unblock" : "Block");
        }

        /// <summary>
        /// Handles the click event on the "Rename" item
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RenameCommand_Click(object sender, RoutedEventArgs e)
        {
            RenameLayerTextBox.Visibility = Visibility.Visible;
            NameLayerText.Visibility = Visibility.Collapsed;
            RenameLayerTextBox.Focus(FocusState.Programmatic);
            RenameLayerTextBox.SelectionStart = RenameLayerTextBox.Text.Length;
        }

        /// <summary>
        /// Handles the KeyDown event on the textbox
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RenameLayerTextBox_KeyDown(object sender, Windows.UI.Xaml.Input.KeyRoutedEventArgs e)
        {
            if (e.Key == VirtualKey.Enter)
            {
                RenameLayerTextBox.Visibility = Visibility.Collapsed;
                NameLayerText.Visibility = Visibility.Visible;
            }
        }

        /// <summary>
        /// Handles the lost focus event on the textbox
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void RenameLayerTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            NameLayerText.Visibility = Visibility.Visible;
            RenameLayerTextBox.Visibility = Visibility.Collapsed;
            bool isSuccess = await ServiceLocator.SketchPageViewModel.TryRenameLayer(RenameLayerTextBox.Text);
            if (isSuccess == false)
            {
                RenameLayerTextBox.Text = LayerName;
            }
        }

        /// <summary>
        /// Handles the click event on the show layer command
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ShowCommand_Click(object sender, RoutedEventArgs e)
        {
            IsVisible = !IsVisible;
            var resourceLoader = ResourceLoader.GetForCurrentView();
            ShowCommand.Text = resourceLoader.GetString(IsVisible ? "Hide" : "Show");
        }

        #endregion Private Methods
    }
}