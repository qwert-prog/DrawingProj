using Microsoft.Toolkit.Mvvm.ComponentModel;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading.Tasks;
using Utils.Helpers;
using Windows.Storage;

namespace DrawingProj.Model
{
    /// <summary>
    /// Contains information about sketch object, created and edited by user
    /// </summary>
    internal class Sketch : ObservableObject
    {
        #region Public Fields

        /// <summary>
        /// File extension where the sketch will be saved
        /// </summary>
        public const string SKETCH_FILE_TYPE = ".png";

        #endregion Public Fields

        #region Private Fields

        /// <summary>
        /// Contains full path to the image canvas on which the user is drawing
        /// </summary>
        private string _canvasPath;

        /// <summary>
        /// Contains display name of sketch instance
        /// </summary>
        private string _displayName;

        #endregion Private Fields

        #region Public Properties

        /// <summary>
        /// Gets or sets the full path to the image canvas on which the user is drawing
        /// </summary>
        public string CanvasPath
        {
            get => _canvasPath;
            set
            {
                if (_canvasPath != value)
                {
                    _canvasPath = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Gets or sets display name of sketch instance
        /// </summary>
        public string DisplayName
        {
            get => _displayName;
            set
            {
                _displayName = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Gets full path to file with edited and saved image of sketch
        /// </summary>
        public string ImagePath
        {
            get
            {
                string sketchesFolderName = "Sketches";
                StorageFolder tempFolder = ApplicationData.Current.LocalFolder;
                return Path.Combine(tempFolder.Path, sketchesFolderName, DisplayName, Name);
            }
        }

        /// <summary>
        /// Gets or sets list layers
        /// </summary>
        public ObservableCollection<Layer> LayersList { get; set; } = new ObservableCollection<Layer>();

        /// <summary>
        /// Gets or sets full name of sketch instance
        /// </summary>
        public string Name { get; set; }

        #endregion Public Properties

        #region Public Methods

        /// <summary>
        /// Method for create default layer
        /// </summary>
        public async Task CreateDefaultLayerAsync()
        {
            Layer defaultLayer = new Layer()
            {
                DisplayName = "Layer",
                Name = "Layer" + Layer.LAYER_FILE_TYPE,
                SketchName = DisplayName
            };
            LayersList.Add(defaultLayer);
            StorageFolder localFolder = ApplicationData.Current.LocalFolder;
            StorageFolder sketchesFolder = await localFolder.GetFolderAsync("Sketches");
            StorageFolder sketchFolder = await sketchesFolder.GetFolderAsync(DisplayName);
            StorageFolder layersFolder = await sketchFolder.GetFolderAsync("Layers");
            await LocalStorageFileHelper.CreateFileFromLocalStorageAsync(layersFolder.Path, defaultLayer.Name);
        }

        #endregion Public Methods
    }
}