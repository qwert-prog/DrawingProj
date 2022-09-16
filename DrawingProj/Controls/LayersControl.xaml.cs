using DrawingProj.Model;
using System.Collections.ObjectModel;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace DrawingProj.Controls
{
    /// <summary>
    /// Control for displaying and managing the list of layers
    /// </summary>
    public sealed partial class LayersControl : UserControl
    {
        #region Public Fields

        /// <summary>
        /// Dependency property <see cref="CurrentLayer"/>
        /// </summary>
        public static readonly DependencyProperty CurrentLayerProperty =
            DependencyProperty.Register("CurrentLayer", typeof(Layer), typeof(LayersControl), new PropertyMetadata(null));

        /// <summary>
        /// Dependency property <see cref="LayersList"/>
        /// </summary>
        public static readonly DependencyProperty LayersListProperty =
            DependencyProperty.Register("LayersList", typeof(ObservableCollection<Layer>), typeof(LayersControl), new PropertyMetadata(null));

        #endregion Public Fields

        #region Public Properties

        /// <summary>
        /// Gets or sets current selected layer
        /// </summary>
        public Layer CurrentLayer
        {
            get { return (Layer)GetValue(CurrentLayerProperty); }
            set { SetValue(CurrentLayerProperty, value); }
        }

        /// <summary>
        /// Gets or sets layers list
        /// </summary>
        public ObservableCollection<Layer> LayersList
        {
            get { return (ObservableCollection<Layer>)GetValue(LayersListProperty); }
            set { SetValue(LayersListProperty, value); }
        }

        #endregion Public Properties

        #region Public Constructors

        /// <summary>
        /// Initializes an instance of a class <see cref="LayersControl"/>
        /// </summary>
        public LayersControl()
        {
            this.InitializeComponent();
        }

        #endregion Public Constructors

        #region Private Methods

        /// <summary>
        /// Handles the event of clicking on the button to close the settings window
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CloseDrawingTollSettings_Click(object sender, RoutedEventArgs e)
        {
            this.Visibility = Visibility.Collapsed;
        }

        /// <summary>
        /// Handles the left click event on the ListBox item
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LayerItemControl_RightTapped(object sender, Windows.UI.Xaml.Input.RightTappedRoutedEventArgs e)
        {
            LayerListElementControl selectedGrid = sender as LayerListElementControl;
            var context = selectedGrid?.DataContext;

            LayersListBox.SelectedItem = context;
        }

        /// <summary>
        /// Handles the move event of the control
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MainGrid_ManipulationDelta(object sender, Windows.UI.Xaml.Input.ManipulationDeltaRoutedEventArgs e)
        {
            ControlTranslate.X += e.Delta.Translation.X;
            ControlTranslate.Y += e.Delta.Translation.Y;
        }

        #endregion Private Methods
    }
}