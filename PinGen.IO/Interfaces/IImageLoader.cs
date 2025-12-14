using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using System.Windows.Media.Imaging;

namespace PinGen.IO.Interfaces
{
    public interface IImageLoader
    {
        BitmapImage Load(string path);
        Image<Rgba32> LoadImageSharp(string path);
    }
}
