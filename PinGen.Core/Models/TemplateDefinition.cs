using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;

namespace PinGen.Core.Models
{
    public class TemplateDefinition
    {
        public int Width { get; set; }
        public int Height { get; set; }
        public Rect TitleArea { get; set; }
        public Rect? FooterArea { get; set; }
        public List<Rect> ImageSlots { get; set; } = new();
        public List<Rect> CaptionAreas { get; set; } = new();
    }
}
