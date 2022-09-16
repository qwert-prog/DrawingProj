using IoC;
using Microsoft.Graphics.Canvas.UI;
using Microsoft.Graphics.Canvas.UI.Xaml;
using DrawingProj.Model;
using DrawingProj.Services;
using DrawingProj.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using Utils.Services;
using Windows.Foundation;
using Windows.UI.Input;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Navigation;

namespace DrawingProj.View
{
    /// <summary>
    /// Page for working with the sketch
    /// </summary>
    public sealed partial class SketchPage : Page
    {
        #region Private Fields

        /// <summary>
        /// Zoom In/Out Step Value
        /// </summary>
        private const float STEP_ZOOM_FACTOR_VALUE = 0.1f;

        /// <summary>
        /// Contains ZIndex for main UIElement
        /// </summary>
        private int _zIndexUIElement = 0;

        /// <summary>
        /// Contains visibility bool flag UIElements
        /// </summary>
        private bool _isElementVisible = true;

        /// <summary>
        /// Previous point to move
        /// </summary>
        private Point _previousPoint;

        /// <summary>
        /// Contains current value ZoomFactor
        /// </summary>
        private float _zoomValue;

        #endregion Private Fields

        #region Public Properties

        /// <summary>
        /// Gets or sets current value ZoomFactor
        /// </summary>
        public float ZoomValue
        {
            get => _zoomValue;
            set
            {
                if (Math.Abs(_zoomValue - value) >= float.Epsilon)
                {
                    _zoomValue = value;
                    Bindings.Update();
                }
            }
        }

        #endregion Public Properties

        #region Public Constructors

        /// <summary>
        /// Initializes an instance of a class <see cref="SketchPage"/>
        /// </summary>
        public SketchPage()
        {
            this.InitializeComponent();
            RenderUIElementService renderUIElement = ServicesContainer.GetService<RenderUIElementService>();
            renderUIElement.TargetElement = CanvasGrid;
            Loaded += SketchPage_Loaded;
        }

        #endregion Public Constructors

        #region Protected Methods

        /// <inheritdoc/>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
        }

        #endregion Protected Methods

        #region Private Methods

        /// <summary>
        /// Handles the resource creation event for CanvasControl
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void CanvasControl_CreateResources(CanvasControl sender, CanvasCreateResourcesEventArgs args)
        {
            ServiceLocator.SketchPageViewModel.DrawingController.CanvasControl_CreateResources(sender, args);
            DrawingToolListGridView.SelectedIndex = 0;
            SketchPageViewModel sketchPageViewModel = ServiceLocator.SketchPageViewModel;
            sketchPageViewModel.CurrentLayer = sketchPageViewModel.CurrentSketch.LayersList.First();
        }

        /// <summary>
        /// Handles the draw event for CanvasControl
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void CanvasControl_Draw(CanvasControl sender, CanvasDrawEventArgs args)
        {
            ServiceLocator.SketchPageViewModel.DrawingController.CanvasControl_Draw(sender, args);
        }

        /// <summary>
        /// Handles the pointer moved event on the Grid with the CanvasControl
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CanvasGrid_PointerMoved(object sender, PointerRoutedEventArgs e)
        {
            ServiceLocator.SketchPageViewModel.DrawingController.OnPointerMoved(sender, e);
        }

