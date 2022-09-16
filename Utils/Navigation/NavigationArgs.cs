using System;
using Utils.Navigation.Interfaces;

namespace Utils.Navigation
{
    public class NavigationArgs : INavigationArgs
    {
        #region Public Properties

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public Action NavigationCompleted { get; set; }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public object NavigationData { get; }

        #endregion Public Properties

        #region Public Constructors

        /// <summary>
        /// Creates an instance of <see cref="NavigationArgs"/>.
        /// </summary>
        /// <param name="navigationData"></param>
        public NavigationArgs(object navigationData)
        {
            NavigationData = navigationData;
        }

        #endregion Public Constructors

        #region Public Methods

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <typeparam name="T"><inheritdoc/></typeparam>
        /// <param name="navigationData"><inheritdoc/></param>
        /// <returns><inheritdoc/></returns>
        public bool TryNavigationDataCast<T>(out T navigationData)
        {
            navigationData = default;
            if (NavigationData is T data)
            {
                navigationData = data;
                return true;
            }

            return false;
        }

        #endregion Public Methods
    }
}