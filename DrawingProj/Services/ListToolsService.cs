using DrawingProj.Model;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Windows.ApplicationModel;

namespace DrawingProj.Services
{
    /// <summary>
    /// Service for generate tools list by assets
    /// </summary>
    public class ListToolsService
    {
        #region Public Fields

        /// <summary>
        /// Generated tools list
        /// </summary>
        public List<DrawingTool> DrawingToolsList;

        #endregion Public Fields

        #region Private Fields

        /// <summary>
        /// Full path to folder with drawing tools images
        /// </summary>
        private readonly string _root = Path.Combine(Package.Current.InstalledLocation.Path, AssetsService.DRAWING_TOOLS_PATH);

        /// <summary>
        /// List of all images for the current tool
        /// </summary>
        private List<string> _allImagesForTheCurrentToolList;

        #endregion Private Fields

        #region Public Methods

        /// <summary>
        /// Handles the end of initialization event <see cref="AssetsService"/>
        /// </summary>
        public void AssetsService_AssetsUpdated()
        {
            int countFreeDrawingTool = 3;
            AssetsService assetsService = ServiceLocator.AssetsService;
            DrawingToolsList = new List<DrawingTool>();
            foreach (string toolKey in assetsService.DrawingToolsDictionary.Keys)
            {
                _allImagesForTheCurrentToolList = assetsService.DrawingToolsDictionary[toolKey];
                List<string> toolFormsImageList = GetToolFormsImagePath(toolKey);
                DrawingTool drawingTool = new DrawingTool()
                {
                    Name = toolKey,
                    MainImagePath = ReturnPathToImage(toolKey, ToolImageTypes.ToolMainImageKey),
                    NotActiveImagePath = ReturnPathToImage(toolKey, ToolImageTypes.ToolNotActiveImageKey),
                    ColorImagePath = ReturnPathToImage(toolKey, ToolImageTypes.ToolColorImageKey),
                    FormsImagePathList = toolFormsImageList,
                    IsPremium = false
                };

                if (countFreeDrawingTool > 0)
                {
                    drawingTool.IsPremium = false;
                    countFreeDrawingTool--;
                }

                if (toolFormsImageList.Count != 0)
                {
                    drawingTool.CurrentForm = toolFormsImageList.First();
                }

                DrawingToolsList.Add(drawingTool);
            }
        }

        #endregion Public Methods

        #region Private Methods

        /// <summary>
        /// Method for gets list forms tool images
        /// </summary>
        /// <returns></returns>
        private List<string> GetToolFormsImagePath(string toolKey)
        {
            List<string> fullFormsPathList = new List<string>();
            foreach (string formPath in _allImagesForTheCurrentToolList.FindAll(path => path.Contains(ToolImageTypes.ToolFormsImageKey)))
            {
                fullFormsPathList.Add(Path.Combine(_root, toolKey, formPath));
            }
            return fullFormsPathList;
        }

        /// <summary>
        /// Get the full path to the image
        /// using a special key for the current tool
        /// </summary>
        /// <param name="keyTool">Name current tool</param>
        /// <param name="keyImage">The special image key that is specified in <see cref="ToolImageTypes"/></param>
        /// <returns></returns>
        private string ReturnPathToImage(string keyTool, string keyImage)
        {
            string fullNameImageFile = _allImagesForTheCurrentToolList.Find(imagePath => imagePath.Contains(keyImage));
            if (fullNameImageFile == null)
            {
                return null;
            }
            else
            {
                return Path.Combine(_root, keyTool, fullNameImageFile);
            }
        }

        #endregion Private Methods
    }
}