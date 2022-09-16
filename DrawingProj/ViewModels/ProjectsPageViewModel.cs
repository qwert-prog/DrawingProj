using IoC;
using Microsoft.Graphics.Canvas;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using DrawingProj.Model;
using DrawingProj.View.Dialogs;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Utils.Helpers;
using Utils.Navigation;
using Utils.Navigation.Interfaces;
using Utils.Services;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Resources;
using Windows.Foundation.Collections;
using Windows.Storage;

namespace DrawingProj.ViewModels
{
    /// <summary>
    /// View model <see cref="ProjectsPage"/>
    /// </summary>
    internal class ProjectsPageViewModel : ObservableObject, INavigateAcceptor
    {
        #region Private Fields

        /// <summary>
        /// Maximum number of sketches
        /// </summary>
        private const int MAX_COUNT_SKETCH = 20;

        /// <summary>
        /// Class instance <see cref="NavigationService"/>.
        /// </summary>
        private readonly INavigationService _navigationService;

        /// <summary>
        /// Contains bool flag ability to create a sketch
        /// </summary>
        private bool _isPossibleCreateSketch;

        /// <summary>
        /// Contains Storage Folder sketches path
        /// </summary>
        private string _sketchesFolderPath;

        #endregion Private Fields

        #region Public Properties

        /// <summary>
        /// Gets or sets selelcted sketch
        /// </summary>
        public static Sketch SelectedSketch { get; set; }

        /// <summary>
        /// Sketch create command
        /// </summary>
        public ICommand CreateSketchAsyncCommand { get; private set; }

        /// <summary>
        /// Command to delete sketch
        /// </summary>
        public ICommand DeleteSketchAsyncCommand { get; private set; }

