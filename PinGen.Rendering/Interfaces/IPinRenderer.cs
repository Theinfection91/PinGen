using PinGen.Core.Models;
using System.Collections.Generic;
using System.Windows.Media.Imaging;

namespace PinGen.Rendering.Interfaces
{
    public interface IPinRenderer
    {
        RenderTargetBitmap Render(PinRequest request, TemplateDefinition template);
        RenderTargetBitmap Render(PinRequest request, TemplateDefinition template, List<double> yOffsets);
        RenderTargetBitmap Render(PinRequest request, TemplateDefinition template, string backgroundPath);
        RenderTargetBitmap Render(PinRequest request, TemplateDefinition template, string backgroundPath, List<double> yOffsets);
        
        RenderTargetBitmap RenderWithEditorPositions(
            PinRequest request,
            string backgroundPath,
            ElementPosition titlePosition,
            ElementPosition subtitlePosition,
            ElementPosition? footerPosition,
            List<ElementPosition> captionPositions,
            List<EditorImageElement> imageElements,
            List<EditorNumberElement> numberElements,
            double titleFontSize = 64,
            double subtitleFontSize = 32,
            double footerFontSize = 48);
    }
}
