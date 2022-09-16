using IoC;
using Microsoft.Graphics.Canvas;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using DrawingProj.Controllers;
using DrawingProj.Model;
using DrawingProj.UndoRedoOperations;
using DrawingProj.View.Dialogs;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Utils.Helpers;
using Utils.Navigation;
using Utils.Navigation.Interfaces;
using Utils.Services;
using Windows.ApplicationModel.Resources;
using Windows.Storage;
using Windows.UI;

namespace DrawingProj.ViewModels
{
    /// <summary>
    /// View model <see cref="SketchPage"/>
    /// </summary>
    internal class SketchPageViewModel : ObservableObject, INavigateAcceptor
    {
        #region Private Fields

        /// <summary>
        /// Contains current canvas
        /// </summary>
        private CanvasViewModel _currentCanvasPair;

        /// <summary>
        /// Contains the currently selected drawing tool
        /// </summary>
        private DrawingTool _currentDrawingTool;

        /// <summary>
        /// Contains current layer
        /// </summary>
        private Layer _currentLayer;

        /// <summary>
        /// Contains instance controller for drawing
        /// </summary>
        private DrawingController _drawingController;

        /// <summary>
        /// Contains a folder for storing images from layers
        /// </summary>
        private string _layersFolderPath;

        /// <summary>
        /// Class instance <see cref="NavigationService"/>.
        /// </summary>
        private NavigationService _navigationService;

        #endregion Private Fields

        #region Public Properties

        /// <summary>
        /// Command to add layer in layers list
        /// </summary>
        public ICommand AddLayerAsyncCommand { get; private set; }

        /// <summary>
        /// Command to return to the page with projects
        /// </summary>
        public ICommand BackToProjectsPageAsyncCommand { get; private set; }

        /// <summary>
        /// Boolean to link RedoButton to
        /// </summary>
        public bool CanRedo => UndoRedo.HasRedoOperations();

        /// <summary>
        /// Boolean to link UndoButton to
        /// </summary>
        public bool CanUndo => UndoRedo.HasUndoOperations();

        /// <summary>
        /// Current canvas in format KeyValuePair.
        /// Key - name canvas. Value - path to the image canvas
        /// </summary>
        public CanvasViewModel CurrentCanvas
        {
            get => _currentCanvasPair;
            set
            {
                _currentCanvasPair = value;
                if (value != null)
                {
                    CurrentSketch.CanvasPath = value.CanvasPath;
                }
            }
        }

