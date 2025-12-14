using PinGen.IO.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Media;

namespace PinGen.IO.Services
{
    public class FontLoader : IFontLoader
    {
        // TODO: Fine tune this later as WPF font loading can be tricky
        public Typeface Load(string fontPath, string fontName)
        {
            if (!File.Exists(fontPath))
                throw new FileNotFoundException("Font file not found", fontPath);

            var fontFamily = new FontFamily(new Uri(fontPath), $"./#{fontName}");
            return new Typeface(fontFamily, FontStyles.Normal, FontWeights.Normal, FontStretches.Normal);
        }
    }
}
