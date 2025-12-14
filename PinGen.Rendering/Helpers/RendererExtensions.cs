using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Media.Imaging;

namespace PinGen.Rendering.Helpers
{
    public static class RendererExtensions
    {
        public static Rect FitTo(this Rect slot, BitmapSource image)
        {
            double scale = Math.Min(
                slot.Width / image.PixelWidth,
                slot.Height / image.PixelHeight);

            double width = image.PixelWidth * scale;
            double height = image.PixelHeight * scale;

            double x = slot.X + (slot.Width - width) / 2;
            double y = slot.Y + (slot.Height - height) / 2;

            // Pixel snap
            return new Rect(
                Math.Round(x),
                Math.Round(y),
                Math.Round(width),
                Math.Round(height));
        }

    }
}
