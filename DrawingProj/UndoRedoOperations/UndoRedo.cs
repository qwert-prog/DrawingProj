using DrawingProj.UndoRedoOperations.Interfaces;
using System.Collections.Generic;

namespace DrawingProj.UndoRedoOperations
{
    /// <summary>
    /// Controller class for undo/redo action
    /// </summary>
    public class UndoRedo
    {
        #region Private Fields

        /// <summary>
        ///  Maximum size of stack with operation made by user
        /// </summary>
        private const int MAX_UNDO_STACK_SIZE = 40;

        #endregion Private Fields

        #region Private Properties

        /// <summary>
        /// Undone operations stack
        /// </summary>
        private LinkedList<IUndoRedoOperation> RedoStack { get; } = new LinkedList<IUndoRedoOperation>();

        /// <summary>
        /// Done operations stack
        /// </summary>
        private LinkedList<IUndoRedoOperation> UndoStack { get; } = new LinkedList<IUndoRedoOperation>();

        #endregion Private Properties

        #region Public Methods

        /// <summary>
        /// Adds specified operation to Undo stack
        /// </summary>
        /// <param name="operation"></param>
        public void AddOperationToUndoStack(IUndoRedoOperation operation)
        {
            RedoStack.Clear();
            if (UndoStack.Count >= MAX_UNDO_STACK_SIZE)
            {
                UndoStack.RemoveFirst();
            }

            UndoStack.AddLast(operation);
        }

        /// <summary>
        /// Clears Redo stack
        /// </summary>
        public void ClearUndoRedoStack()
        {
            UndoStack.Clear();
            RedoStack.Clear();
        }

        /// <summary>
        /// Boolean to tell if there are operations to redo
        /// </summary>
        /// <returns></returns>
        public bool HasRedoOperations() => RedoStack.Count != 0;

        /// <summary>
        /// Boolean to tell if there are operations to undo
        /// </summary>
        /// <returns></returns>
        public bool HasUndoOperations() => UndoStack.Count != 0;

        /// <summary>
        /// Performs last undone transaction ad puts it into Undo stack
        /// </summary>
        public void Redo()
        {
            if (RedoStack.Count == 0)
            {
                return;
            }
            var operationNode = RedoStack.Last;
            var currentOperation = operationNode.Value;
            currentOperation.Redo();

            UndoStack.AddLast(currentOperation);
            RedoStack.RemoveLast();
        }

        /// <summary>
        /// Rolls back last done transaction ad puts it into Redo stack
        /// </summary>
        public void Undo()
        {
            if (UndoStack.Count == 0)
            {
                return;
            }
            var operationNode = UndoStack.Last;
            var currentOperation = operationNode.Value;
            currentOperation.Undo();

            RedoStack.AddLast(currentOperation);
            UndoStack.RemoveLast();
        }

        #endregion Public Methods
    }
}