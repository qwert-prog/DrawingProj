using DrawingProj.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Resources;
using Windows.Storage;

namespace DrawingProj.Services
{
    public class AssetsService
    {
        #region Public Fields

        /// <summary>
        /// Path to files with вкфцштп ещщды images
        /// </summary>
        public const string DRAWING_TOOLS_PATH = "Assets\\Brushes";

        #endregion Public Fields

        #region Private Fields

        /// <summary>
        /// Path to files with background images
        /// </summary>
        private const string BACKGROUNDS_PATH = "Assets\\Paper";

        #endregion Private Fields

        #region Public Properties

        /// <summary>
        /// List all backgrounds for sketches
        /// </summary>
        public List<CanvasViewModel> CanvasesList { get; set; }

        /// <summary>
        /// List of all images for each tool
        /// </summary>
        public Dictionary<string, List<string>> DrawingToolsDictionary { get; set; }

        #endregion Public Properties

        #region Public Events

        /// <summary>
        /// Method group, invoked on completing of assets initialization
        /// </summary>
        public event Action AssetsUpdated;

        #endregion Public Events

        #region Public Methods

        /// <summary>
        /// Initializes an instance of a class <see cref="ProjectsPageViewModel"/>
        /// </summary>
        /// <returns></returns>
        public async Task InitializeAsync()
        {
            await Task.WhenAll(LoadDrawingToolsAsync(), LoadBackgroundsAsync());
            AssetsUpdated?.Invoke();
        }

        #endregion Public Methods

        #region Private Methods

        /// <summary>
        /// Method for initialize <see cref="CanvasesList"/>
        /// </summary>
        /// <returns></returns>
        private async Task LoadBackgroundsAsync()
        {
            CanvasesList ??= new List<CanvasViewModel>();
            string root = Package.Current.InstalledLocation.Path;
            var resourceLoader = ResourceLoader.GetForCurrentView("Canvases");
            int countFreeCanvas = 2;
            StorageFolder backgroundsFolder = await StorageFolder.GetFolderFromPathAsync(Path.Combine(root, BACKGROUNDS_PATH));
            foreach (StorageFile file in await backgroundsFolder.GetFilesAsync())
            {
                string nameCanvas = resourceLoader.GetString(file.DisplayName);
                CanvasViewModel canvas = new CanvasViewModel
                {
                    CanvasPath = file.Path,
                    Name = nameCanvas,
                    IsPremium = false
                };
                if (countFreeCanvas > 0)
                {
                    canvas.IsPremium = false;
                    countFreeCanvas--;
                }

                CanvasesList.Add(canvas);
            }
        }

        /// <summary>
        /// Method for initialize <see cref="DrawingToolsDictionary"/>
        /// </summary>
        /// <returns></returns>
        private async Task LoadDrawingToolsAsync()
        {
            DrawingToolsDictionary ??= new Dictionary<string, List<string>>();
            string root = Package.Current.InstalledLocation.Path;
            StorageFolder brushesFolder = await StorageFolder.GetFolderFromPathAsync(Path.Combine(root, DRAWING_TOOLS_PATH));
            foreach (StorageFolder brushFolder in await brushesFolder.GetFoldersAsync())
            {
                List<string> brushImagesPathList = new List<string>();
                foreach (StorageFile brushFile in await brushFolder.GetFilesAsync())
                {
                    brushImagesPathList.Add(brushFile.Name);
                }
                if (DrawingToolsDictionary.ContainsKey(brushFolder.DisplayName) == false)
                {
                    DrawingToolsDictionary.Add(brushFolder.DisplayName, brushImagesPathList);
                }
            }
        }

        #endregion Private Methods
    }
}