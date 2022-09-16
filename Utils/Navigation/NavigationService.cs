using Utils.Navigation.Interfaces;
using System;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Windows.UI.Xaml.Controls;

namespace Utils.Navigation
{
    /// <summary>
    /// Represents functionality of service navigation management.
    /// </summary>
    public class NavigationService : INavigationService
    {
        #region Private Fields

        /// <summary>
        /// Represents an instance of <see cref="IRouter"/>.
        /// </summary>
        private IRouter _router;

        /// <summary>
        /// Represents <see cref="Frame"/> in navigation context.
        /// </summary>
        private Frame _navigationFrame;

        #endregion Private Fields

        #region Public Events

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public event EventHandler Navigated;

        #endregion Public Events

        #region Public Methods

        /// <summary>
        /// Configure an instance of <see cref="NavigationService"/>.
        /// </summary>
        /// <param name="router">Router <see cref="IRouter"/>.</param>
        /// <param name="frame"></param>
        public void Configure(IRouter router, Frame frame)
        {
            _router = router;
            _navigationFrame = frame;
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="navigationArgs"><inheritdoc/></param>
        /// <param name="viewModel"><inheritdoc/></param>
        public void NavigateTo(ObservableObject viewModel, INavigationArgs navigationArgs)
        {
            IRoutable route = _router.TryGetRoute(viewModel);
            bool isNavigationExecuted = route.TryExecute(_navigationFrame, navigationArgs);

            if (isNavigationExecuted)
            {
                Navigated?.Invoke(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="viewModel"><inheritdoc/></param>
        /// <param name="navigationArgs"><inheritdoc/></param>
        public void ProvideData(ObservableObject viewModel, INavigationArgs navigationArgs)
        {
            if (viewModel is INavigateAcceptor acceptor)
            {
                acceptor.Notify(navigationArgs);
            }
        }

        #endregion Public Methods
    }
}