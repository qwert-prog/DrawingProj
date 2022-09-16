using System;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.ApplicationModel.DataTransfer;
using Windows.Storage;
using Windows.Storage.FileProperties;
using Windows.Storage.Streams;

namespace Utils.Services
{
    /// <summary>
    /// Service to implement the share function
    /// </summary>
    public class ShareDataService
    {
        #region Private Fields

        /// <summary>
        /// Class instance <see cref="ShareDataService"/>, синглтон
        /// </summary>
        private static readonly Lazy<ShareDataService> _instance =
            new Lazy<ShareDataService>(() => new ShareDataService());

        /// <summary>
        /// Transfer file
        /// </summary>
        private StorageFile _file;

        #endregion Private Fields

        #region Private Constructors

        /// <summary>
        /// Initializes an instance of a class <see cref="ShareDataService"/>
        /// </summary>
        private ShareDataService()
        {
            DataTransferManager dataTransferManager = DataTransferManager.GetForCurrentView();
            dataTransferManager.DataRequested += DataTransferManager_DataRequested;
        }

        #endregion Private Constructors

        #region Public Methods

        /// <summary>
        /// Returns <see cref="ShareDataService"/>
        /// </summary>
        /// <returns></returns>
        public static ShareDataService GetInstance()
        {
            return _instance.Value;
        }

        /// <summary>
        /// Method to start file transfer
        /// </summary>
        /// <param name="file"></param>
        public void ShareFile(StorageFile file)
        {
            _file = file;
            DataTransferManager.ShowShareUI();
        }

        #endregion Public Methods

        #region Private Methods

        /// <summary>
        /// Handling the data transfer start event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private async void DataTransferManager_DataRequested(DataTransferManager sender, DataRequestedEventArgs args)
        {
            DataRequestDeferral deferral = args.Request.GetDeferral();

            await ShareAsync(args.Request.Data);

            deferral.Complete();
        }

        /// <summary>
        /// Prepares and sends a file
        /// </summary>
        /// <param name="dataPackage"></param>
        /// <returns></returns>
        private async Task ShareAsync(DataPackage dataPackage)
        {
            AppDisplayInfo appDisplayInfo = AppInfo.Current.DisplayInfo;
            dataPackage.Properties.Title = appDisplayInfo.DisplayName;
            dataPackage.Properties.Description = appDisplayInfo.Description;
            using (IRandomAccessStream stream = await _file.OpenAsync(FileAccessMode.Read))
            {
                dataPackage.SetBitmap(RandomAccessStreamReference.CreateFromStream(stream));
                dataPackage.SetStorageItems(new[] { _file });
                StorageItemThumbnail thumbnail = await _file.GetThumbnailAsync(ThumbnailMode.SingleItem);
                dataPackage.Properties.Thumbnail = RandomAccessStreamReference.CreateFromStream(thumbnail);
            }
        }

        #endregion Private Methods
    }
}