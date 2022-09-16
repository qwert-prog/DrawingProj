using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.UI;
using Microsoft.Graphics.Canvas.UI.Xaml;
using DrawingProj.Model;
using DrawingProj.UndoRedoOperations;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI;
using Windows.UI.Input;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;

namespace DrawingProj.Controllers
{
    /// <summary>
    /// Controller for drawing on CanvasControl
    /// </summary>
    internal class DrawingController
    {
        #region Private Fields

        /// <summary>
        /// Contains instance current layer
        /// </summary>
        private Layer _currentLayer;

        /// <summary>
        /// Contains instance current drawing tool
        /// </summary>
        private DrawingTool _drawingTool;

        /// <summary>
        /// Contains 2D grid with pixels of drawing tool image
        /// </summary>
        private CanvasBitmap _drawingToolFormCanvasBitmap;

        /// <summary>
        /// Contains the user's last operation
        /// </summary>
        private DrawLineOperation _lastOperation;

        /// <summary>
        /// Previous point to move
        /// </summary>
        private Point _previousPoint;

        /// <summary>
        /// Contains a boolean flag with information about the start of drawing
        /// </summary>
        private bool _isStartDraw = false;

        #endregion Private Fields

        #region Public Properties

        /// <summary>
        /// Gets or sets the CanvasControl instance on which user actions are rendered
        /// </summary>
        public CanvasControl CanvasControl { get; set; }

        /// <summary>
        /// Gets or sets instance current layer
        /// </summary>
        public Layer CurrentLayer
        {
            get => _currentLayer;
            set
            {
                _currentLayer = value;
                if (value != null)
                {
                    if (_currentLayer.CanvasRenderTarget == null)
                    {
                        _currentLayer.CanvasRenderTarget = new CanvasRenderTarget(CanvasControl, CanvasControl.Size);
                    }
                }
                CanvasControl.Invalidate();
            }
        }

        /// <summary>
        /// Gets or sets instance current drawing tool
        /// </summary>
        public DrawingTool DrawingTool
        {
            get => _drawingTool;
            set
            {
                _drawingTool = value;
                UpdateBrush();
            }
        }

        /// <summary>
        /// Boolean to tell if there was device content lost and <see cref="CanvasImageSource"/> instances are not recreated yet
        /// </summary>
        public bool SurfaceContentsLost { get; private set; }

        #endregion Public Properties

        #region Private Properties

        /// <summary>
        /// Gets or sets instance current sketch
        /// </summary>
        private Sketch CurrentSketch { get; set; }

        /// <summary>
        /// Undo/redo actions controller
        /// </summary>
        private UndoRedo UndoRedo { get; set; }

        #endregion Private Properties

        #region Public Events

        /// <summary>
        /// Event for updating the status of Undo/Redo buttons
        /// </summary>
        public event Action DrawnLine;

        #endregion Public Events

        #region Public Constructors

        /// <summary>
        /// Initializes an instance of a class <see cref="DrawingController"/>
        /// </summary>
        /// <param name="currentSketch"></param>
        /// <param name="undoRedo"></param>
        public DrawingController(Sketch currentSketch, UndoRedo undoRedo)
        {
            UndoRedo = undoRedo;
            CurrentSketch = currentSketch;

            CompositionTarget.SurfaceContentsLost -= CompositionTarget_SurfaceContentsLost;
            CompositionTarget.SurfaceContentsLost += CompositionTarget_SurfaceContentsLost;
        }

        #endregion Public Constructors

        #region Public Methods

        /// <summary>
        /// Handles the resource creation event for CanvasControl
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        public void CanvasControl_CreateResources(CanvasControl sender, CanvasCreateResourcesEventArgs args)
        {
            CanvasControl = sender;
            args.TrackAsyncAction(CreateBrushes(sender).AsAsyncAction());
        }

        /// <summary>
        /// Handles the draw event for CanvasControl
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        public void CanvasControl_Draw(CanvasControl sender, CanvasDrawEventArgs args)
        {
            foreach (Layer layer in CurrentSketch.LayersList)
            {
                if (!layer.IsVisible || layer.CanvasRenderTarget == null)
                    continue;

                var ds = args.DrawingSession;

                using (ds.CreateLayer(layer.Opasity / 100))
                {
                    ds.DrawImage(layer.CanvasRenderTarget);
                }
                layer.PixelBytes = layer.CanvasRenderTarget.GetPixelBytes();
            }
        }

