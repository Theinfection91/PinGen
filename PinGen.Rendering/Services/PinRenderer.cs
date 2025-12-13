using PinGen.Core.Models;
using PinGen.IO.Interfaces;
using PinGen.Rendering.Helpers;
using PinGen.Rendering.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace PinGen.Rendering.Services
{
    public class PinRenderer : IPinRenderer
    {
        private readonly IImageLoader _imageLoader;

        public PinRenderer(IImageLoader imageLoader)
        {
            _imageLoader = imageLoader;
        }

        public RenderTargetBitmap Render(PinRequest request, TemplateDefinition template)
        {
            var bitmap = new RenderTargetBitmap(template.Width, template.Height, 96, 96, PixelFormats.Pbgra32);
            var visual = new DrawingVisual();

            using var context = visual.RenderOpen();

            // Draw background dynamically
            //string appBase = AppDomain.CurrentDomain.BaseDirectory;
            //string bgPath = Path.Combine(appBase, "Assets", "Backgrounds", "bg1.png");
            string bgPath = @"C:\Chase\CSharpProjects\PinGen\PinGen.App\Assets\Backgrounds\bg1.png";
            var background = _imageLoader.Load(bgPath);
            context.DrawImage(background, new Rect(0, 0, template.Width, template.Height));

            // Draw title
            var titleText = new FormattedText(
                request.Title,
                System.Globalization.CultureInfo.CurrentCulture,
                FlowDirection.LeftToRight,
                new Typeface("Arial"),
                48,
                Brushes.Black,
                1.0);

            context.DrawText(titleText, new Point(template.TitleArea.X, template.TitleArea.Y));

            // Draw images
            for (int i = 0; i < request.ItemImages.Count && i < template.ImageSlots.Count; i++)
            {
                var slot = template.ImageSlots[i];
                var image = _imageLoader.Load(request.ItemImages[i].SourcePath);

                // Fit image into slot
                context.DrawImage(image, slot.FitTo(image));
            }

            // Draw captions
            for (int i = 0; i < request.Captions.Count && i < template.CaptionAreas.Count; i++)
            {
                var area = template.CaptionAreas[i];
                var captionText = new FormattedText(
                    request.Captions[i],
                    System.Globalization.CultureInfo.CurrentCulture,
                    FlowDirection.LeftToRight,
                    new Typeface("Arial"),
                    24,
                    Brushes.Black,
                    1.0);

                context.DrawText(captionText, new Point(area.X, area.Y));
            }

            // Draw footer if exists
            if (!string.IsNullOrEmpty(request.Footer) && template.FooterArea.HasValue)
            {
                var footerArea = template.FooterArea.Value;
                var footerText = new FormattedText(
                    request.Footer,
                    System.Globalization.CultureInfo.CurrentCulture,
                    FlowDirection.LeftToRight,
                    new Typeface("Arial"),
                    48,
                    Brushes.Gray,
                    1.0);
                context.DrawText(footerText, new Point(footerArea.X, footerArea.Y));
            }

            context.Close();
            bitmap.Render(visual);
            bitmap.Freeze();
            return bitmap;
        }
    }
}
