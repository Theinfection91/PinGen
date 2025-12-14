using PinGen.ImageProcessing.Interfaces;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using System.IO;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace PinGen.ImageProcessing.Services
{
    public class ImageSharpProcessor : IImageProcessor
    {
        private static BitmapSource ToBitmapSource(Image<Rgba32> image)
        {
            var pixels = new byte[image.Width * image.Height * 4];
            image.CopyPixelDataTo(pixels);

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

        public BitmapSource LoadPrepareAndRemoveWhite(string path, int targetWidth, int targetHeight, byte tolerance = 15)
        {
            using var image = Image.Load<Rgba32>(path);

            // Remove white background FIRST
            image.ProcessPixelRows(accessor =>
            {
                for (int y = 0; y < accessor.Height; y++)
                {
                    var row = accessor.GetRowSpan(y);
                    for (int x = 0; x < row.Length; x++)
                    {
                        ref Rgba32 p = ref row[x];

                        // Calculate "whiteness" as average of RGB
                        float whiteness = (p.R + p.G + p.B) / (3f * 255f);

                        // Remove nearly white pixels with feathering
                        if (whiteness > 0.9f)
                        {
                            // Proportional alpha reduction
                            float alphaFactor = (1f - (whiteness - 0.9f) / 0.1f);
                            p.A = (byte)(p.A * Math.Clamp(alphaFactor, 0f, 1f));
                        }
                    }
                }
            });

            // Resize AFTER transparency exists
            image.Mutate(x => x.Resize(new ResizeOptions
            {
                Size = new Size(targetWidth, targetHeight),
                Mode = ResizeMode.Max,
                Sampler = KnownResamplers.Lanczos3
            }));

            image.Mutate(x => x.GaussianBlur(0.5f));


            // Convert once
            return ToBitmapSource(image);
        }
    }
}