        /// <summary>
        /// Handles the pointer moved event on the Grid with the CanvasControl
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void OnPointerMoved(object sender, PointerRoutedEventArgs e)
        {
            PointerPoint ptrPt = e.GetCurrentPoint(sender as Grid);
            if (!ptrPt.Properties.IsLeftButtonPressed)
                return;

            if (CurrentLayer == null || DrawingTool == null || CurrentLayer.IsLock)
                return;

            _lastOperation ??= new DrawLineOperation()
            {
                CurrentLayer = CurrentLayer,
                OldBytesArray = CurrentLayer.CanvasRenderTarget.GetPixelBytes()
            };

            DrawingLineFromTwoPoints(ptrPt.Position, _previousPoint);
            _previousPoint = ptrPt.Position;
        }

        /// <summary>
        /// Handles the pointer pressed event on the Grid with the CanvasControl
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void OnPointerPressed(object sender, PointerRoutedEventArgs e)
        {
            _isStartDraw = true;
            PointerPoint ptrPt = e.GetCurrentPoint(sender as Grid);
            if (!ptrPt.Properties.IsLeftButtonPressed)
                return;

            if (CurrentLayer == null || DrawingTool == null || CurrentLayer.IsLock)
                return;

            _lastOperation = new DrawLineOperation()
            {
                CurrentLayer = CurrentLayer,
                OldBytesArray = CurrentLayer.CanvasRenderTarget.GetPixelBytes()
            };
            _previousPoint = ptrPt.Position;
            DrawingLineFromTwoPoints(ptrPt.Position, _previousPoint);
        }

        /// <summary>
        /// Handles the pointer released event on the Grid with the CanvasControl
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void OnPointerReleased(object sender, PointerRoutedEventArgs e)
        {
            PointerPoint ptrPt = e.GetCurrentPoint(sender as Grid);
            if (ptrPt.Properties.PointerUpdateKind != PointerUpdateKind.LeftButtonReleased)
                return;

            if (CurrentLayer == null || DrawingTool == null || CurrentLayer.IsLock)
                return;
            if (_lastOperation != null)
            {
                _lastOperation.NewBytesArray = CurrentLayer.CanvasRenderTarget.GetPixelBytes();
                UndoRedo.AddOperationToUndoStack(_lastOperation);
                DrawnLine?.Invoke();
            }
            _lastOperation = null;
            try
            {
                CurrentLayer.UpdateImageSource();
            }
            catch
            {
            }
            CanvasControl.Invalidate();
            _isStartDraw = false;
        }

        /// <summary>
        /// Method for recreating the image with the negotiated parameters
        /// </summary>
        public async void UpdateBrush()
        {
            if (CanvasControl != null)
            {
                await CreateBrushes(CanvasControl);
            }
        }

        #endregion Public Methods

        #region Private Methods

        /// <summary>
        /// Метод, вычисляющий шаг, с которым мы идем
        /// по отрезку для определения точек, зависящий
        /// от заданного параметра
        /// </summary>
        /// <param name="currentPoint">Конечная точка отрезка</param>
        /// <param name="previousPoint">Начальная точка отрезка</param>
        /// <param name="param">Параметр частоты шага</param>
        /// <returns></returns>
        private double CalcStep(Point currentPoint, Point previousPoint, float param)
        {
            double step = 1;
            if (Math.Abs(previousPoint.X - currentPoint.X) < Math.Abs(previousPoint.Y - currentPoint.Y))
            {
                step = (currentPoint.Y - previousPoint.Y) / param;
            }
            else
            {
                step = (currentPoint.X - previousPoint.X) / param;
            }
            return step;
        }

        /// <summary>
        /// Method for changing color image of drawing tool
        /// </summary>
        /// <param name="targetBitmap"></param>
        /// <param name="currentcolor"></param>
        private void ChangeColorCanvasBitmap(CanvasBitmap targetBitmap, Color currentColor)
        {
            if (targetBitmap != null)
            {
                var colors = targetBitmap.GetPixelColors();

                List<Color> newColors = new List<Color>();

                foreach (var color in colors)
                {
                    if (color.Equals(Color.FromArgb(0, 0, 0, 0)))
                    {
                        newColors.Add(color);
                    }
                    else
                    {
                        newColors.Add(Color.FromArgb(255, currentColor.R, currentColor.G, currentColor.B));
                    }
                }

                targetBitmap.SetPixelColors(newColors.ToArray());
            }
        }

        /// <summary>
        /// Method invoked on device lost event.
        /// Device lost may be triggered, for example, on chnaging virtual desktops or unfocusing app for too long
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CompositionTarget_SurfaceContentsLost(object sender, object e)
        {
            SurfaceContentsLost = true;
            if (!Window.Current.Visible)
                return;

            foreach (var layer in CurrentSketch.LayersList)
            {
                if (SurfaceContentsLost)
                {
                    layer.RecreateImageSource();
                }
            }

            SurfaceContentsLost = false;
        }

