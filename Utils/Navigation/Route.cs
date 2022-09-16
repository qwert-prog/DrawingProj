using Utils.Navigation.Interfaces;
using System;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Animation;
using Microsoft.Toolkit.Mvvm.ComponentModel;

namespace Utils.Navigation
{
    /// <summary>
    /// Represents route management.
    /// </summary>
    public class Route : IRoutable
    {
        #region Private Fields

        /// <summary>
        /// Provides view model.
        /// </summary>
        private readonly ObservableObject _viewModel;

        /// <summary>
        /// Provides view type.
        /// </summary>
        private readonly Type _viewType;

        #endregion Private Fields

        #region Public Constructors

        /// <summary>
        /// Creates an instance of <see cref="Route"/>.
        /// </summary>
        /// <param name="viewType"></param>
        /// <param name="viewModel"></param>
        public Route(Type viewType, ObservableObject viewModel)
        {
            _viewType = viewType;
            _viewModel = viewModel;
        }

        #endregion Public Constructors

        #region Public Methods

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="frame"><inheritdoc/></param>
        /// <param name="navigationArgs"><inheritdoc/></param>
        /// <returns><inheritdoc/></returns>
        public bool TryExecute(Frame frame, INavigationArgs navigationArgs)
        {
            bool result = frame.Navigate(_viewType, default, new SlideNavigationTransitionInfo());

            if (result)
            {
                Page page = frame.Content as Page;
                page.DataContext = _viewModel;

                if (_viewModel is INavigateAcceptor acceptor)
                {
                    acceptor.Notify(navigationArgs);
                }
            }

            return result;
        }

        #endregion Public Methods
    }
}