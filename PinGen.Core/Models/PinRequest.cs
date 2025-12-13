using System;
using System.Collections.Generic;
using System.Text;

namespace PinGen.Core.Models
{
    public class PinRequest
    {
        public string Title { get; set; } = string.Empty;
        public List<ItemImage> ItemImages { get; set; } = new();
        public List<string> Captions { get; set; } = new();
        public string? Footer { get; set; }
        public string? TemplateKey { get; set; }
    }
}
