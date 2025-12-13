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

                TitleArea = new Rect(60, 40, 880, 100),
                FooterArea = new Rect(60, 1360, 880, 80),

                TemplateSlots = new List<TemplateSlot>
                {
                    new TemplateSlot
                    {
                        Bounds = new Rect(80, 200, 380, 380),
                        ShowNumber = true,
                        NumberArea = new Rect(80, 200, 60, 60)
                    },
                    new TemplateSlot
                    {
                        Bounds = new Rect(520, 240, 360, 360),
                        ShowNumber = true,
                        NumberArea = new Rect(520, 240, 60, 60)
                    },
                    new TemplateSlot
                    {
                        Bounds = new Rect(120, 650, 360, 360),
                        ShowNumber = true,
                        NumberArea = new Rect(120, 650, 60, 60)
                    },
                    new TemplateSlot
                    {
                        Bounds = new Rect(560, 700, 320, 320),
                        ShowNumber = true,
                        NumberArea = new Rect(560, 700, 60, 60)
                    }
                },

                CaptionAreas = new List<Rect>
                {
                    new Rect(80, 590, 380, 30),
                    new Rect(520, 610, 360, 30),
                    new Rect(120, 1020, 360, 30),
                    new Rect(560, 1030, 320, 30)
                }
            };
        }
    }
}
