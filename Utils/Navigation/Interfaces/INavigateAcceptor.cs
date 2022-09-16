namespace Utils.Navigation.Interfaces
{
    /// <summary>
    /// Provides data transfer by navigation.
    /// </summary>
    public interface INavigateAcceptor
    {
        /// <summary>
        /// Accepts navigation arguments.
        /// </summary>
        /// <param name="navigationArgs"></param>
        void Notify(INavigationArgs navigationArgs);
    }
}