using ColorHelper;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Input;

namespace DrawingProj.Controls
{
    /// <summary>
    /// Control to select the color of the drawing tool
    /// </summary>
    public sealed partial class ColorPickerControl : UserControl
    {
        #region Public Fields

        /// <summary>
        /// Dependency property <see cref="CurrentColor"/>
        /// </summary>
        public static readonly DependencyProperty CurrentColorProperty =
            DependencyProperty.Register("CurrentColor", typeof(Color), typeof(ColorPickerControl), new PropertyMetadata(Colors.White));

        /// <summary>
        /// Dependency property <see cref="HSLCurrentColor"/>
        /// </summary>
        public static readonly DependencyProperty HSLCurrentColorProperty =
            DependencyProperty.Register("HSLCurrentColor", typeof(HSL), typeof(ColorPickerControl), new PropertyMetadata(new HSL(0, 0, 100)));

        /// <summary>
        /// Dependency property <see cref="OpacityValue"/>
        /// </summary>
        public static readonly DependencyProperty OpacityValueProperty =
            DependencyProperty.Register("OpacityValue", typeof(int), typeof(ColorPickerControl), new PropertyMetadata(100, new PropertyChangedCallback(OpacityValueChange)));

        #endregion Public Fields

        #region Private Fields

        /// <summary>
        /// Contains examples list colors
        /// </summary>
        private ObservableCollection<Color> _exampleColorsList;

        #endregion Private Fields

        #region Public Properties

        /// <summary>
        /// Current color for current drawing tool
        /// </summary>
        public Color CurrentColor
        {
            get { return (Color)GetValue(CurrentColorProperty); }
            set
            {
                if (value != CurrentColor)
                {
                    SetValue(CurrentColorProperty, value);
                    RGB rgbColor = new RGB(value.R, value.G, value.B);
                    HSLCurrentColor = ColorConverter.RgbToHsl(rgbColor);
                }
            }
        }

        /// <summary>
        /// Gets or sets current color in HSL color model
        /// </summary>
        public HSL HSLCurrentColor
        {
            get { return (HSL)GetValue(HSLCurrentColorProperty); }
            set { SetValue(HSLCurrentColorProperty, value); }
        }

        /// <summary>
        /// Gets or sets opacity value color
        /// </summary>
        public int OpacityValue
        {
            get { return (int)GetValue(OpacityValueProperty); }
            set { SetValue(OpacityValueProperty, value); }
        }

        #endregion Public Properties

        #region Public Constructors

        /// <summary>
        /// Initializes an instance of a class <see cref="ColorPickerControl"/>
        /// </summary>
        public ColorPickerControl()
        {
            this.InitializeComponent();

            _exampleColorsList = new ObservableCollection<Color>();

            List<Color> exampleColors = new List<Color>()
            {
                Colors.Black,
                Colors.White,
                Colors.Red,
                Colors.Blue,
                Colors.Green,
                Colors.Purple,
                Colors.Transparent,
            };

            _exampleColorsList = new ObservableCollection<Color>(exampleColors);

            Loaded += ColorPickerControl_Loaded;
        }

        #endregion Public Constructors

        #region Private Methods

        /// <summary>
        /// Handles the event of change opacity value
        /// </summary>
        /// <param name="d"></param>
        /// <param name="e"></param>
        private static void OpacityValueChange(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            double convertCoefficient = 2.55;
            ColorPickerControl control = d as ColorPickerControl;
            Color currentColor = control.CurrentColor;
            currentColor.A = (byte)((int)e.NewValue * convertCoefficient);
            control.CurrentColor = currentColor;
        }

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
        /// Handles the event of change color mode
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ColorModeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (RGBModeGrid != null && HSLModeGrid != null && ExamplesModeGrid != null)
            {
                switch (ColorModeComboBox.SelectedIndex)
                {
                    case 0:
                        RGBModeGrid.Visibility = Visibility.Visible;
                        HSLModeGrid.Visibility = Visibility.Collapsed;
                        ExamplesModeGrid.Visibility = Visibility.Collapsed;
                        break;

                    case 1:
                        RGBModeGrid.Visibility = Visibility.Collapsed;
                        HSLModeGrid.Visibility = Visibility.Visible;
                        ExamplesModeGrid.Visibility = Visibility.Collapsed;
                        break;

                    case 2:
                        RGBModeGrid.Visibility = Visibility.Collapsed;
                        HSLModeGrid.Visibility = Visibility.Collapsed;
                        ExamplesModeGrid.Visibility = Visibility.Visible;
                        break;
                }
            }
        }

        /// <summary>
        /// Method for handling the end of loading the control
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ColorPickerControl_Loaded(object sender, RoutedEventArgs e)
        {
            int defaultMode = 0; //RGB mode
            ColorModeComboBox.SelectedIndex = defaultMode;
        }

        /// <summary>
        /// Handles the click event on the "Delete" item
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DeleteFlyoutItem_Click(object sender, RoutedEventArgs e)
        {
            Color currentColor = (Color)ExampleColorsGridView.SelectedItem;
            if (currentColor != Colors.Transparent)
            {
                _exampleColorsList.Remove((Color)ExampleColorsGridView.SelectedItem);
            }
        }

        /// <summary>
        /// Handles the selection changed event of the examples colors list
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ExampleColorsGridView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ExampleColorsGridView.SelectedItem == null)
                return;

            Color currentColor = (Color)ExampleColorsGridView.SelectedItem;
            if (currentColor != Colors.Transparent)
            {
                CurrentColor = currentColor;
                return;
            }

            if (_exampleColorsList.Contains(CurrentColor))
            {
                ExampleColorsGridView.SelectedIndex = -1;
                return;
            }

            int lastButOneIndex = _exampleColorsList.Count - 1;
            _exampleColorsList.Insert(lastButOneIndex, CurrentColor);
            ExampleColorsGridView.SelectedItem = _exampleColorsList.ElementAt(lastButOneIndex);
        }

        /// <summary>
        /// Handles the right click event on the GridView item
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Grid_RightTapped(object sender, RightTappedRoutedEventArgs e)
        {
            Grid selectedGrid = sender as Grid;
            Color context = (Color)selectedGrid?.DataContext;

            ExampleColorsGridView.SelectedIndex = _exampleColorsList.IndexOf(context);
        }

        /// <summary>
        /// Handles the changed value HSL chanels sliders
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void HSLChanelSlider_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            RGB rgbColor = ColorConverter.HslToRgb(HSLCurrentColor);
            Color color = Color.FromArgb(255, rgbColor.R, rgbColor.G, rgbColor.B);
            if (ColorModeComboBox.SelectedIndex == 1)
            {
                CurrentColor = color;
            }
        }

        /// <summary>
        /// Handles the move event of the control
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MainGrid_ManipulationDelta(object sender, ManipulationDeltaRoutedEventArgs e)
        {
            ControlTranslate.X += e.Delta.Translation.X;
            ControlTranslate.Y += e.Delta.Translation.Y;
        }

        /// <summary>
        /// Handles the changed value RGB chanels sliders
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RGBChanelSlider_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            if (ColorModeComboBox.SelectedIndex == 0)
            {
                CurrentColor = Color.FromArgb(255, (byte)RChanelSlider.Value, (byte)GChanelSlider.Value, (byte)BChanelSlider.Value);
            }
        }

        #endregion Private Methods
    }
}