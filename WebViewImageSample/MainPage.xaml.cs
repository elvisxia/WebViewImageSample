using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Graphics.Imaging;
using Windows.Storage;
using Windows.Storage.FileProperties;
using Windows.Storage.Pickers;
using Windows.Storage.Streams;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace WebViewImageSample
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
        }

        private async Task<String> ToBase64(byte[] image, uint height, uint width, double dpiX = 96, double dpiY= 96)
        {
            var encoded = new InMemoryRandomAccessStream();
            var encoder = await BitmapEncoder.CreateAsync(BitmapEncoder.PngEncoderId, encoded);
            encoder.SetPixelData(BitmapPixelFormat.Bgra8, BitmapAlphaMode.Straight, height, width, dpiX, dpiY, image);
            await encoder.FlushAsync();
            encoded.Seek(0);

            var bytes = new byte[encoded.Size];
            await encoded.AsStream().ReadAsync(bytes, 0, bytes.Length);
            return Convert.ToBase64String(bytes);
        }

        private async Task<String> ToBase64(WriteableBitmap bitmap)
        {
            var bytes = bitmap.PixelBuffer.ToArray();
            return await ToBase64(bytes, (uint)bitmap.PixelWidth, (uint)bitmap.PixelHeight);
        }

        private async void myBtn_Click(object sender, RoutedEventArgs e)
        {
            
            StorageFile myImage = await GetFileAsync();

            ImageProperties properties = await myImage.Properties.GetImagePropertiesAsync();
            WriteableBitmap bmp = new WriteableBitmap((int)properties.Width, (int)properties.Height);
            bmp.SetSource(await myImage.OpenReadAsync());
            String dataStr=await ToBase64(bmp);
            String fileType = myImage.FileType.Substring(1);
            String str = "<figure><img src=\"data:image/"+myImage.FileType+";base64,"+dataStr+"\" alt =\"aaa\" height=\"400\" width=\"400\"/><figcaption>Figure  : thumb_IMG_0057_1024</figcaption></figure>";

            myWebView.NavigateToString(str);
        }

        private async Task<StorageFile> GetFileAsync()
        {
            StorageFile myImage = await ApplicationData.Current.LocalFolder.GetFileAsync("myImage.jpg");
            return myImage;
        }

        //private async void btnUpload_Click(object sender, RoutedEventArgs e)
        //{
        //    var picker = new FileOpenPicker();
        //    picker.ViewMode = Windows.Storage.Pickers.PickerViewMode.Thumbnail;
        //    picker.SuggestedStartLocation =
        //        Windows.Storage.Pickers.PickerLocationId.PicturesLibrary;
        //    picker.FileTypeFilter.Add(".jpg");
        //    picker.FileTypeFilter.Add(".jpeg");
        //    picker.FileTypeFilter.Add(".png");

        //    Windows.Storage.StorageFile file = await picker.PickSingleFileAsync();
        //    if (file != null)
        //    {
        //        WriteableBitmap bmp=null;
        //        using (var randStream =await file.OpenAsync(Windows.Storage.FileAccessMode.Read))
        //        {
        //            BitmapDecoder decoder = await BitmapDecoder.CreateAsync(randStream);
        //            bmp = new WriteableBitmap((int)decoder.PixelWidth, (int)decoder.PixelHeight);

        //            await bmp.SetSourceAsync(randStream);
        //        }

        //        var fileToSave = await ApplicationData.Current.LocalFolder.CreateFileAsync("myImage.png", Windows.Storage.CreationCollisionOption.ReplaceExisting);

        //        using (var stream = await fileToSave.OpenStreamForWriteAsync())
        //        {
        //            BitmapEncoder encoder = await BitmapEncoder.CreateAsync(BitmapEncoder.JpegEncoderId, stream.AsRandomAccessStream());
        //            var pixelStream = bmp.PixelBuffer.AsStream();
        //            byte[] pixels = new byte[bmp.PixelBuffer.Length];

        //            await pixelStream.ReadAsync(pixels, 0, pixels.Length);

        //            encoder.SetPixelData(BitmapPixelFormat.Bgra8, BitmapAlphaMode.Ignore, (uint)bmp.PixelWidth, (uint)bmp.PixelHeight, 96, 96, pixels);

        //            await encoder.FlushAsync();
        //        }
        //    }
        //}
    }
}