        /// <summary>
        /// Gets or sets bool flag ability to create a sketch
        /// </summary>
        public bool IsPossibleCreateSketch
        {
            get => _isPossibleCreateSketch;
            set
            {
                _isPossibleCreateSketch = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Command to open the delete dialog
        /// </summary>
        public ICommand OpenDeleteDialogAsyncCommand { get; private set; }

        /// <summary>
        /// Command to open sketch for work
        /// </summary>
        public ICommand OpenSketchCommand { get; private set; }

        /// <summary>
        /// Gets link to privacy policy
        /// </summary>
        public string PrivacyPolicyUriString { get => "https://motivapp.at/privacy"; }

        /// <summary>
        /// Command to share sketch
        /// </summary>
        public ICommand ShareSketchAsyncCommand { get; private set; }

        /// <summary>
        /// Gets or sets list created sketches
        /// </summary>
        public ObservableCollection<Sketch> SketchesList { get; set; }

        /// <summary>
        /// Gets text with application version number
        /// </summary>
        public string VersionText
        {
            get
            {
                PackageVersion version = Package.Current.Id.Version;
                var resourceLoader = ResourceLoader.GetForCurrentView();
                return string.Format(resourceLoader.GetString("Version") + " {0}.{1}.{2}", version.Major, version.Minor, version.Build);
            }
        }

        #endregion Public Properties

        #region Public Constructors

        /// <summary>
        /// Initializes an instance of a class <see cref="ProjectsPageViewModel"/>
        /// </summary>
        public ProjectsPageViewModel(NavigationService navigationService)
        {
            IsPossibleCreateSketch = true;
            _navigationService = navigationService;
            CreateSketchAsyncCommand = new AsyncRelayCommand(CreateSketchAsync);
            OpenSketchCommand = new RelayCommand(OpenSketch);
            DeleteSketchAsyncCommand = new AsyncRelayCommand(DeleteSketchAsync);
            ShareSketchAsyncCommand = new AsyncRelayCommand(ShareSketchAsync);
            OpenDeleteDialogAsyncCommand = new AsyncRelayCommand(OpenDeleteDialogAsync);
        }

        #endregion Public Constructors

        #region Public Methods

        /// <summary>
        /// Method for get sketch by name
        /// </summary>
        /// <param name="name">Name of the sketch</param>
        /// <returns></returns>
        public Sketch GetSketchByName(string name)
        {
            foreach (Sketch sketch in SketchesList)
            {
                if (sketch.DisplayName == name)
                {
                    return sketch;
                }
            }
            return null;
        }

        /// <inheritdoc/>
        public void Notify(INavigationArgs navigationArgs)
        {
            SelectedSketch = null;
        }

        /// <summary>
        /// Open sketch for work
        /// </summary>
        /// <param name="obj"></param>
        public void OpenSketch()
        {
            INavigationArgs navigationArgs = new NavigationArgs(SelectedSketch);
            SketchPageViewModel sketchPageVM = ServicesContainer.GetService<SketchPageViewModel>();
            _navigationService.NavigateTo(sketchPageVM, navigationArgs);
        }

        /// <summary>
        /// Method for save sketches list in local settings
        /// </summary>
        public async Task SetSketchesListInLocalFolder()
        {
            foreach (Sketch sketch in SketchesList)
            {
                foreach (Layer layer in sketch.LayersList)
                {
                    if (layer.CanvasRenderTarget != null)
                    {
                        try
                        {
                            await layer.CanvasRenderTarget.SaveAsync(layer.ImagePath);
                        }
                        catch { }
                    }
                }
            }
        }

        /// <summary>
        /// Method for changing the display name of a sketch
        /// </summary>
        /// <param name="newName"></param>
        /// <returns>True if the sketch has been renamed. Else false</returns>
        public async Task<bool> TryRenameSketch(string newName)
        {
            if (!LocalStorageFileHelper.ValidateFileName(newName))
            {
                return false;
            }
            if (GetSketchByName(newName) != null || newName == "")
            {
                return false;
            }
            else
            {
                await LocalStorageFileHelper.RenameSketchFileFromLocalStorageAsync(_sketchesFolderPath, SelectedSketch.DisplayName, SelectedSketch.Name, newName + Sketch.SKETCH_FILE_TYPE);
                SelectedSketch.DisplayName = newName;
                SelectedSketch.Name = newName + Sketch.SKETCH_FILE_TYPE;
                foreach (Layer layer in SelectedSketch.LayersList)
                {
                    layer.SketchName = SelectedSketch.DisplayName;
                }
                DeleteLocalSettingsByKey(SelectedSketch.DisplayName);
                AddCanvasPathBySketchNameInLocalSettings(newName, SelectedSketch.CanvasPath);
                return true;
            }
        }

        /// <summary>
        /// Method for writing the background path of sketches to the application's local settings
        /// </summary>
        public void WriteSketchesCanvasPathInLocalSettings()
        {
            ApplicationDataContainer localSettingsContainer = ApplicationData.Current.LocalSettings;
            IPropertySet settings = localSettingsContainer.Values;
            foreach (Sketch sketch in SketchesList)
            {
                settings[sketch.DisplayName] = sketch.CanvasPath;
            }
        }

        #endregion Public Methods

        #region Private Methods

        /// <summary>
        /// Adds an entry about the sketch and its background to the local settings
        /// </summary>
        /// <param name="settingsKey"></param>
        /// <param name="canvasPath"></param>
        private void AddCanvasPathBySketchNameInLocalSettings(string settingsKey, string canvasPath)
        {
            ApplicationDataContainer localSettingsContainer = ApplicationData.Current.LocalSettings;
            IPropertySet settings = localSettingsContainer.Values;
            settings[settingsKey] = canvasPath;
        }

        /// <summary>
        /// Create sketch
        /// </summary>
        /// <param name="obj"></param>
        private async Task CreateSketchAsync()
        {
            var resourceLoader = ResourceLoader.GetForCurrentView();
            string newSketchName = resourceLoader.GetString("Sketch");
            int countPostfix = 0;
            string equalsSketchName = newSketchName;
            while (GetSketchByName(equalsSketchName) != null)
            {
                countPostfix++;
                equalsSketchName = newSketchName + countPostfix;
            }

            Sketch newSketch = new Sketch()
            {
                Name = equalsSketchName + Sketch.SKETCH_FILE_TYPE,
                DisplayName = equalsSketchName
            };

            if (SketchesList.Count == MAX_COUNT_SKETCH)
            {
                IsPossibleCreateSketch = false;
                return;
            }

            StorageFolder sketchFolder = await LocalStorageFileHelper.CreateFolderFromLocalStorageAsync(_sketchesFolderPath, newSketch.DisplayName);
            await LocalStorageFileHelper.CreateFileFromLocalStorageAsync(sketchFolder.Path, newSketch.Name);
            await LocalStorageFileHelper.CreateFolderFromLocalStorageAsync(sketchFolder.Path, "Layers");

            await newSketch.CreateDefaultLayerAsync();
            SketchesList.Add(newSketch);

            INavigationArgs navigationArgs = new NavigationArgs(newSketch);
            SketchPageViewModel sketchPageVM = ServicesContainer.GetService<SketchPageViewModel>();
            _navigationService.NavigateTo(sketchPageVM, navigationArgs);
        }

        /// <summary>
        /// Removes local settings by key
        /// </summary>
        /// <param name="settingsKey"></param>
        private void DeleteLocalSettingsByKey(string settingsKey)
        {
            ApplicationDataContainer localSettingsContainer = ApplicationData.Current.LocalSettings;
            IPropertySet settings = localSettingsContainer.Values;
            settings.Remove(settingsKey);
        }

        /// <summary>
        /// Method for delete sketch
        /// </summary>
        /// <param name="obj"></param>
        private async Task DeleteSketchAsync()
        {
            await LocalStorageFileHelper.DeleteFolderFromLocalStorageAsync(_sketchesFolderPath, SelectedSketch.DisplayName);
            SketchesList.Remove(SelectedSketch);
        }

        /// <summary>
        /// Method for gets sketches list from local settings
        /// </summary>
        public async Task InitializeSketchesListAsync()
        {
            StorageFolder localFolder = ApplicationData.Current.LocalFolder;
            string sketchesFolderName = "Sketches";
            SketchesList = new ObservableCollection<Sketch>();
            SketchesList.CollectionChanged += SketchesList_CollectionChanged;
            if (await localFolder.TryGetItemAsync(sketchesFolderName) != null)
            {
                var sketchesFolder = await localFolder.GetFolderAsync(sketchesFolderName);
                _sketchesFolderPath = sketchesFolder.Path;
                foreach (StorageFolder sketchFolder in await sketchesFolder.GetFoldersAsync())
                {
                    StorageFile sketchFile = (await sketchFolder.GetFilesAsync()).First();
                    Sketch sketch = new Sketch()
                    {
                        DisplayName = sketchFile.DisplayName,
                        Name = sketchFile.Name,
                    };
                    object canvasPath = ReadSketcheCanvasPathInLocalSettings(sketchFile.DisplayName);
                    if (canvasPath != null)
                    {
                        sketch.CanvasPath = canvasPath.ToString();
                    }
                    await InitializeSketchLayersList(sketch, sketchFolder);
                    SketchesList.Add(sketch);
                }
            }
            else
            {
                var sketchesFolder = await localFolder.CreateFolderAsync(sketchesFolderName);
                _sketchesFolderPath = sketchesFolder.Path;
            }
        }

        /// <summary>
        /// Initialize the list of layers for the given sketch
        /// </summary>
        /// <param name="sketch">Current sketch</param>
        /// <param name="sketchFolder">StorageFolder sketch</param>
        /// <returns></returns>
        private async Task InitializeSketchLayersList(Sketch sketch, StorageFolder sketchFolder)
        {
            StorageFolder layersFolder = await sketchFolder.GetFolderAsync("Layers");
            foreach (StorageFile layersFile in await layersFolder.GetFilesAsync())
            {
                Layer layer = new Layer()
                {
                    Name = layersFile.Name,
                    DisplayName = layersFile.DisplayName,
                    SketchName = sketch.DisplayName
                };

                CanvasBitmap canvasBitmap;
                try
                {
                    canvasBitmap = await CanvasBitmap.LoadAsync(CanvasDevice.GetSharedDevice(), layer.ImagePath);
                }
                catch (Exception)
                {
                    sketch.LayersList.Add(layer);
                    continue;
                }
                layer.CanvasRenderTarget = new CanvasRenderTarget(canvasBitmap, canvasBitmap.Size);
                using (var ds = layer.CanvasRenderTarget.CreateDrawingSession())
                {
                    ds.DrawImage(canvasBitmap);
                }
                layer.UpdateImageSource();
                sketch.LayersList.Add(layer);
            }
        }

        /// <summary>
        /// Method for showing a dialog window
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        private async Task OpenDeleteDialogAsync()
        {
            DeleteSketchDialog deleteSketchDialog = new DeleteSketchDialog(DeleteSketchAsyncCommand);
            await deleteSketchDialog.ShowAsync();
        }

        /// <summary>
        /// Method for read the background path of sketches to the application's local settings
        /// </summary>
        /// <param name="key">Sketch name</param>
        /// <returns></returns>
        private object ReadSketcheCanvasPathInLocalSettings(string key)
        {
            ApplicationDataContainer localSettingsContainer = ApplicationData.Current.LocalSettings;
            IPropertySet settings = localSettingsContainer.Values;
            settings.TryGetValue(key, out object value);
            return value;
        }

        /// <summary>
        /// Method for share sketch file
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        private async Task ShareSketchAsync()
        {
            StorageFile file = await LocalStorageFileHelper.GetFileFromLocalStorageAsync(SelectedSketch.ImagePath);
            ShareDataService shareDataService = ShareDataService.GetInstance();
            shareDataService.ShareFile(file);
        }

        /// <summary>
        /// Handles the sketch list change event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SketchesList_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (SketchesList.Count != MAX_COUNT_SKETCH)
            {
                IsPossibleCreateSketch = true;
            }
        }

        #endregion Private Methods
    }
}