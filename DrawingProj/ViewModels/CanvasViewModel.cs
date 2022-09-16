using Microsoft.Toolkit.Mvvm.ComponentModel;
using System.Linq;

namespace DrawingProj.ViewModels
{
    /// <summary>
    /// ViewModel for display canvas
    /// </summary>
    public class CanvasViewModel : ObservableObject
    {
        #region Private Fields

        /// <summary>
        /// Contains bool flag of canvas
        /// </summary>
        private bool _isPremium = false;

        #endregion Private Fields

        #region Public Properties

        /// <summary>
        /// Gets or sets path to image file
        /// </summary>
        public string CanvasPath { get; set; }

        /// <summary>
        /// Gets or sets bool flag of canvas
        /// </summary>
        public bool IsPremium
        {
            get => _isPremium;
            set
            {
                _isPremium = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets display name of canvas
        /// </summary>
        public string Name { get; set; }

        #endregion Public Properties
    }
}