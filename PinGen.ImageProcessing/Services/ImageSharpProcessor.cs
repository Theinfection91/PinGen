using PinGen.ImageProcessing.Interfaces;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace PinGen.ImageProcessing.Services
{
    public class ImageSharpProcessor : IImageProcessor
    {
        public BitmapSource LoadPrepareAndRemoveWhite(string path, int targetWidth, int targetHeight, byte tolerance = 15)
        {
            using var image = Image.Load<Rgba32>(path);
            var frame = image.Frames.RootFrame;

            int width = frame.Width;
            int height = frame.Height;

            // Top-to-bottom and bottom-to-top edge removal
            frame.ProcessPixelRows(accessor =>
            {
                // Top-to-bottom per column
                for (int x = 0; x < width; x++)
                {
                    for (int y = 0; y < height; y++)
                    {
                        ref Rgba32 p = ref accessor.GetRowSpan(y)[x];
                        if (!IsWhite(p, tolerance)) break;
                        p.A = 0;
                    }

                    // Bottom-to-top
                    for (int y = height - 1; y >= 0; y--)
                    {
                        ref Rgba32 p = ref accessor.GetRowSpan(y)[x];
                        if (!IsWhite(p, tolerance)) break;
                        p.A = 0;
                    }
                }

                // Left-to-right per row
                for (int y = 0; y < height; y++)
                {
                    var row = accessor.GetRowSpan(y);

                    for (int x = 0; x < width; x++)
                    {
                        ref Rgba32 p = ref row[x];
                        if (!IsWhite(p, tolerance)) break;
                        p.A = 0;
                    }

                    // Right-to-left
                    for (int x = width - 1; x >= 0; x--)
                    {
                        ref Rgba32 p = ref row[x];
                        if (!IsWhite(p, tolerance)) break;
                        p.A = 0;
                    }
                }
            });

            // Resize AFTER transparency adjustment
            image.Mutate(ctx => ctx.Resize(new ResizeOptions
            {
                Size = new Size(targetWidth, targetHeight),
                Mode = ResizeMode.Max,
                Sampler = KnownResamplers.Lanczos3
            }));

            // Copy pixels into BitmapSource
            var pixels = new byte[image.Width * image.Height * 4];
            frame.ProcessPixelRows(accessor =>
            {
                for (int y = 0; y < image.Height; y++)
                {
                    var row = accessor.GetRowSpan(y);
                    for (int x = 0; x < row.Length; x++)
                    {
                        int idx = (y * image.Width + x) * 4;
                        var p = row[x];

                        // Premultiply alpha
                        float alpha = p.A / 255f;
                        pixels[idx] = (byte)(p.R * alpha);
                        pixels[idx + 1] = (byte)(p.G * alpha);
                        pixels[idx + 2] = (byte)(p.B * alpha);
                        pixels[idx + 3] = p.A;
                    }
                }
            });

            var bmp = BitmapSource.Create(
                image.Width,
                image.Height,
                96,
                96,
                PixelFormats.Pbgra32,
                null,
                pixels,
                image.Width * 4);

            bmp.Freeze();
            return bmp;
        }

        private static bool IsWhite(Rgba32 p, byte tolerance) =>
            p.R >= 255 - tolerance && p.G >= 255 - tolerance && p.B >= 255 - tolerance;
    }
}
