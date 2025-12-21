using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace PinGen.Core.Models
{
    public class ItemImage
    {
        public string SourcePath { get; set; } = string.Empty;
        public string FileName => Path.GetFileName(SourcePath);
        public string? AltText { get; set; }
    }
}
