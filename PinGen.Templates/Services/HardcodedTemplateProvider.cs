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

        /*
        Plan:
        1. Extend GetTemplate switch to route counts 7 and 8 to newly implemented templates.
        2. Create GetSevenItemTemplate with 215x330 slots arranged as a 4-by-3 grid (top row of four, second row of three) to keep margins even, plus repositioned number badges.
        3. Create GetEightItemTemplate with two rows of four slots each using the same compact slot size to support an eight-image layout.
        4. Provide caption rectangles beneath the title and each slot row to maintain consistent descriptive space.
        */

        public TemplateDefinition GetTemplate(int itemCount)
        {
            switch (itemCount)
            {
                case 4:
                    return GetFourItemTemplate();
                case 5:
                    return GetFiveItemTemplate();
                case 6:
                    return GetSixItemTemplate();
                case 7:
                    return GetSevenItemTemplate();
                case 8:
                    return GetEightItemTemplate();
                default:
                    throw new NotImplementedException($"No hardcoded template for {itemCount} items.");
            }
        }

        public TemplateDefinition GetFourItemTemplate()
        {
            return new TemplateDefinition
            {
                TitleArea = _defaultTitleArea,
                SubtitleArea = _defaultSubtitleArea,
                FooterArea = _defaultFooterArea,
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
                    new Rect(525, 740, 360, 30),
                    new Rect(25, 1300, 925, 30)
                }
            };
        }

        public TemplateDefinition GetFiveItemTemplate()
        {
            return new TemplateDefinition
            {
                TitleArea = _defaultTitleArea,
                SubtitleArea = _defaultSubtitleArea,
                FooterArea = _defaultFooterArea,
                TemplateSlots = new List<TemplateSlot>
                {
                    new TemplateSlot
                    {
                        Bounds = new Rect(25, 300, 300, 400),
                        ShowNumber = true,
                        NumberArea = new Rect(235, 320, 70, 70)
                    },
                    new TemplateSlot
                    {
                        Bounds = new Rect(335, 300, 300, 400),
                        ShowNumber = true,
                        NumberArea = new Rect(545, 320, 70, 70)
                    },
                    new TemplateSlot
                    {
                        Bounds = new Rect(645, 300, 300, 400),
                        ShowNumber = true,
                        NumberArea = new Rect(855, 320, 70, 70)
                    },
                    new TemplateSlot
                    {
                        Bounds = new Rect(25, 750, 450, 430),
                        ShowNumber = true,
                        NumberArea = new Rect(375, 770, 70, 70)
                    },
                    new TemplateSlot
                    {
                        Bounds = new Rect(525, 750, 450, 430),
                        ShowNumber = true,
                        NumberArea = new Rect(875, 770, 70, 70)
                    },
                },
                CaptionAreas = new List<Rect>
                {
                    new Rect(25, 270, 925, 30),
                    new Rect(25, 710, 925, 30),
                    new Rect(25, 1190, 925, 30),
                    new Rect(25, 1240, 925, 30)
                }
            };
        }

        public TemplateDefinition GetSixItemTemplate()
        {
            return new TemplateDefinition
            {
                TitleArea = _defaultTitleArea,
                SubtitleArea = _defaultSubtitleArea,
                FooterArea = _defaultFooterArea,
                TemplateSlots = new List<TemplateSlot>
                {
                    new TemplateSlot
                    {
                        Bounds = new Rect(25, 300, 300, 420),
                        ShowNumber = true,
                        NumberArea = new Rect(235, 330, 70, 70)
                    },
                    new TemplateSlot
                    {
                        Bounds = new Rect(345, 300, 300, 420),
                        ShowNumber = true,
                        NumberArea = new Rect(555, 330, 70, 70)
                    },
                    new TemplateSlot
                    {
                        Bounds = new Rect(665, 300, 300, 420),
                        ShowNumber = true,
                        NumberArea = new Rect(875, 330, 70, 70)
                    },
                    new TemplateSlot
                    {
                        Bounds = new Rect(25, 780, 300, 420),
                        ShowNumber = true,
                        NumberArea = new Rect(235, 810, 70, 70)
                    },
                    new TemplateSlot
                    {
                        Bounds = new Rect(345, 780, 300, 420),
                        ShowNumber = true,
                        NumberArea = new Rect(555, 810, 70, 70)
                    },
                    new TemplateSlot
                    {
                        Bounds = new Rect(665, 780, 300, 420),
                        ShowNumber = true,
                        NumberArea = new Rect(875, 810, 70, 70)
                    },
                },
                CaptionAreas = new List<Rect>
                {
                    new Rect(25, 270, 925, 30),
                    new Rect(25, 730, 925, 30),
                    new Rect(25, 1210, 925, 30)
                }
            };
        }

        public TemplateDefinition GetSevenItemTemplate()
        {
            return new TemplateDefinition
            {
                TitleArea = _defaultTitleArea,
                SubtitleArea = _defaultSubtitleArea,
                FooterArea = _defaultFooterArea,
                TemplateSlots = new List<TemplateSlot>
                {
                    new TemplateSlot
                    {
                        Bounds = new Rect(25, 300, 220, 360),
                        ShowNumber = true,
                        NumberArea = new Rect(165, 320, 60, 60)
                    },
                    new TemplateSlot
                    {
                        Bounds = new Rect(255, 300, 220, 360),
                        ShowNumber = true,
                        NumberArea = new Rect(395, 320, 60, 60)
                    },
                    new TemplateSlot
                    {
                        Bounds = new Rect(485, 300, 220, 360),
                        ShowNumber = true,
                        NumberArea = new Rect(625, 320, 60, 60)
                    },
                    new TemplateSlot
                    {
                        Bounds = new Rect(715, 300, 220, 360),
                        ShowNumber = true,
                        NumberArea = new Rect(855, 320, 60, 60)
                    },
                    new TemplateSlot
                    {
                        Bounds = new Rect(135, 730, 260, 360),
                        ShowNumber = true,
                        NumberArea = new Rect(315, 750, 60, 60)
                    },
                    new TemplateSlot
                    {
                        Bounds = new Rect(405, 730, 260, 360),
                        ShowNumber = true,
                        NumberArea = new Rect(585, 750, 60, 60)
                    },
                    new TemplateSlot
                    {
                        Bounds = new Rect(675, 730, 260, 360),
                        ShowNumber = true,
                        NumberArea = new Rect(855, 750, 60, 60)
                    },
                },
                CaptionAreas = new List<Rect>
                {
                    new Rect(25, 270, 925, 30),
                    new Rect(25, 670, 925, 30),
                    new Rect(25, 1100, 925, 30),
                    new Rect(25, 1170, 925, 30)
                }
            };
        }

        public TemplateDefinition GetEightItemTemplate()
        {
            return new TemplateDefinition
            {
                TitleArea = _defaultTitleArea,
                SubtitleArea = _defaultSubtitleArea,
                FooterArea = _defaultFooterArea,
                TemplateSlots = new List<TemplateSlot>
                {
                    new TemplateSlot
                    {
                        Bounds = new Rect(25, 300, 220, 360),
                        ShowNumber = true,
                        NumberArea = new Rect(165, 320, 60, 60)
                    },
                    new TemplateSlot
                    {
                        Bounds = new Rect(255, 300, 220, 360),
                        ShowNumber = true,
                        NumberArea = new Rect(395, 320, 60, 60)
                    },
                    new TemplateSlot
                    {
                        Bounds = new Rect(485, 300, 220, 360),
                        ShowNumber = true,
                        NumberArea = new Rect(625, 320, 60, 60)
                    },
                    new TemplateSlot
                    {
                        Bounds = new Rect(715, 300, 220, 360),
                        ShowNumber = true,
                        NumberArea = new Rect(855, 320, 60, 60)
                    },
                    new TemplateSlot
                    {
                        Bounds = new Rect(25, 730, 220, 360),
                        ShowNumber = true,
                        NumberArea = new Rect(165, 750, 60, 60)
                    },
                    new TemplateSlot
                    {
                        Bounds = new Rect(255, 730, 220, 360),
                        ShowNumber = true,
                        NumberArea = new Rect(395, 750, 60, 60)
                    },
                    new TemplateSlot
                    {
                        Bounds = new Rect(485, 730, 220, 360),
                        ShowNumber = true,
                        NumberArea = new Rect(625, 750, 60, 60)
                    },
                    new TemplateSlot
                    {
                        Bounds = new Rect(715, 730, 220, 360),
                        ShowNumber = true,
                        NumberArea = new Rect(855, 750, 60, 60)
                    },
                },
                CaptionAreas = new List<Rect>
                {
                    new Rect(25, 270, 925, 30),
                    new Rect(25, 670, 925, 30),
                    new Rect(25, 1100, 925, 30),
                    new Rect(25, 1140, 925, 30)
                }
            };
        }
    }
}
                        