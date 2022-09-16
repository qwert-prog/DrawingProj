using Microsoft.Toolkit.Mvvm.ComponentModel;
using DrawingProj.Controllers;
using DrawingProj.Services;
using System.Collections.Generic;
using System.Linq;
using Windows.UI;

namespace DrawingProj.Model
{
    /// <summary>
    /// A interface that implements the logic of the drawing tool
    /// </summary>
    public class DrawingTool : ObservableObject
    {
        #region Public Fields

        /// <summary>
        /// The maximum possible opacity value of the drawing tool
        /// </summary>
        public const int MAX_VALUE_OPACITY = 100;

        /// <summary>
        /// The maximum possible size value of the drawing tool
        /// </summary>
        public const int MAX_VALUE_SIZE = 100;

        /// <summary>
        /// The minimum possible opacity value of the drawing tool
        /// </summary>
        public const int MIN_VALUE_OPACITY = 0;

        /// <summary>
        /// The minimum possible size value of the drawing tool
        /// </summary>
        public const int MIN_VALUE_SIZE = 1;

        #endregion Public Fields

        #region Private Fields

        /// <summary>
        /// Contains current color selected drawing tool
        /// </summary>
        private Color _currentColor = Colors.White;

        /// <summary>
        /// Contains path to the image of shape the tool draws with
        /// </summary>
        private string _currentForm;

        /// <summary>
        /// Contains a boolean tool premium flag
        /// </summary>
        private bool _isPremium = true;

        /// <summary>
        /// Contains tool selection flag
        /// </summary>
        private bool _isSelected = false;

        /// <summary>
        /// Contains opacity value for the drawing tool
        /// </summary>
        private float _opacity = MAX_VALUE_OPACITY;

        /// <summary>
        /// Contains size value for the drawing tool
        /// </summary>
        private int _size = MIN_VALUE_SIZE;

        #endregion Private Fields

        #region Public Properties

        /// <summary>
        /// Gets or sets the path to the image of the part
        /// of the tool that should be painted in the selected color
        /// </summary>
        public string ColorImagePath { get; set; }

        /// <summary>
        /// Gets or sets current color selected drawing tool
        /// </summary>
        public Color CurrentColor
        {
            get => _currentColor;
            set
            {
                if (value != _currentColor)
                {
                    _currentColor = value;
                    OnPropertyChanged();
                    UpdateControllerTool();
                }
            }
        }

        /// <summary>
        /// Gets or sets path to the image of shape the tool draws with
        /// </summary>
        public string CurrentForm
        {
            get => _currentForm;
            set
            {
                if (_currentForm != value && value != null)
                {
                    _currentForm = value;
                    OnPropertyChanged();
                    UpdateControllerTool();
                }
            }
        }

        /// <summary>
        /// Gets or sets list of paths to the images of shapes the tool draws with
        /// </summary>
        public List<string> FormsImagePathList { get; set; }

        /// <summary>
        /// Gets or sets a boolean tool premium flag
        /// </summary>
        public bool IsPremium
        {
            get => _isPremium;
            set
            {
                _isPremium = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets tool selection flag
        /// </summary>
        public bool IsSelected
        {
            get => _isSelected;
            set
            {
                if (_isSelected != value)
                {
                    _isSelected = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Gets or sets the path to the active tool image
        /// </summary>
        public string MainImagePath { get; set; }

        /// <summary>
        /// Name of tool
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the path to the not active tool image
        /// </summary>
        public string NotActiveImagePath { get; set; }

        /// <summary>
        /// Gets or sets opacity value for the drawing tool
        /// </summary>
        public float Opacity
        {
            get => _opacity;
            set
            {
                if (_opacity != value)
                {
                    _opacity = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Gets or sets size value for the drawing tool
        /// </summary>
        public int Size
        {
            get => _size;
            set
            {
                if (_size != value)
                {
                    _size = value;
                    OnPropertyChanged();
                }
            }
        }

        #endregion Public Properties

        #region Private Methods

        /// <summary>
        /// Method for updating drawing tool of <see cref="DrawingController"/>
        /// </summary>
        private void UpdateControllerTool()
        {
            ServiceLocator.SketchPageViewModel.DrawingController?.UpdateBrush();
        }

        #endregion Private Methods
    }
}