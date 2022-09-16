using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.UI.Xaml;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using DrawingProj.Services;
using System.IO;
using Windows.Foundation;
using Windows.Storage;
using Windows.UI;

namespace DrawingProj.Model
{
    /// <summary>
    /// Layer implementation class
    /// </summary>
    public class Layer : ObservableObject
    {
        #region Public Fields

        /// <summary>
        /// File extension where the layer will be saved
        /// </summary>
        public const string LAYER_FILE_TYPE = ".png";

        #endregion Public Fields

        #region Private Fields

        /// <summary>
        /// Contains bitmap that can be drawn onto
        /// </summary>
        private CanvasRenderTarget _canvasRenderTarget;

        /// <summary>
        /// Contains display name of layer instance
        /// </summary>
        private string _displayName;

        /// <summary>
        /// Contains resource for displaying drawn lines on the layer preview
        /// </summary>
        private CanvasImageSource _imageSource;

        /// <summary>
        /// Contains bool flag that this layer is locked
        /// </summary>
        private bool _isLock = false;

        /// <summary>
        /// Contains bool flag that this layer is visible
        /// </summary>
        private bool _isVisible = true;

        /// <summary>
        /// Contains full name of layer instance
        /// </summary>
        private string _name;

        /// <summary>
        /// Contains value of layer opacity
        /// </summary>
        private float _opacity = 100;

        /// <summary>
        /// Contains byte array of the layer's image
        /// </summary>
        private byte[] _pixelBytes;

        #endregion Private Fields

        #region Public Properties

        /// <summary>
        /// Gets or sets bitmap that can be drawn onto
        /// </summary>
        public CanvasRenderTarget CanvasRenderTarget
        {
            get => _canvasRenderTarget;
            set
            {
                _canvasRenderTarget = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets display name of layer instance
        /// </summary>
        public string DisplayName
        {
            get => _displayName;
            set
            {
                if (_displayName != value)
                {
                    _displayName = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Gets full path to file with edited and saved image of layer
        /// </summary>
        public string ImagePath
        {
            get
            {
                string layersFolderName = "Layers";
                string sketchesFolderName = "Sketches";
                StorageFolder tempFolder = ApplicationData.Current.LocalFolder;
                return Path.Combine(tempFolder.Path, sketchesFolderName, SketchName, layersFolderName, Name);
            }
        }

        /// <summary>
        /// Gets or sets resource for displaying drawn lines on the layer preview
        /// </summary>
        public CanvasImageSource ImageSource
        {
            get => _imageSource;
            set
            {
                _imageSource = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets bool flag that this layer is locked
        /// </summary>
        public bool IsLock
        {
            get => _isLock;
            set
            {
                if (_isLock != value)
                {
                    _isLock = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Gets or sets bool flag that this layer is visible
        /// </summary>
        public bool IsVisible
        {
            get => _isVisible;
            set
            {
                if (_isVisible != value)
                {
                    _isVisible = value;
                    ServiceLocator.SketchPageViewModel.DrawingController?.CanvasControl.Invalidate();
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Gets or sets full name of layer instance
        /// </summary>
        public string Name
        {
            get => _name;
            set
            {
                _name = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets value opacity for layer
        /// </summary>
        public float Opasity
        {
            get => _opacity;
            set
            {
                if (_opacity != value)
                {
                    _opacity = value;
                    ServiceLocator.SketchPageViewModel.DrawingController?.CanvasControl.Invalidate();
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Gets or sets byte array of the layer's image
        /// </summary>
        public byte[] PixelBytes
        {
            get => _pixelBytes;
            set
            {
                _pixelBytes = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets the name of the sketch in which the layer is located
        /// </summary>
        public string SketchName { get; set; }

        #endregion Public Properties

        #region Public Methods

        /// <summary>
        /// Method for copy of the target layer
        /// </summary>
        /// <returns></returns>
        public Layer Copy()
        {
            Layer copiedLayer = new Layer()
            {
                DisplayName = this.DisplayName,
                IsVisible = this.IsVisible,
                IsLock = this.IsLock,
                Opasity = this.Opasity,
                SketchName = this.SketchName,
                PixelBytes = this.PixelBytes
            };
            CanvasRenderTarget canvasRenderTarget = new CanvasRenderTarget(this.CanvasRenderTarget, this.CanvasRenderTarget.Size);
            canvasRenderTarget.SetPixelBytes(CanvasRenderTarget.GetPixelBytes());
            copiedLayer.CanvasRenderTarget = canvasRenderTarget;
            return copiedLayer;
        }

        /// <summary>
        /// Method for recreate to image source
        /// </summary>
        public void RecreateImageSource()
        {
            if (this.CanvasRenderTarget == null)
                return;
            ImageSource ??= new CanvasImageSource(CanvasDevice.GetSharedDevice(), (float)CanvasRenderTarget.Size.Width, (float)CanvasRenderTarget.Size.Height, CanvasRenderTarget.Dpi);

            ImageSource.Recreate(CanvasDevice.GetSharedDevice());

            UpdateImageSource();
        }

        /// <summary>
        /// Method to update the image source
        /// </summary>
        public void UpdateImageSource()
        {
            if (this.CanvasRenderTarget == null)
            {
                return;
            }

            ImageSource ??= new CanvasImageSource(CanvasDevice.GetSharedDevice(), (float)CanvasRenderTarget.Size.Width, (float)CanvasRenderTarget.Size.Height, CanvasRenderTarget.Dpi);

            using var cisds = ImageSource.CreateDrawingSession(Colors.Transparent,
                                                                new Rect(CanvasRenderTarget.Bounds.X,
                                                                        CanvasRenderTarget.Bounds.Y,
                                                                        CanvasRenderTarget.Bounds.Width,
                                                                        CanvasRenderTarget.Bounds.Height));

            cisds.DrawImage(CanvasRenderTarget);
        }

        #endregion Public Methods
    }
}