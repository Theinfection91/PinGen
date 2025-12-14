using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Media.Imaging;

namespace PinGen.ImageProcessing.Interfaces
{
    public interface IImageProcessor
    {
        //BitmapSource RemoveWhiteBackground(string imagePath, byte tolerance = 15);
        //BitmapSource LoadAndPrepare(string path, int targetWidth, int targetHeight);
        BitmapSource LoadPrepareAndRemoveWhite(string path, int targetWidth, int targetHeight, byte tolerance = 15);
    }
}
