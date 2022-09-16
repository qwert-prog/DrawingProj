using System;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Windows.UI.Xaml.Controls;

namespace Utils.Navigation.Interfaces
{
    /// <summary>
    /// Describes functionality of navigation service.
    /// </summary>
    public interface INavigationService
    {
        #region Public Events

        /// <summary>
        /// Occurs when navigation completed.
        /// </summary>
        event EventHandler Navigated;

        #endregion Public Events

        #region Public Methods

        /// <summary>
        /// Configure navigation service.
        /// </summary>
        /// <param name="frame"><see cref="Frame"/> for view.</param>
        /// <param name="router">Router of views.</param>
        void Configure(IRouter router, Frame frame);

        /// <summary>
        /// Executes navigation to selected page.
        /// </summary>
        /// <param name="viewModel">Concrete <see cref="BaseViewModel"/> that navigates to.</param>
        /// <param name="navigationArgs">Arguments that transferred due to navigation.</param>
        void NavigateTo(ObservableObject viewModel, INavigationArgs navigationArgs);

        /// <summary>
        /// Provide data to concrete view model.
        /// </summary>
        /// <param name="viewModel">Concrete <see cref="BaseViewModel"/> that navigates to.</param>
        /// <param name="navigationArgs">Arguments that transferred due to navigation.</param>
        void ProvideData(ObservableObject viewModel, INavigationArgs navigationArgs);

        #endregion Public Methods
    }
}