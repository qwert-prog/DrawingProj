using System;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace DrawingProj.Controls
{
    /// <summary>
    /// Сontrol for the drawing tool view
    /// </summary>
    public sealed partial class DrawingToolControl : UserControl
    {
        #region Public Fields

        /// <summary>
        /// Dependency property <see cref="ColorImagePath"/>
        /// </summary>
        public static readonly DependencyProperty ColorImagePathProperty =
            DependencyProperty.Register("ColorImagePath", typeof(Uri), typeof(DrawingToolControl), new PropertyMetadata(null));

        /// <summary>
        /// Dependency property <see cref="DrawingToolColor"/>
        /// </summary>
        public static readonly DependencyProperty DrawingToolColorProperty =
                    DependencyProperty.Register("DrawingToolColor", typeof(Color), typeof(DrawingToolControl), new PropertyMetadata(default));

        /// <summary>
        /// Dependency property <see cref="IsPremium"/>
        /// </summary>
        public static readonly DependencyProperty IsPremiumProperty =
            DependencyProperty.Register("IsPremium", typeof(bool), typeof(DrawingToolControl), new PropertyMetadata(true));

        /// <summary>
        /// Dependency property <see cref="IsSelected"/>
        /// </summary>
        public static readonly DependencyProperty IsSelectedProperty =
                    DependencyProperty.Register("IsSelected", typeof(bool), typeof(DrawingToolControl), new PropertyMetadata(false));

        /// <summary>
        /// Dependency property <see cref="MainImagePath"/>
        /// </summary>
        public static readonly DependencyProperty MainImagePathProperty =
            DependencyProperty.Register("MainImagePath", typeof(Uri), typeof(DrawingToolControl), new PropertyMetadata(null));

        /// <summary>
        /// Dependency property <see cref="NotActiveImagePath"/>
        /// </summary>
        public static readonly DependencyProperty NotActiveImagePathProperty =
            DependencyProperty.Register("NotActiveImagePath", typeof(Uri), typeof(DrawingToolControl), new PropertyMetadata(null));

        #endregion Public Fields

        #region Public Properties

        /// <summary>
        /// Gets or sets the path to the image of the part
        /// of the tool that should be painted in the selected color
        /// </summary>
        public Uri ColorImagePath
        {
            get { return (Uri)GetValue(ColorImagePathProperty); }
            set { SetValue(ColorImagePathProperty, value); }
        }

        /// <summary>
        /// Gets or sets the current color of the drawing tool
        /// </summary>
        public Color DrawingToolColor
        {
            get { return (Color)GetValue(DrawingToolColorProperty); }
            set { SetValue(DrawingToolColorProperty, value); }
        }

        /// <summary>
        /// Gets or sets a boolean tool premium flag
        /// </summary>
        public bool IsPremium
        {
            get { return (bool)GetValue(IsPremiumProperty); }
            set { SetValue(IsPremiumProperty, value); }
        }

        /// <summary>
        /// Gets or sets tool selection flag
        /// </summary>
        public bool IsSelected
        {
            get { return (bool)GetValue(IsSelectedProperty); }
            set { SetValue(IsSelectedProperty, value); }
        }

        /// <summary>
        /// Gets or sets the path to the image when the tool is selected
        /// </summary>
        public Uri MainImagePath
        {
            get { return (Uri)GetValue(MainImagePathProperty); }
            set { SetValue(MainImagePathProperty, value); }
        }

        /// <summary>
        /// Gets or sets the path to the image when the tool is not selected
        /// </summary>
        public Uri NotActiveImagePath
        {
            get { return (Uri)GetValue(NotActiveImagePathProperty); }
            set { SetValue(NotActiveImagePathProperty, value); }
        }

        #endregion Public Properties

        #region Public Constructors

        /// <summary>
        /// Initializes an instance of a class <see cref="DrawingToolControl"/>
        /// </summary>
        public DrawingToolControl()
        {
            this.InitializeComponent();
        }

        #endregion Public Constructors
    }
}