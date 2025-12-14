using PinGen.Core.Models;
using PinGen.ImageProcessing.Interfaces;
using PinGen.IO.Interfaces;
using PinGen.Rendering.Helpers;
using PinGen.Rendering.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Intrinsics.Arm;
using System.Text;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace PinGen.Rendering.Services
{
    public class PinRenderer : IPinRenderer
    {
        private readonly IFontLoader _fontLoader;
        private readonly IImageLoader _imageLoader;
        private readonly IImageProcessor _imageProcessor;

        private static Typeface defaultFont;

        public PinRenderer(IFontLoader fontLoader, IImageLoader imageLoader, IImageProcessor imageProcessor)
        {
            _fontLoader = fontLoader;
            _imageLoader = imageLoader;
            _imageProcessor = imageProcessor;

            // Load default font from base app domain directory
            string appBase = AppDomain.CurrentDomain.BaseDirectory;
            defaultFont = _fontLoader.Load(
                Path.Combine(appBase, "Assets", "Fonts", "horizon.otf"),
                "Horizon");
        }
       
        public RenderTargetBitmap Render(PinRequest request, TemplateDefinition template)
        {
            var bitmap = new RenderTargetBitmap(
                template.Width,
                template.Height,
                96,
                96,
                PixelFormats.Pbgra32);

            var visual = new DrawingVisual();

            RenderOptions.SetBitmapScalingMode(visual, BitmapScalingMode.HighQuality);
            RenderOptions.SetEdgeMode(visual, EdgeMode.Unspecified);

            using var context = visual.RenderOpen();

            // Background (already correct size)
            string appBase = AppDomain.CurrentDomain.BaseDirectory;
            var bgPath = Path.Combine(appBase, "Assets", "Backgrounds", "bg1.png");
            var background = _imageLoader.Load(bgPath);
            context.DrawImage(background, new Rect(0, 0, template.Width, template.Height));

            // Title (black fill + white stroke)
            DrawOutlinedText(context, request.Title, template.TitleArea.X, template.TitleArea.Y, 48, template.TitleArea.Width, Brushes.Black, Brushes.White, 4, TextAlignment.Center);

            // Images
            for (int i = 0; i < request.ItemImages.Count && i < template.TemplateSlots.Count; i++)
            {
                var slot = template.TemplateSlots[i];

                var fitted = slot.Bounds;
                int targetW = (int)Math.Round(fitted.Width);
                int targetH = (int)Math.Round(fitted.Height);

                var imageSharp = _imageLoader.LoadImageSharp(request.ItemImages[i].SourcePath);
                var image = _imageProcessor.PrepareAndRemoveWhite(imageSharp, targetW, targetH);

                var drawRect = fitted.FitTo(image);
                context.DrawImage(image, drawRect);

                // Draw number overlay if enabled
                if (slot.ShowNumber && slot.NumberArea.HasValue)
                {
                    var numberArea = slot.NumberArea.Value;

                    // Circle background
                    context.DrawEllipse(
                        Brushes.Black,
                        null,
                        new Point(
                            numberArea.X + numberArea.Width / 2,
                            numberArea.Y + numberArea.Height / 2),
                        numberArea.Width / 2,
                        numberArea.Height / 2);

                    var numberText = new FormattedText(
                        (i + 1).ToString(),
                        System.Globalization.CultureInfo.InvariantCulture,
                        FlowDirection.LeftToRight,
                        defaultFont,
                        28,
                        Brushes.White,
                        1.0);

                    context.DrawText(
                        numberText,
                        new Point(
                            numberArea.X + (numberArea.Width - numberText.Width) / 2,
                            numberArea.Y + (numberArea.Height - numberText.Height) / 2));
                }
            }

            // Caption
            for (int i = 0; i < request.Captions.Count && i < template.CaptionAreas.Count; i++)
            {
                var area = template.CaptionAreas[i];
                DrawOutlinedText(context, request.Captions[i], area.X, area.Y, 24, area.Width, Brushes.Black, Brushes.White, 2, TextAlignment.Left);
            }

            // Footer
            if (!string.IsNullOrEmpty(request.Footer) && template.FooterArea.HasValue)
            {
                var footerArea = template.FooterArea.Value;
                DrawOutlinedText(context, request.Footer, footerArea.X, footerArea.Y, 48, footerArea.Width, Brushes.Black, Brushes.White, 4, TextAlignment.Center);
            }

            context.Close();
            bitmap.Render(visual);
            bitmap.Freeze();
            return bitmap;
        }


        private static void DrawOutlinedText(DrawingContext ctx, string text, double x, double y, double size, double maxWidth, Brush fill, Brush stroke, double strokeWidth, TextAlignment alignment = TextAlignment.Left)
        {
            var ft = new FormattedText(
                text,
                System.Globalization.CultureInfo.CurrentCulture,
                FlowDirection.LeftToRight,
                defaultFont,
                size,
                fill,
                1.0)
            {
                MaxTextWidth = maxWidth,
                TextAlignment = alignment
            };

            // Use BuildGeometry for outline
            var geo = ft.BuildGeometry(new Point(x, y));

            ctx.DrawGeometry(null, new Pen(stroke, strokeWidth), geo);
            ctx.DrawGeometry(fill, null, geo);
        }

    }
}
