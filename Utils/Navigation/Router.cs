using Utils.Navigation.Interfaces;
using System;
using System.Collections.Generic;
using Windows.UI.Xaml.Controls;
using Microsoft.Toolkit.Mvvm.ComponentModel;

namespace Utils.Navigation
{
    /// <summary>
    /// Provides registration methods and routes management.
    /// </summary>
    public class Router : IRouter
    {
        #region Private Fields

        /// <summary>
        /// Represents collection of routes.
        /// </summary>
        private readonly Dictionary<ObservableObject, Type> _routes;

        #endregion Private Fields

        #region Public Constructors

        /// <summary>
        /// Creates an instance of <see cref="Route"/>.
        /// </summary>
        /// <param name="serviceProvider"></param>
        public Router()
        {
            _routes = new Dictionary<ObservableObject, Type>();
        }

        #endregion Public Constructors

        #region Public Methods

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="viewModel"><inheritdoc/></param>
        /// <returns><inheritdoc/></returns>
        public IRoutable TryGetRoute(ObservableObject viewModel)
        {
            if (_routes.TryGetValue(viewModel, out Type view))
            {
                return new Route(view, viewModel);
            }
            throw new KeyNotFoundException("Route not found");
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <typeparam name="TView"><inheritdoc/></typeparam>
        /// <param name="key"><inheritdoc/></param>
        public void RegisterRoute<TView>(ObservableObject key) where TView : Page, new()
        {
            Type value = typeof(TView);
            _routes.Add(key, value);
        }

        #endregion Public Methods
    }
}