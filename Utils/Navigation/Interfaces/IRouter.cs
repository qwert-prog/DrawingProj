using Microsoft.Toolkit.Mvvm.ComponentModel;
using Windows.UI.Xaml.Controls;

namespace Utils.Navigation.Interfaces
{
    /// <summary>
    /// Provides registry and build navigation route.
    /// </summary>
    public interface IRouter
    {
        /// <summary>
        /// Creates route build for current view model.
        /// </summary>
        /// <typeparam name="TViewModel">Requested view model.</typeparam>
        /// <returns>Route.</returns>
        IRoutable TryGetRoute(ObservableObject viewModel);

        /// <summary>
        /// Creates registry of route.
        /// </summary>
        /// <typeparam name="TView">View type.</typeparam>
        /// <param name="viewModel">View model type.</param>
        /// <param name="isViewCashed">Flag that shows necessary of cashed view.</param>
        void RegisterRoute<TView>(ObservableObject viewModel) where TView : Page, new();
    }
}