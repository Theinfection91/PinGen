using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using System.IO;
using System.Windows.Media.Imaging;
using PinGen.ImageProcessing.Interfaces;

namespace PinGen.ImageProcessing.Services
{
    public class ImageSharpProcessor : IImageProcessor
    {
        public BitmapSource RemoveWhiteBackground(string imagePath, byte tolerance = 15)
        {
            using Image<Rgba32> image = Image.Load<Rgba32>(imagePath);

            image.ProcessPixelRows(accessor =>
            {
                for (int y = 0; y < accessor.Height; y++)
                {
                    var row = accessor.GetRowSpan(y);
                    for (int x = 0; x < row.Length; x++)
                    {
                        ref Rgba32 pixel = ref row[x];

                        // If pixel is near white, make it transparent
                        if (pixel.R >= (255 - tolerance) &&
                            pixel.G >= (255 - tolerance) &&
                            pixel.B >= (255 - tolerance))
                        {
                            pixel.A = 0;
                        }
                    }
                }
            });

            using var ms = new MemoryStream();
            image.SaveAsPng(ms);
            ms.Position = 0;

            var bmp = new BitmapImage();
            bmp.BeginInit();
            bmp.StreamSource = ms;
            bmp.CacheOption = BitmapCacheOption.OnLoad;
            bmp.EndInit();
            bmp.Freeze();

            return bmp;
        }
    }
}
