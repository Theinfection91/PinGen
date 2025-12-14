using PinGen.Core.Models;
using PinGen.Templates.Interfaces;
using System.Collections.Generic;
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

                // Title and Footer dimensions were measured from example of what output should look like and will likely be the standard for most templates
                TitleArea = new Rect(25, 30, 950, 275),
                FooterArea = new Rect(25, 1325, 950, 140),

                TemplateSlots = new List<TemplateSlot>
                {
                    new TemplateSlot
                    {
                        Bounds = new Rect(30, 310, 400, 350),
                        ShowNumber = true,
                        NumberArea = new Rect(310, 370, 80, 80)
                    },
                    new TemplateSlot
                    {
                        Bounds = new Rect(575, 350, 400, 350),
                        ShowNumber = true,
                        NumberArea = new Rect(825, 525, 80, 80)
                    },
                    new TemplateSlot
                    {
                        Bounds = new Rect(320, 590, 400, 350),
                        ShowNumber = true,
                        NumberArea = new Rect(370, 635, 80, 80)
                    },
                    new TemplateSlot
                    {
                        Bounds = new Rect(40, 865, 400, 350),
                        ShowNumber = true,
                        NumberArea = new Rect(290, 1045, 80, 80)
                    },
                    new TemplateSlot
                    {
                        Bounds = new Rect(560, 765, 400, 350),
                        ShowNumber = true,
                        NumberArea = new Rect(810, 845, 80, 80)
                    },
                },

                CaptionAreas = new List<Rect>
                {
                    new Rect(80, 690, 380, 30),
                    new Rect(620, 660, 360, 30),
                    new Rect(280, 1230, 360, 30)
                }
            };
        }
    }
}
