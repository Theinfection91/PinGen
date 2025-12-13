using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Media.Imaging;

namespace PinGen.IO.Interfaces
{
    public interface IFileSaver
    {
        void Save(BitmapSource bitmap, string path);
    }
}