        /// <summary>
        /// Method for initialize CanvasBitmap
        /// </summary>
        /// <param name="sender"></param>
        /// <returns></returns>
        private async Task CreateBrushes(CanvasControl sender)
        {
            if (DrawingTool != null)
            {
                if (DrawingTool.CurrentForm != null)
                {
                    _drawingToolFormCanvasBitmap = await CanvasBitmap.LoadAsync(sender, DrawingTool.CurrentForm);
                    ChangeColorCanvasBitmap(_drawingToolFormCanvasBitmap, DrawingTool.CurrentColor);
                }
                else
                {
                    _drawingToolFormCanvasBitmap = null;
                }
            }
        }

        /// <summary>
        /// Method for drawing form image of drawing tool
        /// </summary>
        /// <param name="point"></param>
        private void DrawForm(Point point, CanvasDrawingSession ds, CanvasActiveLayer layer)
        {
            if (!_isStartDraw)
            {
                return;
            }
            float toolFormSize = DrawingTool.Size + 1;
            float toolFormCenter = toolFormSize / 2;

            if (_drawingToolFormCanvasBitmap != null)
            {
                ds.DrawImage(_drawingToolFormCanvasBitmap, new Rect((float)point.X - toolFormCenter, (float)point.Y - toolFormCenter, toolFormSize, toolFormSize));
            }
            else
            {
                layer.Dispose();
                ds.Blend = CanvasBlend.Copy;
                ds.FillCircle(new Vector2((float)point.X - (toolFormCenter / 8), (float)point.Y - (toolFormCenter / 8)), toolFormCenter, Colors.Transparent);
            }
            CanvasControl.Invalidate();
        }

        /// <summary>
        /// Отрисовывает точки на линии
        /// </summary>
        /// <param name="currentPoint"></param>
        /// <param name="previousPoint"></param>
        /// <param name="step"></param>
        /// <param name="count"></param>
        /// <param name="isX"></param>
        /// <param name="ds"></param>
        /// <param name="layer"></param>
        private void DrawingFormsInLine(Point currentPoint, Point previousPoint, int count, bool isX, double step, CanvasDrawingSession ds, CanvasActiveLayer layer)
        {
            while (count > 0)
            {
                float newX, newY;
                if (isX)
                {
                    newX = (float)(previousPoint.X + step * count);
                    //Уравнение прямой через X
                    newY = (float)(-newX * (previousPoint.Y - currentPoint.Y) / (currentPoint.X - previousPoint.X) - (previousPoint.X * currentPoint.Y - currentPoint.X * previousPoint.Y) / (currentPoint.X - previousPoint.X));
                }
                else
                {
                    newY = (float)(previousPoint.Y + step * count);
                    //Уравнение прямой через Y
                    newX = (float)(-newY * (currentPoint.X - previousPoint.X) / (previousPoint.Y - currentPoint.Y) -
                        (previousPoint.X * currentPoint.Y - currentPoint.X * previousPoint.Y) / (previousPoint.Y - currentPoint.Y));
                }
                DrawForm(new Point(newX, newY), ds, layer);
                count--;
            }
        }

        /// <summary>
        /// Метод для получения всех точек отрезка с определенным шагом,
        /// проходящей через заданные точки и отрисовки на этих точках изображения мазка кисти.
        /// Точки получаются из уравнения прямой, проходящей через две заданные точки
        /// </summary>
        /// <param name="currentPoint">Конечная точка отрезка</param>
        /// <param name="previousPoint">Начальная точка отрезка</param>
        private void DrawingLineFromTwoPoints(Point currentPoint, Point previousPoint)
        {
            float toolFormSize = DrawingTool.Size + 1;
            //Шаг, с которым мы будем идти по отрезку, зависящий от размера кисти
            double step = CalcStep(currentPoint, previousPoint, toolFormSize * 40);
            bool isX = true;
            //Смотрим, по какой оси наибольшая разница между точками
            if (Math.Abs(previousPoint.X - currentPoint.X) < Math.Abs(previousPoint.Y - currentPoint.Y))
            {
                isX = false;
            }
            //Находим длину максимальной проекции по осям
            double length = Math.Max(Math.Abs(previousPoint.X - currentPoint.X), Math.Abs(previousPoint.Y - currentPoint.Y));
            //Высчитываем количество итераций отрисовки
            int count = (int)Math.Abs(length / step);
            using (var ds = CurrentLayer.CanvasRenderTarget.CreateDrawingSession())
            {
                float opacityDrawingTool = DrawingTool.Opacity / 100;
                CanvasActiveLayer layer = ds.CreateLayer(opacityDrawingTool);
                DrawingFormsInLine(currentPoint, previousPoint, count, isX, step, ds, layer);
                DrawForm(currentPoint, ds, layer);
                layer.Dispose();
            }
        }

        #endregion Private Methods
    }
}