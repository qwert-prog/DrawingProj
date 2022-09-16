using DrawingProj.Model;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;

namespace DrawingProj.Controls
{
    /// <summary>
    /// Control for changing the size, transparency and shape of the tool
    /// </summary>
    public sealed partial class ToolsSettingsControl : UserControl
    {
        #region Public Fields

        /// <summary>
        /// Dependency property <see cref="CurrentDrawingTool"/>
        /// </summary>
        public static readonly DependencyProperty CurrentDrawingToolProperty =
            DependencyProperty.Register("CurrentDrawingTool", typeof(DrawingTool), typeof(ToolsSettingsControl), new PropertyMetadata(default));

        /// <summary>
        /// Dependency property <see cref="CurrentFormTool"/>
        /// </summary>
        public static readonly DependencyProperty CurrentFormToolProperty =
            DependencyProperty.Register("CurrentFormTool", typeof(string), typeof(ToolsSettingsControl), new PropertyMetadata(default));

        #endregion Public Fields

        #region Public Properties

        /// <summary>
        /// The current tool for which the parameters will be changed
        /// </summary>
        public DrawingTool CurrentDrawingTool
        {
            get { return (DrawingTool)GetValue(CurrentDrawingToolProperty); }
            set { SetValue(CurrentDrawingToolProperty, value); }
        }

        /// <summary>
        /// Gets or sets current form of drawing tool
        /// </summary>
        public string CurrentFormTool
        {
            get { return (string)GetValue(CurrentFormToolProperty); }
            set { SetValue(CurrentFormToolProperty, value); }
        }

        #endregion Public Properties

        #region Public Constructors

        /// <summary>
        /// Initializes an instance of a class <see cref="ToolsSettingsControl"/>
        /// </summary>
        public ToolsSettingsControl()
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
        /// Handles the move event of the control
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DrawingToolSettingsGrid_ManipulationDelta(object sender, ManipulationDeltaRoutedEventArgs e)
        {
            ControlTranslate.X += e.Delta.Translation.X;
            ControlTranslate.Y += e.Delta.Translation.Y;
        }

        #endregion Private Methods
    }
}