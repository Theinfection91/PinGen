using PinGen.Core.Models;
using PinGen.ImageProcessing.Interfaces;
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
            DrawOutlinedTextAutoFit(
                context,
                request.Title,
                template.TitleArea,
                64,
                24,
                Brushes.Black,
                Brushes.White,
                4,
                TextAlignment.Center);

            // Subtitle (black fill + white stroke)
            DrawOutlinedTextAutoFit(
                context,
                request.Subtitle,
                template.SubtitleArea,
                32,
                24,
                Brushes.Black,
                Brushes.White,
                2,
                TextAlignment.Center);

            // Images
            for (int i = 0; i < request.ItemImages.Count && i < template.TemplateSlots.Count; i++)
            {
                var slot = template.TemplateSlots[i];

                // Calculate scaled and clamped rect
                var scaledRect = GetScaledRect(slot, request.ItemImages[i].Scale);
                
                // Apply random Y offset for visual variance (±15 pixels)
                double yOffset = Random.Shared.Next(-15, 16);
                scaledRect = new Rect(scaledRect.X, scaledRect.Y + yOffset, scaledRect.Width, scaledRect.Height);
                
                var clampedRect = Rect.Intersect(scaledRect, template.SafeZone);
                
                // Skip if completely outside safe zone
                if (clampedRect.IsEmpty)
                    continue;
                
                int targetW = (int)Math.Round(clampedRect.Width);
                int targetH = (int)Math.Round(clampedRect.Height);

                var imageSharp = _imageLoader.LoadImageSharp(request.ItemImages[i].SourcePath);
                var image = _imageProcessor.PrepareAndRemoveWhite(imageSharp, targetW, targetH);

                var drawRect = clampedRect.FitTo(image);
                context.DrawImage(image, drawRect);

                // Draw number overlay if enabled
                if (slot.ShowNumber && slot.NumberArea.HasValue)
                {
                    var numberArea = slot.NumberArea.Value;

                    // Create two number texts offset for 3d effect
                    var numberTextShadow = new FormattedText(
                        (i + 1).ToString(),
                        System.Globalization.CultureInfo.InvariantCulture,
                        FlowDirection.LeftToRight,
                        defaultFont,
                        72,
                        Brushes.Black,
                        1.0);
                    var numberText = new FormattedText(
                        (i + 1).ToString(),
                        System.Globalization.CultureInfo.InvariantCulture,
                        FlowDirection.LeftToRight,
                        defaultFont,
                        64,
                        Brushes.White,
                        1.0);

                    // Draw shadow offset
                    context.DrawText(
                        numberTextShadow,
                        new Point(
                            numberArea.X + (numberArea.Width - numberTextShadow.Width) / 2 + 6,
                            numberArea.Y + (numberArea.Height - numberTextShadow.Height) / 2));
                    // Draw main text
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
                DrawOutlinedTextAutoFit(
                    context,
                    request.Captions[i],
                    area,
                    24,
                    12,
                    Brushes.Black,
                    Brushes.White,
                    2,
                    TextAlignment.Center);
            }

            // Footer
            if (!string.IsNullOrEmpty(request.Footer) && template.FooterArea.HasValue)
            {
                var footerArea = template.FooterArea.Value;
                DrawOutlinedTextAutoFit(
                    context,
                    request.Footer,
                    footerArea,
                    48,
                    18,
                    Brushes.Black,
                    Brushes.White,
                    4,
                    TextAlignment.Center);
            }

            // Finalize
            context.Close();
            bitmap.Render(visual);
            bitmap.Freeze();
            return bitmap;
        }

        private static void DrawOutlinedTextAutoFit(DrawingContext ctx, string text, Rect area, double maxFontSize, double minFontSize, Brush fill, Brush stroke, double strokeWidth, TextAlignment alignment = TextAlignment.Left)
        {
            if (string.IsNullOrWhiteSpace(text))
                return;

            FormattedText ft = null;
            double fontSize = maxFontSize;

            // Decrease font size until it fits within area
            while (fontSize >= minFontSize)
            {
                ft = new FormattedText(
                    text,
                    System.Globalization.CultureInfo.CurrentCulture,
                    FlowDirection.LeftToRight,
                    defaultFont,
                    fontSize,
                    fill,
                    1.0)
                {
                    MaxTextWidth = area.Width,
                    TextAlignment = alignment,
                    Trimming = TextTrimming.None
                };
                // Check if it fits, if so, break
                if (ft.Height <= area.Height)
                    break;

                if (fontSize == minFontSize)
                {
                    ft.Trimming = TextTrimming.CharacterEllipsis;
                    break;
                }

                // Otherwise, reduce font size and try again
                fontSize -= 1;
            }

            if (ft == null || fontSize < minFontSize)
                return;

            // Center vertically within area
            var origin = new Point(
                area.X,
                area.Y + (area.Height - ft.Height) / 2);

            // Build geometry and draw with stroke and fill
            var geo = ft.BuildGeometry(origin);
            ctx.DrawGeometry(null, new Pen(stroke, strokeWidth), geo);
            ctx.DrawGeometry(fill, null, geo);
        }

        private static Rect GetScaledRect(TemplateSlot slot, double scale)
        {
            // Original center point
            double centerX = slot.Bounds.X + slot.Bounds.Width / 2;
            double centerY = slot.Bounds.Y + slot.Bounds.Height / 2;

            // New dimensions
            double newWidth = slot.Bounds.Width * scale;
            double newHeight = slot.Bounds.Height * scale;

            // Reposition around same center
            return new Rect(
                centerX - newWidth / 2,
                centerY - newHeight / 2,
                newWidth,
                newHeight);
        }
    }
}
