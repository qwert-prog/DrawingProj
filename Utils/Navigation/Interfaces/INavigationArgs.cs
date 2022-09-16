using System;

namespace Utils.Navigation.Interfaces
{
    /// <summary>
    /// Provides a description of navigation event arguments.
    /// </summary>
    public interface INavigationArgs
    {
        #region Public Properties

        /// <summary>
        /// Gets or sets delegate of navigation completion.
        /// </summary>
        Action NavigationCompleted { get; set; }

        /// <summary>
        /// Presents navigation data.
        /// </summary>
        object NavigationData { get; }

        #endregion Public Properties

        #region Public Methods

        /// <summary>
        /// Tries to complete navigation data casting to concrete type.
        /// </summary>
        /// <typeparam name="T">Type to cast.</typeparam>
        /// <param name="navigationData">Casted data.</param>
        /// <returns><see langword="true"/> - if casting completed successfully, else <see langword="false"/>.</returns>
        bool TryNavigationDataCast<T>(out T navigationData);

        #endregion Public Methods
    }
}