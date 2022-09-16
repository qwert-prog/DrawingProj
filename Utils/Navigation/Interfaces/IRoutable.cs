using Windows.UI.Xaml.Controls;

namespace Utils.Navigation.Interfaces
{
    /// <summary>
    /// Provides a mechanism of navigation route.
    /// </summary>
    public interface IRoutable
    {
        /// <summary>
        /// Tries to complete navigation route.
        /// </summary>
        /// <param name="frame"><see cref="Frame"/> in context of navigation operation.</param>
        /// <param name="navigationArgs">Navigation arguments.</param>
        /// <returns><see langword="true"/> - if successfully, else <see langword="false"/>.</returns>
        bool TryExecute(Frame frame, INavigationArgs navigationArgs);
    }
}