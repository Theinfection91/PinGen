using PinGen.Core.Models;
using PinGen.Templates.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;

namespace PinGen.Templates.Services
{
    public class HardcodedTemplateProvider : ITemplateProvider
    {
        public TemplateDefinition GetTemplate(int itemCount)
        {
            return new TemplateDefinition
            {
                Width = 1000,
                Height = 1500,
                TitleArea = new Rect(50, 50, 900, 100),
                FooterArea = new Rect(50, 1350, 900, 100),
                ImageSlots = new List<Rect>
                {
                    new Rect(50, 200, 400, 400),
                    new Rect(550, 200, 400, 400),
                    new Rect(50, 650, 400, 400),
                    new Rect(550, 650, 400, 400)
                },
                CaptionAreas = new List<Rect>
                {
                    new Rect(50, 610, 400, 30),
                    new Rect(550, 610, 400, 30),
                    new Rect(50, 1060, 400, 30),
                    new Rect(550, 1060, 400, 30)
                }
            };
        }
    }
}