        /// <summary>
        /// Gets or sets the currently selected drawing tool
        /// </summary>
        public DrawingTool CurrentDrawingTool
        {
            get => _currentDrawingTool;
            set
            {
                _currentDrawingTool = value;
                _drawingController.DrawingTool = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets current layer
        /// </summary>
        public Layer CurrentLayer
        {
            get => _currentLayer;
            set
            {
                _drawingController.CurrentLayer = value;
                _currentLayer = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets current sketch for work
        /// </summary>
        public Sketch CurrentSketch { get; set; }

        /// <summary>
        /// Command to delete layer in layers list
        /// </summary>
        public ICommand DeleteLayerAsyncCommand { get; private set; }

        /// <summary>
        /// Gets or sets instance controller for drawing
        /// </summary>
        public DrawingController DrawingController
        {
            get => _drawingController;
            private set
            {
                _drawingController = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Command to dublicate current layer and add his to the list layers
        /// </summary>
        public ICommand DuplicateCurrentLayerAsyncCommand { get; private set; }

        /// <summary>
        /// Command to merge all layers
        /// </summary>
        public ICommand MergeAllLayersAsyncCommand { get; private set; }

        /// <summary>
        /// Command to merge a layer with the previous one
        /// </summary>
        public ICommand MergePreviousLayerAsyncCommand { get; private set; }

        /// <summary>
        /// Command to open the reset dialog
        /// </summary>
        public ICommand OpenResetDialogAsyncCommand { get; private set; }

        /// <summary>
        /// Gets or sets previous drawing tool
        /// </summary>
        public DrawingTool PreviousDrawingTool { get; set; }

        /// <summary>
        /// Gets or sets previous drawing tool
        /// </summary>
        public CanvasViewModel PreviousCanvas { get; set; }

        /// <summary>
        /// Command to redo last action
        /// </summary>
        public ICommand RedoCommand { get; private set; }

        /// <summary>
        /// Command to reset all actions
        /// </summary>
        public ICommand ResetCommand { get; private set; }

        /// <summary>
        /// Command for processing the selection of a drawing tool
        /// </summary>
        public ICommand SelectToolCommand { get; private set; }

        /// <summary>
        /// Command for processing the selection of a canvas
        /// </summary>
        public ICommand SelectCanvasCommand { get; private set; }

        /// <summary>
        /// Command to share current sketch file
        /// </summary>
        public ICommand ShareSketchAsyncCommand { get; private set; }

        /// <summary>
        /// Command to undo last action
        /// </summary>
        public ICommand UndoCommand { get; private set; }

        #endregion Public Properties

        #region Private Properties

        /// <summary>
        /// Undo/redo actions controllers
        /// </summary>
        private UndoRedo UndoRedo { get; } = new UndoRedo();

        #endregion Private Properties

        #region Public Constructors

        /// <summary>
        /// Initializes an instance of a class <see cref="SketchPageViewModel"/>
        /// </summary>
        /// <param name="navigationService"></param>
        public SketchPageViewModel(NavigationService navigationService)
        {
            _navigationService = navigationService;
            SelectToolCommand = new RelayCommand(SelectTool);
            SelectCanvasCommand = new RelayCommand(SelectCanvas);
            BackToProjectsPageAsyncCommand = new AsyncRelayCommand(BackToProjectsPageAsync);
            ShareSketchAsyncCommand = new AsyncRelayCommand(ShareSketchAsync);
            AddLayerAsyncCommand = new AsyncRelayCommand(AddLayerAsync);
            DeleteLayerAsyncCommand = new AsyncRelayCommand(DeleteLayerAsync);
            DuplicateCurrentLayerAsyncCommand = new AsyncRelayCommand(DuplicateCurrentLayerAsync);
            MergePreviousLayerAsyncCommand = new AsyncRelayCommand(MergePreviousLayerAsync);
            MergeAllLayersAsyncCommand = new AsyncRelayCommand(MergeAllLayersAsync);
            UndoCommand = new RelayCommand(Undo);
            RedoCommand = new RelayCommand(Redo);
            ResetCommand = new RelayCommand(Reset);
            OpenResetDialogAsyncCommand = new AsyncRelayCommand(OpenResetDialogAsync);
        }

        #endregion Public Constructors

        #region Public Methods

        /// <summary>
        /// Method for get layer by name
        /// </summary>
        /// <param name="name">Name of the sketch</param>
        /// <returns></returns>
        public Layer GetLayerByName(string name)
        {
            foreach (Layer sketch in CurrentSketch.LayersList)
            {
                if (sketch.DisplayName == name)
                {
                    return sketch;
                }
            }
            return null;
        }

        /// <inheritdoc/>
        public async void Notify(INavigationArgs navigationArgs)
        {
            if (navigationArgs.TryNavigationDataCast(out Sketch sketch))
            {
                CurrentSketch = sketch;
                OnPropertyChanged(nameof(CurrentSketch));
            }
            DrawingController = new DrawingController(CurrentSketch, UndoRedo);
            DrawingController.DrawnLine += UpdateButtonState;
            await InitFolderAsync();
        }

        /// <summary>
        /// Method for changing the display name of a sketch
        /// </summary>
        /// <param name="newName"></param>
        /// <returns>True if the sketch has been renamed. Else false</returns>
        public async Task<bool> TryRenameLayer(string newName)
        {
            if (!LocalStorageFileHelper.ValidateFileName(newName))
            {
                return false;
            }
            if (GetLayerByName(newName) != null || newName == "")
            {
                return false;
            }
            else
            {
                await LocalStorageFileHelper.RenameStorageFileFromFolder(_layersFolderPath, CurrentLayer.Name, newName + Layer.LAYER_FILE_TYPE);
                CurrentLayer.DisplayName = newName;
                CurrentLayer.Name = newName + Layer.LAYER_FILE_TYPE;
                return true;
            }
        }

        #endregion Public Methods

        #region Private Methods

        /// <summary>
        /// Method for adding new layer in the list layers
        /// </summary>
        private async Task AddLayerAsync()
        {
            var resourceLoader = ResourceLoader.GetForCurrentView();
            string nameLayer = resourceLoader.GetString("Layer");
            string equalsLayerName = nameLayer;
            int countPostfix = 0;

            while (GetLayerByName(equalsLayerName) != null)
            {
                countPostfix++;
                equalsLayerName = nameLayer + countPostfix;
            }
            Layer newlayer = new Layer()
            {
                DisplayName = equalsLayerName,
                Name = equalsLayerName + Layer.LAYER_FILE_TYPE,
                SketchName = CurrentSketch.DisplayName
            };
            await LocalStorageFileHelper.CreateFileFromLocalStorageAsync(_layersFolderPath, newlayer.Name);
            CurrentSketch.LayersList.Add(newlayer);
        }

        /// <summary>
        /// Method for returning to the page with projects
        /// </summary>
        private async Task BackToProjectsPageAsync()
        {
            await LocalStorageFileHelper.SaveSketch(CurrentSketch.ImagePath);
            CurrentDrawingTool = null;
            PreviousDrawingTool = null;
            CurrentSketch = null;
            CurrentLayer = null;
            CurrentCanvas = null;
            UndoRedo.ClearUndoRedoStack();
            ProjectsPageViewModel sketchPageVM = ServicesContainer.GetService<ProjectsPageViewModel>();
            _navigationService.NavigateTo(sketchPageVM, default);
        }

        /// <summary>
        /// Method for deleting current layer
        /// </summary>
        private async Task DeleteLayerAsync()
        {
            ObservableCollection<Layer> layersList = CurrentSketch.LayersList;
            if (layersList.Count != 1)
            {
                await LocalStorageFileHelper.DeleteFileFromLocalStorageAsync(_layersFolderPath, CurrentLayer.Name);
                layersList.Remove(CurrentLayer);
            }
            DrawingController.CanvasControl.Invalidate();
        }

        /// <summary>
        /// Method for dublicate current layer and add his to the list layers
        /// </summary>
        private async Task DuplicateCurrentLayerAsync()
        {
            StorageFolder folder = await StorageFolder.GetFolderFromPathAsync(_layersFolderPath);
            var file = await folder.CreateFileAsync(CurrentLayer.Name, CreationCollisionOption.GenerateUniqueName);
            Layer newLayer = CurrentLayer.Copy();
            newLayer.Name = file.Name;
            newLayer.DisplayName = file.DisplayName;
            CurrentSketch.LayersList.Add(newLayer);
            newLayer.UpdateImageSource();
            DrawingController.CanvasControl.Invalidate();
        }

        /// <summary>
        /// Methods for initialize layers folder
        /// </summary>
        private async Task InitFolderAsync()
        {
            string sketchesFolderName = "Sketches";
            StorageFolder localFolder = ApplicationData.Current.LocalFolder;
            StorageFolder sketchesFolder = await localFolder.GetFolderAsync(sketchesFolderName);
            StorageFolder sketchFolder = await sketchesFolder.GetFolderAsync(CurrentSketch.DisplayName);
            StorageFolder layersFolder = await sketchFolder.GetFolderAsync("Layers");
            _layersFolderPath = layersFolder.Path;
        }

        /// <summary>
        /// Method for merge all layers
        /// </summary>
        private async Task MergeAllLayersAsync()
        {
            var resourceLoader = ResourceLoader.GetForCurrentView();
            ObservableCollection<Layer> layers = CurrentSketch.LayersList;
            Layer newlayer = new Layer()
            {
                DisplayName = resourceLoader.GetString("Layer"),
                CanvasRenderTarget = new CanvasRenderTarget(DrawingController.CanvasControl, DrawingController.CanvasControl.Size),
                Name = resourceLoader.GetString("Layer") + Layer.LAYER_FILE_TYPE,
                SketchName = CurrentSketch.DisplayName
            };
            foreach (Layer layer in layers)
            {
                if (layer.CanvasRenderTarget != null)
                {
                    using (var ds = newlayer.CanvasRenderTarget.CreateDrawingSession())
                    {
                        using (ds.CreateLayer(layer.Opasity / 100))
                        {
                            ds.DrawImage(layer.CanvasRenderTarget);
                        }
                    }
                }
                await LocalStorageFileHelper.DeleteFileFromLocalStorageAsync(_layersFolderPath, layer.Name);
            }
            layers.Clear();
            await LocalStorageFileHelper.CreateFileFromLocalStorageAsync(_layersFolderPath, newlayer.Name);
            layers.Add(newlayer);
            newlayer.UpdateImageSource();
            UndoRedo.ClearUndoRedoStack();
            UpdateButtonState();
            DrawingController.CanvasControl.Invalidate();
        }

        /// <summary>
        /// Method for merging the current layer with the previous one
        /// </summary>
        private async Task MergePreviousLayerAsync()
        {
            if (CurrentLayer == CurrentSketch.LayersList.First())
            {
                return;
            }
            int indexCurrentLayer = CurrentSketch.LayersList.IndexOf(CurrentLayer);
            Layer previousLayer = CurrentSketch.LayersList[indexCurrentLayer - 1];
            if (previousLayer.CanvasRenderTarget != null)
            {
                CanvasRenderTarget canvasRenderTarget = new CanvasRenderTarget(CurrentLayer.CanvasRenderTarget, CurrentLayer.CanvasRenderTarget.Size);
                using (var ds = canvasRenderTarget.CreateDrawingSession())
                {
                    using (ds.CreateLayer(previousLayer.Opasity / 100))
                    {
                        ds.DrawImage(previousLayer.CanvasRenderTarget);
                    }

                    ds.DrawImage(CurrentLayer.CanvasRenderTarget);
                }
                CurrentLayer.CanvasRenderTarget.SetPixelBytes(canvasRenderTarget.GetPixelBytes());
            }
            CurrentLayer.UpdateImageSource();
            await LocalStorageFileHelper.DeleteFileFromLocalStorageAsync(_layersFolderPath, previousLayer.Name);
            CurrentSketch.LayersList.Remove(previousLayer);
            UndoRedo.ClearUndoRedoStack();
            UpdateButtonState();
            DrawingController.CanvasControl.Invalidate();
        }

        /// <summary>
        /// Method for showing a dialog window
        /// </summary>
        /// <returns></returns>
        private async Task OpenResetDialogAsync()
        {
            ResetAllDialog resetAllDialog = new ResetAllDialog(ResetCommand);
            await resetAllDialog.ShowAsync();
        }

        /// <summary>
        /// Method for redo last action
        /// </summary>
        private void Redo()
        {
            UndoRedo.Redo();
            UpdateButtonState();
            CurrentLayer.UpdateImageSource();
            DrawingController.CanvasControl.Invalidate();
        }

        /// <summary>
        /// Method for reset all actions
        /// </summary>
        private void Reset()
        {
            foreach (Layer layer in CurrentSketch.LayersList)
            {
                if (layer.CanvasRenderTarget != null)
                {
                    using (CanvasDrawingSession ds = layer.CanvasRenderTarget.CreateDrawingSession())
                    {
                        ds.Clear(Colors.Transparent);
                    }
                    layer.UpdateImageSource();
                }
            }
            UndoRedo.ClearUndoRedoStack();
            UpdateButtonState();
            DrawingController.CanvasControl.Invalidate();
        }

        /// <summary>
        /// Method for selecting the current drawing tool
        /// </summary>
        private void SelectTool()
        {
            if (PreviousDrawingTool != null)
            {
                PreviousDrawingTool.IsSelected = false;
            }
            if (CurrentDrawingTool != null)
            {
                CurrentDrawingTool.IsSelected = true;
                Dictionary<string, object> parameters = new Dictionary<string, object>{
                { "Name", CurrentDrawingTool.Name }
                };
            }
            if (CurrentDrawingTool != null && CurrentDrawingTool.IsPremium)
            {
                CurrentDrawingTool.IsSelected = false;
                CurrentDrawingTool = PreviousDrawingTool;
            }

            OnPropertyChanged(nameof(CurrentDrawingTool));
            PreviousDrawingTool = CurrentDrawingTool;
        }

        /// <summary>
        /// Method for selecting the current canvas
        /// </summary>
        private void SelectCanvas()
        {
            if (CurrentCanvas.IsPremium)
            {
                CurrentCanvas = PreviousCanvas;
            }
            Dictionary<string, object> parameters = new Dictionary<string, object>{
                { "Name", CurrentCanvas.Name }
            };
            OnPropertyChanged(nameof(CurrentCanvas));
            PreviousCanvas = CurrentCanvas;
        }

        /// <summary>
        /// Method for share current sketch
        /// </summary>
        /// <returns></returns>
        private async Task ShareSketchAsync()
        {
            await LocalStorageFileHelper.SaveSketch(CurrentSketch.ImagePath);
            StorageFile file = await LocalStorageFileHelper.GetFileFromLocalStorageAsync(CurrentSketch.ImagePath);
            ShareDataService shareDataService = ShareDataService.GetInstance();
            shareDataService.ShareFile(file);
        }

        /// <summary>
        /// Method for undo last action
        /// </summary>
        private void Undo()
        {
            UndoRedo.Undo();
            UpdateButtonState();
            CurrentLayer.UpdateImageSource();
            DrawingController.CanvasControl.Invalidate();
        }

        /// <summary>
        /// Method for update button status
        /// </summary>
        private void UpdateButtonState()
        {
            OnPropertyChanged(nameof(CanRedo));
            OnPropertyChanged(nameof(CanUndo));
        }

        #endregion Private Methods
    }
}