        /// <summary>
        /// Handles the pointer pressed event on the Grid with the CanvasControl
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CanvasGrid_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            ServiceLocator.SketchPageViewModel.DrawingController.OnPointerPressed(sender, e);
        }

        /// <summary>
        /// Handles the pointer released event on the Grid with the CanvasControl
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CanvasGrid_PointerReleased(object sender, PointerRoutedEventArgs e)
        {
            ServiceLocator.SketchPageViewModel.DrawingController.OnPointerReleased(sender, e);
        }

        /// <summary>
        /// Zooms scroll viewer with centering
        /// </summary>
        private void ChangeZoomFactor()
        {
            double ratio = (double)ZoomValue / (double)ImageScrollViewer.ZoomFactor;

            double calculatedViewportWidth = Math.Min(ImageScrollViewer.ViewportWidth, ImageScrollViewer.ExtentWidth) * ratio;
            double calculatedViewportHeight = Math.Min(ImageScrollViewer.ViewportHeight, ImageScrollViewer.ExtentHeight) * ratio;

            double insideHorizontalOffset = (calculatedViewportWidth - ImageScrollViewer.ViewportWidth) / 2d;
            double insideVerticalOffset = (calculatedViewportHeight - ImageScrollViewer.ViewportHeight) / 2d;

            double horizontalOffset = (ImageScrollViewer.HorizontalOffset * ratio) + insideHorizontalOffset;
            double verticalOffset = (ImageScrollViewer.VerticalOffset * ratio) + insideVerticalOffset;

            ImageScrollViewer.ChangeView(horizontalOffset, verticalOffset, ZoomValue, false);

            UpScaleButton.IsEnabled = ZoomValue < ImageScrollViewer.MaxZoomFactor;
            DownScaleButton.IsEnabled = ZoomValue > ImageScrollViewer.MinZoomFactor;
        }

        /// <summary>
        /// Handles the click event for the hide all elements button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CollapsedWindowsButton_Click(object sender, RoutedEventArgs e)
        {
            VisibleWindowsButton.Visibility = Visibility.Visible;
            CanvasesToggleButton.IsChecked = false;
            _isElementVisible = false;
            Bindings.Update();
            OpenToolsSettingsButton.IsChecked = false;
            OpenColorSettingsButton.IsChecked = false;
            LayersMenuToggleButton.IsChecked = false;
            CollapsedWindowsButton.Visibility = Visibility.Collapsed;
            UpScaleButton.Visibility = Visibility.Collapsed;
            DownScaleButton.Visibility = Visibility.Collapsed;
            ZoomFactorTextBloc.Visibility = Visibility.Collapsed;
            SeporatorRectangle.Visibility = Visibility.Collapsed;
            VisibleWindowsButton.Visibility = Visibility.Visible;
        }

        /// <summary>
        /// Handles the click event to zoom out
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DownScaleButton_Click(object sender, RoutedEventArgs e)
        {
            ZoomValue -= STEP_ZOOM_FACTOR_VALUE;
            ChangeZoomFactor();
        }

        /// <summary>
        /// Handles the change selected item
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DrawingToolListGridView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            CurrentToolSettings.Visibility = Visibility.Visible;
        }

        /// <summary>
        /// Handles the pointer moved event on the ScrollViewer
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ImageScrollViewer_PointerMoved(object sender, PointerRoutedEventArgs e)
        {
            PointerPoint ptrPt = e.GetCurrentPoint(sender as Grid);
            if (ptrPt.Properties.IsRightButtonPressed)
            {
                double horizontalOffset = ImageScrollViewer.HorizontalOffset - (ptrPt.Position.X - _previousPoint.X);
                double verticalOffset = ImageScrollViewer.VerticalOffset - (ptrPt.Position.Y - _previousPoint.Y);

                ImageScrollViewer.ChangeView(horizontalOffset, verticalOffset, null, true);
                _previousPoint = ptrPt.Position;
            }
        }

        /// <summary>
        /// Handles the pointer pressed event on the ScrollViewer
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ImageScrollViewer_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            PointerPoint ptrPt = e.GetCurrentPoint(sender as Grid);
            if (ptrPt.Properties.IsRightButtonPressed)
            {
                _previousPoint = ptrPt.Position;
            }
        }

        /// <summary>
        /// Method for handling the end of loading the page
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SketchPage_Loaded(object sender, RoutedEventArgs e)
        {
            SketchPageViewModel sketchPageViewModel = ServicesContainer.GetService<SketchPageViewModel>();
            ZoomValue = ImageScrollViewer.ZoomFactor;
            sketchPageViewModel.DrawingController.CanvasControl = CanvasControl;
            AssetsService assetsService = ServiceLocator.AssetsService;
            List<CanvasViewModel> canvasesList = assetsService.CanvasesList;
            Sketch currentSketch = sketchPageViewModel.CurrentSketch;
            if (currentSketch == null)
            {
                CanvasesListBox.SelectedIndex = 0;
                return;
            }
            if (currentSketch.CanvasPath != null)
            {
                CanvasViewModel item = canvasesList.Find(x => x.CanvasPath == currentSketch.CanvasPath);
                if (item == null)
                {
                    CanvasesListBox.SelectedIndex = 0;
                    return;
                }
                if (item.IsPremium)
                {
                    CanvasesListBox.SelectedIndex = 0;
                }
                else
                {
                    CanvasesListBox.SelectedIndex = canvasesList.IndexOf(item);
                }
            }
            else
            {
                CanvasesListBox.SelectedIndex = 0;
            }
        }

        /// <summary>
        /// Handles the click event to zoom in
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void UpScaleButton_Click(object sender, RoutedEventArgs e)
        {
            ZoomValue += STEP_ZOOM_FACTOR_VALUE;
            ChangeZoomFactor();
        }

        /// <summary>
        /// Handles the click event for the show all items button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void VisibleWindowsButton_Click(object sender, RoutedEventArgs e)
        {
            VisibleWindowsButton.Visibility = Visibility.Collapsed;
            _isElementVisible = true;
            Bindings.Update();
            OpenToolsSettingsButton.IsChecked = true;
            OpenColorSettingsButton.IsChecked = true;
            LayersMenuToggleButton.IsChecked = true;
            CollapsedWindowsButton.Visibility = Visibility.Visible;
            UpScaleButton.Visibility = Visibility.Visible;
            DownScaleButton.Visibility = Visibility.Visible;
            ZoomFactorTextBloc.Visibility = Visibility.Visible;
            SeporatorRectangle.Visibility = Visibility.Visible;
        }

        #endregion Private Methods

        private void UIElement_Tapped(object sender, TappedRoutedEventArgs e)
        {
            var element = (UIElement)sender;
            _zIndexUIElement++;
            Canvas.SetZIndex(element, _zIndexUIElement);
        }
    }
}