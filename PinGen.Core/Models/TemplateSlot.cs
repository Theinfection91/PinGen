using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;

namespace PinGen.Core.Models
{
    public class TemplateSlot
    {
        public Rect Bounds { get; set; }
        public double ImageScale { get; set; } = 1.0;
        public bool ShowNumber { get; set; }
        public Rect? NumberArea { get; set; }
    }
}
