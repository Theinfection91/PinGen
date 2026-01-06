using PinGen.Core.Models;
using PinGen.ImageProcessing.Interfaces;
using PinGen.IO.Interfaces;
using PinGen.Rendering.Helpers;
using PinGen.Rendering.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace PinGen.Rendering.Services
{
    public class PinRenderer : IPinRenderer
    {
        private readonly IImageLoader _imageLoader;
        private readonly IImageProcessor _imageProcessor;
        private static Typeface _defaultFont = null!;
        private readonly string _defaultBackgroundPath;

        private const int CanvasWidth = 1000;
        private const int CanvasHeight = 1500;

        public PinRenderer(IFontLoader fontLoader, IImageLoader imageLoader, IImageProcessor imageProcessor)
        {
            _imageLoader = imageLoader;
            _imageProcessor = imageProcessor;

            string appBase = AppDomain.CurrentDomain.BaseDirectory;
            _defaultFont = fontLoader.Load(Path.Combine(appBase, "Assets", "Fonts", "horizon.otf"), "Horizon");
            _defaultBackgroundPath = Path.Combine(appBase, "Assets", "Backgrounds", "bg1.png");
        }

        public static Typeface DefaultFont => _defaultFont;

        public RenderTargetBitmap Render(PinRequest request, TemplateDefinition template)
            => Render(request, template, _defaultBackgroundPath, null);

        public RenderTargetBitmap Render(PinRequest request, TemplateDefinition template, List<double> yOffsets)
            => Render(request, template, _defaultBackgroundPath, yOffsets);

        public RenderTargetBitmap Render(PinRequest request, TemplateDefinition template, string backgroundPath)
            => Render(request, template, backgroundPath, null);

        public RenderTargetBitmap Render(PinRequest request, TemplateDefinition template, string backgroundPath, List<double>? yOffsets)
        {
            var bitmap = new RenderTargetBitmap(template.Width, template.Height, 96, 96, PixelFormats.Pbgra32);
            var visual = new DrawingVisual();
            RenderOptions.SetBitmapScalingMode(visual, BitmapScalingMode.HighQuality);

            using var context = visual.RenderOpen();

            context.DrawImage(_imageLoader.Load(backgroundPath), new Rect(0, 0, template.Width, template.Height));

            DrawOutlinedText(context, request.Title, template.TitleArea, 64, 24, 4, _defaultFont);
            DrawOutlinedText(context, request.Subtitle, template.SubtitleArea, 32, 24, 2, _defaultFont);

            for (int i = 0; i < request.ItemImages.Count && i < template.TemplateSlots.Count; i++)
            {
                var slot = template.TemplateSlots[i];
                var scaledRect = GetScaledRect(slot, request.ItemImages[i].Scale);
                double yOffset = yOffsets != null && i < yOffsets.Count ? yOffsets[i] : Random.Shared.Next(-15, 16);
                scaledRect = new Rect(scaledRect.X, scaledRect.Y + yOffset, scaledRect.Width, scaledRect.Height);

                int targetW = (int)Math.Round(scaledRect.Width);
                int targetH = (int)Math.Round(scaledRect.Height);
                if (targetW <= 0 || targetH <= 0) continue;

                var image = _imageProcessor.PrepareAndRemoveWhite(_imageLoader.LoadImageSharp(request.ItemImages[i].SourcePath), targetW, targetH);
                context.DrawImage(image, scaledRect.FitTo(image));

                if (slot.ShowNumber && slot.NumberArea.HasValue)
                    DrawNumber(context, i + 1, slot.NumberArea.Value.X, slot.NumberArea.Value.Y + yOffset, 1.0);
            }

            for (int i = 0; i < request.Captions.Count && i < template.CaptionAreas.Count; i++)
                DrawOutlinedTextFixed(context, request.Captions[i].Text, template.CaptionAreas[i], request.Captions[i].FontSize, 2, _defaultFont);

            if (!string.IsNullOrEmpty(request.Footer) && template.FooterArea.HasValue)
                DrawOutlinedText(context, request.Footer, template.FooterArea.Value, 48, 18, 4, _defaultFont);

            context.Close();
            bitmap.Render(visual);
            bitmap.Freeze();
            return bitmap;
        }

        public RenderTargetBitmap RenderWithEditorPositions(
            PinRequest request, string backgroundPath,
            ElementPosition titlePosition, ElementPosition subtitlePosition, ElementPosition? footerPosition,
            List<ElementPosition> captionPositions, List<EditorImageElement> imageElements, List<EditorNumberElement> numberElements,
            double titleFontSize = 64, double subtitleFontSize = 32, double footerFontSize = 48,
            Typeface? titleFont = null, Typeface? subtitleFont = null, Typeface? footerFont = null, List<Typeface>? captionFonts = null)
        {
            titleFont ??= _defaultFont;
            subtitleFont ??= _defaultFont;
            footerFont ??= _defaultFont;

            var bitmap = new RenderTargetBitmap(CanvasWidth, CanvasHeight, 96, 96, PixelFormats.Pbgra32);
            var visual = new DrawingVisual();
            RenderOptions.SetBitmapScalingMode(visual, BitmapScalingMode.HighQuality);

            using var context = visual.RenderOpen();

            context.DrawImage(_imageLoader.Load(backgroundPath), new Rect(0, 0, CanvasWidth, CanvasHeight));
            DrawOutlinedTextFixed(context, request.Title, titlePosition.ToRect(), titleFontSize, 4, titleFont);
            DrawOutlinedTextFixed(context, request.Subtitle, subtitlePosition.ToRect(), subtitleFontSize, 2, subtitleFont);

            foreach (var element in imageElements)
            {
                if (string.IsNullOrEmpty(element.SourcePath)) continue;
                double scale = element.ItemImageRef?.Scale ?? 1.0;
                var scaledRect = GetScaledRectFromPosition(element, scale);
                int targetW = (int)Math.Round(scaledRect.Width);
                int targetH = (int)Math.Round(scaledRect.Height);
                if (targetW <= 0 || targetH <= 0) continue;

                var image = _imageProcessor.PrepareAndRemoveWhite(_imageLoader.LoadImageSharp(element.SourcePath), targetW, targetH);
                context.DrawImage(image, scaledRect.FitTo(image));
            }

            for (int i = 0; i < request.Captions.Count && i < captionPositions.Count; i++)
            {
                if (!string.IsNullOrWhiteSpace(request.Captions[i].Text))
                {
                    var captionFont = captionFonts != null && i < captionFonts.Count ? captionFonts[i] : _defaultFont;
                    DrawOutlinedTextFixed(context, request.Captions[i].Text, captionPositions[i].ToRect(), request.Captions[i].FontSize, 2, captionFont);
                }
            }

            if (!string.IsNullOrEmpty(request.Footer) && footerPosition != null)
                DrawOutlinedTextFixed(context, request.Footer, footerPosition.ToRect(), footerFontSize, 4, footerFont);

            foreach (var num in numberElements)
                DrawNumber(context, num.Number, num.X, num.Y, num.Scale);

            context.Close();
            bitmap.Render(visual);
            bitmap.Freeze();
            return bitmap;
        }

        private void DrawOutlinedText(DrawingContext ctx, string text, Rect area, double maxSize, double minSize, double stroke, Typeface font)
        {
            if (string.IsNullOrWhiteSpace(text)) return;
            FormattedText? ft = null;
            for (double size = maxSize; size >= minSize; size--)
            {
                ft = CreateFormattedText(text, size, area.Width, font);
                if (ft.Height <= area.Height) break;
                if (size == minSize) ft.Trimming = TextTrimming.CharacterEllipsis;
            }
            if (ft == null) return;
            var origin = new Point(area.X, area.Y + (area.Height - ft.Height) / 2);
            var geo = ft.BuildGeometry(origin);
            ctx.DrawGeometry(null, new Pen(Brushes.White, stroke), geo);
            ctx.DrawGeometry(Brushes.Black, null, geo);
        }

        private void DrawOutlinedTextFixed(DrawingContext ctx, string text, Rect area, double fontSize, double stroke, Typeface font)
        {
            if (string.IsNullOrWhiteSpace(text)) return;
            var ft = CreateFormattedText(text, fontSize, area.Width, font);
            ft.Trimming = TextTrimming.CharacterEllipsis;
            var origin = new Point(area.X, area.Y + (area.Height - ft.Height) / 2);
            var geo = ft.BuildGeometry(origin);
            ctx.DrawGeometry(null, new Pen(Brushes.White, stroke), geo);
            ctx.DrawGeometry(Brushes.Black, null, geo);
        }

        private void DrawNumber(DrawingContext ctx, int number, double x, double y, double scale)
        {
            var shadow = new FormattedText(number.ToString(), System.Globalization.CultureInfo.InvariantCulture,
                FlowDirection.LeftToRight, _defaultFont, 72 * scale, Brushes.Black, 1.0);
            var main = new FormattedText(number.ToString(), System.Globalization.CultureInfo.InvariantCulture,
                FlowDirection.LeftToRight, _defaultFont, 64 * scale, Brushes.White, 1.0);
            ctx.DrawText(shadow, new Point(x + 6, y));
            ctx.DrawText(main, new Point(x, y));
        }

        private static FormattedText CreateFormattedText(string text, double fontSize, double maxWidth, Typeface font) =>
            new(text, System.Globalization.CultureInfo.CurrentCulture, FlowDirection.LeftToRight, font, fontSize, Brushes.Black, 1.0)
            {
                MaxTextWidth = maxWidth,
                TextAlignment = TextAlignment.Center,
                Trimming = TextTrimming.None
            };

        private static Rect GetScaledRect(TemplateSlot slot, double scale)
        {
            double cx = slot.Bounds.X + slot.Bounds.Width / 2, cy = slot.Bounds.Y + slot.Bounds.Height / 2;
            double w = slot.Bounds.Width * scale, h = slot.Bounds.Height * scale;
            return new Rect(cx - w / 2, cy - h / 2, w, h);
        }

        private static Rect GetScaledRectFromPosition(EditorImageElement e, double scale)
        {
            double cx = e.X + e.Width / 2, cy = e.Y + e.Height / 2;
            double w = e.Width * scale, h = e.Height * scale;
            return new Rect(cx - w / 2, cy - h / 2, w, h);
        }
    }
}
