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
        public BitmapSource PrepareAndRemoveWhite(Image<Rgba32> image, int targetWidth, int targetHeight, byte tolerance = 15)
        {
            var frame = image.Frames.RootFrame;

            int width = frame.Width;
            int height = frame.Height;

            // Step 1: Remove white edges
            frame.ProcessPixelRows(accessor =>
            {
                for (int x = 0; x < width; x++)
                {
                    for (int y = 0; y < height; y++)
                    {
                        ref Rgba32 p = ref accessor.GetRowSpan(y)[x];
                        if (!IsWhite(p, tolerance)) break;
                        p.A = 0;
                    }
                    for (int y = height - 1; y >= 0; y--)
                    {
                        ref Rgba32 p = ref accessor.GetRowSpan(y)[x];
                        if (!IsWhite(p, tolerance)) break;
                        p.A = 0;
                    }
                }
                for (int y = 0; y < height; y++)
                {
                    var row = accessor.GetRowSpan(y);
                    for (int x = 0; x < width; x++)
                    {
                        ref Rgba32 p = ref row[x];
                        if (!IsWhite(p, tolerance)) break;
                        p.A = 0;
                    }
                    for (int x = width - 1; x >= 0; x--)
                    {
                        ref Rgba32 p = ref row[x];
                        if (!IsWhite(p, tolerance)) break;
                        p.A = 0;
                    }
                }
            });

            // Step 2: Zero out RGB for fully transparent pixels
            image.ProcessPixelRows(accessor =>
            {
                for (int y = 0; y < image.Height; y++)
                {
                    var row = accessor.GetRowSpan(y);
                    for (int x = 0; x < row.Length; x++)
                    {
                        ref Rgba32 p = ref row[x];
                        if (p.A == 0)
                        {
                            p.R = 0;
                            p.G = 0;
                            p.B = 0;
                        }
                    }
                }
            });

            // Step 3: Resize
            image.Mutate(ctx =>
            {
                ctx.Resize(new ResizeOptions
                {
                    Size = new Size(targetWidth, targetHeight),
                    Mode = ResizeMode.Max,
                    Sampler = KnownResamplers.Lanczos3
                });
            });

            // Step 4: Copy pixels in BGR premultiplied format for WPF
            var pixels = new byte[image.Width * image.Height * 4];
            image.ProcessPixelRows(accessor =>
            {
                for (int y = 0; y < image.Height; y++)
                {
                    var row = accessor.GetRowSpan(y);
                    for (int x = 0; x < row.Length; x++)
                    {
                        int idx = (y * image.Width + x) * 4;
                        var p = row[x];

                        pixels[idx] = (byte)((p.B * p.A + 127) / 255);
                        pixels[idx + 1] = (byte)((p.G * p.A + 127) / 255);
                        pixels[idx + 2] = (byte)((p.R * p.A + 127) / 255);
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
