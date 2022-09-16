namespace DrawingProj.UndoRedoOperations.Interfaces
{
    /// <summary>
    /// Base interfaces fro undo redo opeations
    /// </summary>
    public interface IUndoRedoOperation
    {
        #region Public Methods

        /// <summary>
        /// Redo operation
        /// </summary>
        public void Redo();

        /// <summary>
        /// Undo operation
        /// </summary>
        public void Undo();

        #endregion Public Methods
    }
}