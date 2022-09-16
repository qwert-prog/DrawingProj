using IoC;
using DrawingProj.ViewModels;

namespace DrawingProj.Services
{
    /// <summary>
    /// Class to access ViewModels
    /// </summary>
    internal class ServiceLocator
    {
        #region Public Properties

        /// <summary>
        /// Gets <see cref="AssetsService"/>
        /// </summary>
        public static AssetsService AssetsService => ServicesContainer.GetService<AssetsService>();

        /// <summary>
        /// Gets <see cref="ListToolsService"/>
        /// </summary>
        public static ListToolsService ListToolsService => ServicesContainer.GetService<ListToolsService>();

        /// <summary>
        /// Gets <see cref="ProjectsPageViewModel"/>
        /// </summary>
        public static ProjectsPageViewModel ProjectsPageViewModel => ServicesContainer.GetService<ProjectsPageViewModel>();

        /// <summary>
        /// Gets <see cref="SketchPageViewModel"/>
        /// </summary>
        public static SketchPageViewModel SketchPageViewModel => ServicesContainer.GetService<SketchPageViewModel>();

        #endregion Public Properties
    }
}