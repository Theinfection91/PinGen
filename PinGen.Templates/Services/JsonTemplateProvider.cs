using PinGen.Core.Models;
using PinGen.Templates.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace PinGen.Templates.Services
{
    public class JsonTemplateProvider : ITemplateProvider
    {
        public TemplateDefinition GetTemplate(int itemCount)
        {
            // Implementation to read and parse a JSON template based on the itemCount
            return new TemplateDefinition();
        }

        public TemplateDefinition GetTemplate(int itemCount, bool hasFooter)
        {
            // Implementation to read and parse a JSON template based on the itemCount and hasFooter
            return new TemplateDefinition();
        }
    }
}
