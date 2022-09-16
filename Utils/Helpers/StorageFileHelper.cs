using IoC;
using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Utils.Services;
using Windows.Graphics.Imaging;
using Windows.Storage;
using Windows.Storage.Streams;

namespace Utils.Helpers
{
    /// <summary>
    /// Class for getting/saving files in local storage by file name
    /// </summary>
    public class LocalStorageFileHelper
    {
        #region Public Methods

        /// <summary>
        /// Method to create the StorageFile in folder
        /// </summary>
        /// <param name="folderPath">Folder path</param>
        /// <param name="fileName">Name of sketch</param>
        /// <returns></returns>
        public static async Task CreateFileFromLocalStorageAsync(string folderPath, string fileName)
        {
            StorageFolder folder = await StorageFolder.GetFolderFromPathAsync(folderPath);
            await folder.CreateFileAsync(fileName);
        }

        /// <summary>
        /// Method to create StorageFolder in local folder app
        /// </summary>
        /// <param name="nameFolder">Created folder name</param>
        /// <param name="folderPath">Folder path</param>

        /// <returns></returns>
        public static async Task<StorageFolder> CreateFolderFromLocalStorageAsync(string folderPath, string nameFolder)
        {
            StorageFolder curFolder = await StorageFolder.GetFolderFromPathAsync(folderPath);
            return await curFolder.CreateFolderAsync(nameFolder);
        }

        /// <summary>
        /// Method to delete the StorageFile in storage folder
        /// </summary>
        /// <param name="folderPath">Folder path</param>
        /// <param name="nameFile">file name</param>
        /// <returns></returns>
        public static async Task DeleteFileFromLocalStorageAsync(string folderPath, string nameFile)
        {
            StorageFolder folder = await StorageFolder.GetFolderFromPathAsync(folderPath);
            if (await folder.TryGetItemAsync(nameFile) != null)
            {
                StorageFile file = await folder.GetFileAsync(nameFile);
                await file.DeleteAsync();
            }
        }

        /// <summary>
        /// Method to delete the StorageFolder in local folder app
        /// </summary>
        /// <param name="folderPath">Folder path</param>
        /// <param name="nameFolder">Folder name of sketch</param>
        /// <returns></returns>
        public static async Task DeleteFolderFromLocalStorageAsync(string folderPath, string nameFolder)
        {
            StorageFolder folder = await StorageFolder.GetFolderFromPathAsync(folderPath);
            if (await folder.TryGetItemAsync(nameFolder) != null)
            {
                StorageFolder sketchFolder = await folder.GetFolderAsync(nameFolder);
                await sketchFolder.DeleteAsync();
            }
        }

        /// <summary>
        /// Method for getting the StorageFile of the sketch
        /// </summary>
        /// <param name="filePath">File path</param>
        /// <returns></returns>
        public static async Task<StorageFile> GetFileFromLocalStorageAsync(string filePath)
        {
            if (await StorageFile.GetFileFromPathAsync(filePath) != null)
            {
                return await StorageFile.GetFileFromPathAsync(filePath);
            }
            return null;
        }

        /// <summary>
        /// Method to rename the StorageFile of the sketch
        /// </summary>
        /// <param name="folderPath">Folder path</param>
        /// <param name="oldName">Old full name of sketch</param>
        /// <param name="newName">New full name of sketch</param>
        /// <param name="nameFolder">Folder name of sketch</param>
        /// <returns></returns>
        public static async Task RenameSketchFileFromLocalStorageAsync(string folderPath, string nameFolder, string oldName, string newName)
        {
            StorageFolder folder = await StorageFolder.GetFolderFromPathAsync(folderPath);
            if (await folder.TryGetItemAsync(nameFolder) != null)
            {
                StorageFolder sketchFolder = await folder.GetFolderAsync(nameFolder);
                StorageFile sketchFile = await sketchFolder.GetFileAsync(oldName);
                if (await sketchFolder.TryGetItemAsync(newName) == null)
                {
                    await sketchFile.RenameAsync(newName);
                }
                string newNameFolder = Path.GetFileNameWithoutExtension(newName);
                await sketchFolder.RenameAsync(newNameFolder);
            }
        }

        /// <summary>
        /// Method to rename the StorageFile in the folder
        /// </summary>
        /// <param name="folderPath">Folder path</param>
        /// <param name="oldFileName">Old full name file</param>
        /// <param name="newFileName">New full name file</param>
        /// <returns></returns>
        public static async Task RenameStorageFileFromFolder(string folderPath, string oldFileName, string newFileName)
        {
            StorageFolder folder = await StorageFolder.GetFolderFromPathAsync(folderPath);
            if (await folder.TryGetItemAsync(oldFileName) != null)
            {
                StorageFile file = await folder.GetFileAsync(oldFileName);
                if (await folder.TryGetItemAsync(newFileName) == null)
                {
                    await file.RenameAsync(newFileName);
                }
            }
        }

        /// <summary>
        /// Method for saves sketch in the StorageFile
        /// </summary>
        /// <param name="filePath">Sketch file path</param>
        public static async Task SaveSketch(string filePath)
        {
            StorageFile sketchFile = await GetFileFromLocalStorageAsync(filePath);
            using (IRandomAccessStream stream = await sketchFile.OpenAsync(FileAccessMode.ReadWrite))
            {
                RenderUIElementService renderUIElement = ServicesContainer.GetService<RenderUIElementService>();
                try
                {
                    await renderUIElement.ExportIntoStreamAsync(stream);
                    BitmapDecoder decoder = await BitmapDecoder.CreateAsync(stream);
                    BitmapEncoder encoder = await BitmapEncoder.CreateForTranscodingAsync(stream, decoder);
                    await encoder.FlushAsync();
                }
                catch
                {
                }
            }
        }

        /// <summary>
        /// Method for checking the validity of a filename
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static bool ValidateFileName(string name)
        {
            var invalidChars = string.Join("", Path.GetInvalidFileNameChars());
            var regex = new Regex("[" + Regex.Escape(string.Join("", invalidChars)) + "]");
            bool isContainsInvalidChars = regex.IsMatch(name);
            bool isEndsPointOrSpace = (name[name.Length - 1] == ' ' || name[name.Length - 1] == '.');
            if (isEndsPointOrSpace || isContainsInvalidChars)
            {
                return false;
            }
            return true;
        }

        #endregion Public Methods
    }
}