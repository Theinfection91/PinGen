using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Media;

namespace PinGen.IO.Interfaces
{
    public interface IFontLoader
    {
        Typeface Load(string fontPath);
    }
}
