using DrawingProj.Model;
using DrawingProj.UndoRedoOperations.Interfaces;

namespace DrawingProj.UndoRedoOperations
{
    /// <summary>
    /// Line drawing operation
    /// </summary>
    public class DrawLineOperation : IUndoRedoOperation
    {
        #region Public Properties

        /// <summary>
        /// Curent layer for which the operation was applied
        /// </summary>
        public Layer CurrentLayer { get; set; }

        /// <summary>
        /// Gets or sets byte array after drawing the line
        /// </summary>
        public byte[] NewBytesArray { get; set; }

        /// <summary>
        /// Gets or sets byte array before drawing the line
        /// </summary>
        public byte[] OldBytesArray { get; set; }

        #endregion Public Properties

        #region Public Methods

        /// <summary>
        /// Sets the CanvasRenderTarget to an array of bytes after the line was drawn
        /// </summary>
        public void Redo()
        {
            CurrentLayer.CanvasRenderTarget.SetPixelBytes(NewBytesArray);
        }

        /// <summary>
        /// Sets the CanvasRenderTarget to an array of bytes before the line was drawn
        /// </summary>
        public void Undo()
        {
            CurrentLayer.CanvasRenderTarget.SetPixelBytes(OldBytesArray);
        }

        #endregion Public Methods
    }
}