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
        private readonly IImageLoader _imageLoader;
        private readonly IImageProcessor _imageProcessor;

        public PinRenderer(IImageLoader imageLoader, IImageProcessor imageProcessor)
        {
            _imageLoader = imageLoader;
            _imageProcessor = imageProcessor;
        }

        //public RenderTargetBitmap Render(PinRequest request, TemplateDefinition template)
        //{
        //    var bitmap = new RenderTargetBitmap(template.Width, template.Height, 96, 96, PixelFormats.Pbgra32);
        //    var visual = new DrawingVisual();
        //    // Setup Render Options
        //    RenderOptions.SetBitmapScalingMode(visual, BitmapScalingMode.HighQuality);
        //    RenderOptions.SetEdgeMode(visual, EdgeMode.Un);

        //    using var context = visual.RenderOpen();

        //    // Draw background dynamically
        //    //string appBase = AppDomain.CurrentDomain.BaseDirectory;
        //    //string bgPath = Path.Combine(appBase, "Assets", "Backgrounds", "bg1.png");
        //    string bgPath = @"C:\Chase\CSharpProjects\PinGen\PinGen.App\Assets\Backgrounds\bg1.png";
        //    var background = _imageLoader.Load(bgPath);
        //    context.DrawImage(background, new Rect(0, 0, template.Width, template.Height));

        //    // Draw title with black fill and white outline
        //    var titleText = new FormattedText(
        //        request.Title,
        //        System.Globalization.CultureInfo.CurrentCulture,
        //        FlowDirection.LeftToRight,
        //        new Typeface("Arial"),
        //        48,
        //        Brushes.Black,
        //        1.0);

        //    // Convert text to geometry
        //    var textGeometry = titleText.BuildGeometry(new Point(template.TitleArea.X, template.TitleArea.Y));

        //    // Draw white outline
        //    context.DrawGeometry(null, new Pen(Brushes.White, 4), textGeometry);

        //    // Draw black fill on top
        //    context.DrawGeometry(Brushes.Black, null, textGeometry);

        //    // Draw images
        //    for (int i = 0; i < request.ItemImages.Count && i < template.TemplateSlots.Count; i++)
        //    {
        //        var slot = template.TemplateSlots[i];

        //        //var image = _imageLoader.Load(request.ItemImages[i].SourcePath);

        //        // Use image processor to remove white background
        //        var image = _imageProcessor.RemoveWhiteBackground(request.ItemImages[i].SourcePath);

        //        // Fit image into slot
        //        context.DrawImage(image, slot.Bounds.FitTo(image));

        //        // Draw number overlay if enabled
        //        if (slot.ShowNumber && slot.NumberArea.HasValue)
        //        {
        //            var numberArea = slot.NumberArea.Value;

        //            // Circle background
        //            context.DrawEllipse(
        //                Brushes.Black,
        //                null,
        //                new Point(
        //                    numberArea.X + numberArea.Width / 2,
        //                    numberArea.Y + numberArea.Height / 2),
        //                numberArea.Width / 2,
        //                numberArea.Height / 2);

        //            var numberText = new FormattedText(
        //                (i + 1).ToString(),
        //                System.Globalization.CultureInfo.InvariantCulture,
        //                FlowDirection.LeftToRight,
        //                new Typeface(new FontFamily("Arial"), FontStyles.Normal, FontWeights.Bold, FontStretches.Normal),
        //                28,
        //                Brushes.White,
        //                1.0);

        //            context.DrawText(
        //                numberText,
        //                new Point(
        //                    numberArea.X + (numberArea.Width - numberText.Width) / 2,
        //                    numberArea.Y + (numberArea.Height - numberText.Height) / 2));
        //        }
        //    }

        //    // Draw captions with black fill and white outline
        //    for (int i = 0; i < request.Captions.Count && i < template.CaptionAreas.Count; i++)
        //    {
        //        var area = template.CaptionAreas[i];
        //        var captionText = new FormattedText(
        //            request.Captions[i],
        //            System.Globalization.CultureInfo.CurrentCulture,
        //            FlowDirection.LeftToRight,
        //            new Typeface("Arial"),
        //            24,
        //            Brushes.Black,
        //            1.0);
        //        var captionGeometry = captionText.BuildGeometry(new Point(area.X, area.Y));
        //        // Draw white outline
        //        context.DrawGeometry(null, new Pen(Brushes.White, 2), captionGeometry);
        //        // Draw black fill on top
        //        context.DrawGeometry(Brushes.Black, null, captionGeometry);
        //    }

        //    // Draw footer if exists with black fill and white outline
        //    if (!string.IsNullOrEmpty(request.Footer) && template.FooterArea.HasValue)
        //    {
        //        var footerArea = template.FooterArea.Value;
        //        var footerText = new FormattedText(
        //            request.Footer,
        //            System.Globalization.CultureInfo.CurrentCulture,
        //            FlowDirection.LeftToRight,
        //            new Typeface("Arial"),
        //            48,
        //            Brushes.Black,
        //            1.0);
        //        var footerGeometry = footerText.BuildGeometry(new Point(footerArea.X, footerArea.Y));
        //        // Draw white outline
        //        context.DrawGeometry(null, new Pen(Brushes.White, 4), footerGeometry);
        //        // Draw black fill on top
        //        context.DrawGeometry(Brushes.Black, null, footerGeometry);
        //    }

        //    context.Close();
        //    bitmap.Render(visual);
        //    bitmap.Freeze();
        //    return bitmap;
        //}

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
            var background = _imageLoader.Load(@"C:\Chase\CSharpProjects\PinGen\PinGen.App\Assets\Backgrounds\bg1.png");
            context.DrawImage(background, new Rect(0, 0, template.Width, template.Height));

            // Title (black fill + white stroke)
            DrawOutlinedText(
                context,
                request.Title,
                template.TitleArea.X,
                template.TitleArea.Y,
                48,
                Brushes.Black,
                Brushes.White,
                4);

            // Images
            for (int i = 0; i < request.ItemImages.Count && i < template.TemplateSlots.Count; i++)
            {
                var slot = template.TemplateSlots[i];

                var fitted = slot.Bounds;
                int targetW = (int)Math.Round(fitted.Width);
                int targetH = (int)Math.Round(fitted.Height);

                var image = _imageProcessor.LoadPrepareAndRemoveWhite(request.ItemImages[i].SourcePath, targetW, targetH);

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
                        new Typeface(new FontFamily("Arial"), FontStyles.Normal, FontWeights.Bold, FontStretches.Normal),
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

            // Draw captions with black fill and white outline
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
                var captionGeometry = captionText.BuildGeometry(new Point(area.X, area.Y));
                // Draw white outline
                context.DrawGeometry(null, new Pen(Brushes.White, 2), captionGeometry);
                // Draw black fill on top
                context.DrawGeometry(Brushes.Black, null, captionGeometry);
            }

            // Draw footer if exists with black fill and white outline
            if (!string.IsNullOrEmpty(request.Footer) && template.FooterArea.HasValue)
            {
                var footerArea = template.FooterArea.Value;
                var footerText = new FormattedText(
                    request.Footer,
                    System.Globalization.CultureInfo.CurrentCulture,
                    FlowDirection.LeftToRight,
                    new Typeface("Arial"),
                    48,
                    Brushes.Black,
                    1.0);
                var footerGeometry = footerText.BuildGeometry(new Point(footerArea.X, footerArea.Y));
                // Draw white outline
                context.DrawGeometry(null, new Pen(Brushes.White, 4), footerGeometry);
                // Draw black fill on top
                context.DrawGeometry(Brushes.Black, null, footerGeometry);
            }

            context.Close();
            bitmap.Render(visual);
            bitmap.Freeze();
            return bitmap;
        }


        private static void DrawOutlinedText(DrawingContext ctx, string text, double x, double y, double size, Brush fill, Brush stroke, double strokeWidth)
        {
            var ft = new FormattedText(
                text,
                System.Globalization.CultureInfo.CurrentCulture,
                FlowDirection.LeftToRight,
                new Typeface("Arial"),
                size,
                fill,
                1.0);

            var geo = ft.BuildGeometry(new Point(x, y));

            ctx.DrawGeometry(null, new Pen(stroke, strokeWidth), geo);
            ctx.DrawGeometry(fill, null, geo);
        }

    }
}
