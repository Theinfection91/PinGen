using PinGen.Core.Models;
using PinGen.Templates.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace PinGen.Templates.Services
{
    public class HardcodedTemplateProvider : ITemplateProvider
    {
        public TemplateDefinition GetTemplate(int itemCount)
        {
            // Return a hardcoded template definition based on the itemCount
            return new TemplateDefinition();
        }
    }
}
