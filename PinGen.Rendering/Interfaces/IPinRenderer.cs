using PinGen.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Media.Imaging;

namespace PinGen.Rendering.Interfaces
{
    public interface IPinRenderer
    {
        RenderTargetBitmap Render(PinRequest request, TemplateDefinition template);
        RenderTargetBitmap Render(PinRequest request, TemplateDefinition template, List<double> yOffsets);
        RenderTargetBitmap Render(PinRequest request, TemplateDefinition template, string backgroundPath);
        RenderTargetBitmap Render(PinRequest request, TemplateDefinition template, string backgroundPath, List<double> yOffsets);
    }
}
