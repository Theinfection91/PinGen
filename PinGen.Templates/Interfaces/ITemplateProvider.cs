using System;
using System.Collections.Generic;
using System.Text;
using PinGen.Core.Models;

namespace PinGen.Templates.Interfaces
{
    public interface ITemplateProvider
    {
        TemplateDefinition GetTemplate(int itemCount);
    }
}
