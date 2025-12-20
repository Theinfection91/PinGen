using PinGen.Core.Models;
using PinGen.Templates.Interfaces;
using System.Collections.Generic;
using System.Windows;

namespace PinGen.Templates.Services
{
    public class HardcodedTemplateProvider : ITemplateProvider
    {
        private Rect _defaultTitleArea = new Rect(25, 30, 950, 125);
        private Rect _defaultSubtitleArea = new Rect(25, 120, 950, 150);
        private Rect _defaultFooterArea = new Rect(25, 1325, 950, 140);

        public TemplateDefinition GetTemplate(int itemCount)
        {
            switch (itemCount)
            {
                case 4:
                    return GetFourItemTemplate();
                case 5:
                    return GetFiveItemTemplate();
                default:
                    throw new NotImplementedException($"No hardcoded template for {itemCount} items.");
            }
        }

        public TemplateDefinition GetFourItemTemplate()
        {
            return new TemplateDefinition
            {
                // Title and Footer dimensions were measured from example of what output should look like and will likely be the standard for most templates
                TitleArea = _defaultTitleArea,
                SubtitleArea = _defaultSubtitleArea,
                FooterArea = _defaultFooterArea,

                // Four items in a 2x2 grid
                TemplateSlots = new List<TemplateSlot>
                {
                    new TemplateSlot
                    {
                        Bounds = new Rect(100, 300, 400, 500),
                        ShowNumber = true,
                        NumberArea = new Rect(370, 370, 80, 80)
                    },
                    new TemplateSlot
                    {
                        Bounds = new Rect(550, 300, 400, 500),
                        ShowNumber = true,
                        NumberArea = new Rect(820, 375, 80, 80)
                    },
                    new TemplateSlot
                    {
                        Bounds = new Rect(100, 800, 400, 500),
                        ShowNumber = true,
                        NumberArea = new Rect(370, 870, 80, 80)
                    },
                    new TemplateSlot
                    {
                        Bounds = new Rect(550, 800, 400, 500),
                        ShowNumber = true,
                        NumberArea = new Rect(820, 875, 80, 80)
                    },
                },
                CaptionAreas = new List<Rect>
                {
                    new Rect(25, 270, 925, 30),
                    // Under second template slot
                    new Rect(525, 740, 360, 30),
                    new Rect(25, 1300, 925, 30)
                }
            };
        }

        public TemplateDefinition GetFiveItemTemplate()
        {
            return new TemplateDefinition
            {
                // Title and Footer dimensions were measured from example of what output should look like and will likely be the standard for most templates
                TitleArea = _defaultTitleArea,
                SubtitleArea = _defaultSubtitleArea,
                FooterArea = _defaultFooterArea,

                TemplateSlots = new List<TemplateSlot>
                {
                    new TemplateSlot
                    {
                        Bounds = new Rect(30, 310, 400, 500),
                        ShowNumber = true,
                        NumberArea = new Rect(310, 370, 80, 80)
                    },
                    new TemplateSlot
                    {
                        Bounds = new Rect(575, 300, 400, 500),
                        ShowNumber = true,
                        NumberArea = new Rect(600, 375, 80, 80)
                    },
                    new TemplateSlot
                    {
                        Bounds = new Rect(320, 650, 400, 500),
                        ShowNumber = true,
                        NumberArea = new Rect(370, 635, 80, 80)
                    },
                    new TemplateSlot
                    {
                        Bounds = new Rect(40, 950, 400, 500),
                        ShowNumber = true,
                        NumberArea = new Rect(410, 1070, 80, 80)
                    },
                    new TemplateSlot
                    {
                        Bounds = new Rect(560, 850, 400, 500),
                        ShowNumber = true,
                        NumberArea = new Rect(810, 845, 80, 80)
                    },
                },

                CaptionAreas = new List<Rect>
                {
                    new Rect(25, 270, 925, 30),
                    new Rect(25, 940, 360, 30),
                    new Rect(530, 1270, 360, 30)
                }
            };
        }
    }
}
