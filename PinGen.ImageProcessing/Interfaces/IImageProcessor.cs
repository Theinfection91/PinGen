using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Media.Imaging;

namespace PinGen.ImageProcessing.Interfaces
{
    public interface IImageProcessor
    {
        BitmapSource LoadPrepareAndRemoveWhite(Image<Rgba32> image, int targetWidth, int targetHeight, byte tolerance = 15);
    }
}
