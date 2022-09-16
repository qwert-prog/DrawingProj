using System;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Graphics.Display;
using Windows.Graphics.Imaging;
using Windows.Storage.Streams;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media.Imaging;

namespace Utils.Services
{
    /// <summary>
    /// Class for rendering UIElement
    /// </summary>
    public class RenderUIElementService
    {
        /// <summary>
        /// Gets or sets UIElement to be rendered
        /// </summary>
        public UIElement TargetElement { get; set; }

        /// <summary>
        /// Method to render the given UIElement
        /// </summary>
        /// <param name="outputFileStream"></param>
        /// <returns></returns>
        public async Task ExportIntoStreamAsync(IRandomAccessStream outputFileStream)
        {
            if (TargetElement == null)
                return;

            RenderTargetBitmap rtb = new RenderTargetBitmap();
            await rtb.RenderAsync(TargetElement);

            IBuffer pixelBuffer = await rtb.GetPixelsAsync();
            byte[] pixels = pixelBuffer.ToArray();
            DisplayInformation displayImformation = DisplayInformation.GetForCurrentView();

            BitmapEncoder encoder = await BitmapEncoder.CreateAsync(BitmapEncoder.PngEncoderId, outputFileStream);

            encoder.SetPixelData(BitmapPixelFormat.Bgra8,
                BitmapAlphaMode.Premultiplied,
                (uint)rtb.PixelWidth,
                (uint)rtb.PixelHeight,
                displayImformation.RawDpiX,
                displayImformation.RawDpiY,
                pixels);

            await encoder.FlushAsync();
        }
    }
}