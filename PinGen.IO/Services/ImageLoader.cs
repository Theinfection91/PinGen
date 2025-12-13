using PinGen.IO.Interfaces;
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

            using (var stream = File.OpenRead(path))
            {
                bitmap.BeginInit();
                bitmap.CacheOption = BitmapCacheOption.OnLoad; // ensures the stream can be closed
                bitmap.StreamSource = stream;
                bitmap.EndInit();
                bitmap.Freeze(); // makes it cross-thread safe
            }

            return bitmap;
        }
    }
}
