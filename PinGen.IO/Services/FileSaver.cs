using PinGen.IO.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows.Media.Imaging;

namespace PinGen.IO.Services
{
    public class FileSaver : IFileSaver
    {
        public void Save(BitmapSource bitmap, string path)
        {
            // Save the BitmapSource as a PNG file
            var encoder = new PngBitmapEncoder();

            // Add the frame to the encoder
            encoder.Frames.Add(BitmapFrame.Create(bitmap));

            // Write the encoded image to the specified path
            using var fileStream = new FileStream(path, FileMode.Create);
            encoder.Save(fileStream);
        }
    }
}
