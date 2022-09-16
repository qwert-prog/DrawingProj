using System.Windows.Input;
using Windows.UI.Xaml.Controls;

namespace DrawingProj.View.Dialogs
{
    /// <summary>
    /// Dialog box for confirming or canceling the sketch deletion operation
    /// </summary>
    public sealed partial class DeleteSketchDialog : ContentDialog
    {
        #region Private Fields

        /// <summary>
        /// Command to remove the current sketch
        /// </summary>
        private ICommand _deleteSketchCommand;

        #endregion Private Fields

        #region Public Constructors

        /// <summary>
        /// Initializes an instance of a class <see cref="DeleteSketchDialog"/>
        /// </summary>
        /// <param name="deleteCommand"></param>
        public DeleteSketchDialog(ICommand deleteCommand)
        {
            _deleteSketchCommand = deleteCommand;
            this.InitializeComponent();
        }

        #endregion Public Constructors

        #region Private Methods

        /// <summary>
        /// Close dialog window
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CloseDialog(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            Hide();
        }

        #endregion Private Methods
    }
}