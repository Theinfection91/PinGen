using PinGen.IO.Interfaces;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using System.IO;
using System.Windows.Media.Imaging;

namespace PinGen.IO.Services
{
    public class ImageLoader : IImageLoader
    {
        public BitmapImage Load(string path)
        {
            if (!File.Exists(path))
                throw new FileNotFoundException("Image not found", path);

            var bitmap = new BitmapImage();
            using var stream = File.OpenRead(path);
            bitmap.BeginInit();
            bitmap.CacheOption = BitmapCacheOption.OnLoad;
            bitmap.StreamSource = stream;
            bitmap.EndInit();
            bitmap.Freeze();
            return bitmap;
        }

        public Image<Rgba32> LoadImageSharp(string path)
        {
            if (!File.Exists(path))
                throw new FileNotFoundException("Image not found", path);

            return Image.Load<Rgba32>(path); // caller disposes
        }
    }
}