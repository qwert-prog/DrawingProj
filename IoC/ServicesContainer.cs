using Microsoft.Extensions.DependencyInjection;
using System;

namespace IoC
{
    /// <summary>
    /// Class for storing services
    /// </summary>
    public class ServicesContainer
    {
        /// <summary>
        /// Provides access to service container.
        /// </summary>
        private static IServiceProvider _container;

        /// <summary>
        /// Collection of services.
        /// </summary>
        private static readonly IServiceCollection _services = new ServiceCollection();

        /// <summary>
        /// Add services from different projects.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static void AddSingletonService<T>() where T : class
        {
            _services.AddSingleton<T>();
        }

        /// <summary>
        /// Build all services.
        /// </summary>
        /// <returns></returns>
        public static IServiceProvider BuildServices()
        {
            _container = _services.BuildServiceProvider();
            return _container;
        }

        /// <summary>
        /// Provides service from service collection by type.
        /// </summary>
        /// <typeparam name="T">Required type of service.</typeparam>
        /// <returns>Requested service.</returns>
        public static T GetService<T>()
        {
            return _container.GetRequiredService<T>();
        }
    }
}