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
                        Bounds = new Rect(25, 280, 360, 420),
                        ShowNumber = true,
                        NumberArea = new Rect(275, 300, 80, 80)
                    },
                    new TemplateSlot
                    {
                        Bounds = new Rect(615, 280, 360, 420),
                        ShowNumber = true,
                        NumberArea = new Rect(865, 300, 80, 80)
                    },
                    new TemplateSlot
                    {
                        Bounds = new Rect(300, 540, 360, 520),
                        ShowNumber = true,
                        NumberArea = new Rect(620, 560, 90, 90)
                    },
                    new TemplateSlot
                    {
                        Bounds = new Rect(25, 900, 360, 420),
                        ShowNumber = true,
                        NumberArea = new Rect(275, 1000, 80, 80)
                    },
                    new TemplateSlot
                    {
                        Bounds = new Rect(615, 900, 360, 420),
                        ShowNumber = true,
                        NumberArea = new Rect(865, 1000, 80, 80)
                    },
                },
                CaptionAreas = new List<Rect>
                {
                    new Rect(25, 250, 925, 30),
                    new Rect(310, 720, 925, 30),
                    new Rect(25, 1300, 925, 30)
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
                        Bounds = new Rect(25, 300, 310, 470),
                        ShowNumber = true,
                        NumberArea = new Rect(250, 330, 85, 85)
                    },
                    new TemplateSlot
                    {
                        Bounds = new Rect(345, 300, 310, 470),
                        ShowNumber = true,
                        NumberArea = new Rect(570, 330, 85, 85)
                    },
                    new TemplateSlot
                    {
                        Bounds = new Rect(665, 300, 310, 470),
                        ShowNumber = true,
                        NumberArea = new Rect(890, 330, 85, 85)
                    },
                    new TemplateSlot
                    {
                        Bounds = new Rect(25, 720, 310, 470),
                        ShowNumber = true,
                        NumberArea = new Rect(250, 750, 85, 85)
                    },
                    new TemplateSlot
                    {
                        Bounds = new Rect(345, 720, 310, 470),
                        ShowNumber = true,
                        NumberArea = new Rect(570, 750, 85, 85)
                    },
                    new TemplateSlot
                    {
                        Bounds = new Rect(665, 720, 310, 470),
                        ShowNumber = true,
                        NumberArea = new Rect(890, 750, 85, 85)
                    },
                },
                CaptionAreas = new List<Rect>
                {
                    new Rect(25, 270, 925, 30),
                    new Rect(25, 690, 925, 30),
                    new Rect(25, 1195, 925, 30)
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
                        Bounds = new Rect(25, 300, 235, 400),
                        ShowNumber = true,
                        NumberArea = new Rect(380, 320, 80, 80)
                    },
                    new TemplateSlot
                    {
                        Bounds = new Rect(265, 300, 235, 400),
                        ShowNumber = true,
                        NumberArea = new Rect(620, 320, 80, 80)
                    },
                    new TemplateSlot
                    {
                        Bounds = new Rect(505, 300, 235, 400),
                        ShowNumber = true,
                        NumberArea = new Rect(860, 320, 80, 80)
                    },
                    new TemplateSlot
                    {
                        Bounds = new Rect(745, 300, 235, 400),
                        ShowNumber = true,
                        NumberArea = new Rect(900, 320, 80, 80)
                    },
                    new TemplateSlot
                    {
                        Bounds = new Rect(60, 740, 310, 400),
                        ShowNumber = true,
                        NumberArea = new Rect(280, 760, 90, 90)
                    },
                    new TemplateSlot
                    {
                        Bounds = new Rect(360, 740, 310, 400),
                        ShowNumber = true,
                        NumberArea = new Rect(580, 760, 90, 90)
                    },
                    new TemplateSlot
                    {
                        Bounds = new Rect(660, 740, 310, 400),
                        ShowNumber = true,
                        NumberArea = new Rect(880, 760, 90, 90)
                    },
                },
                CaptionAreas = new List<Rect>
                {
                    new Rect(25, 270, 925, 30),
                    new Rect(25, 710, 925, 30),
                    new Rect(25, 1145, 925, 30)
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
                        Bounds = new Rect(25, 300, 230, 380),
                        ShowNumber = true,
                        NumberArea = new Rect(170, 320, 85, 85)
                    },
                    new TemplateSlot
                    {
                        Bounds = new Rect(265, 300, 230, 380),
                        ShowNumber = true,
                        NumberArea = new Rect(410, 320, 85, 85)
                    },
                    new TemplateSlot
                    {
                        Bounds = new Rect(505, 300, 230, 380),
                        ShowNumber = true,
                        NumberArea = new Rect(650, 320, 85, 85)
                    },
                    new TemplateSlot
                    {
                        Bounds = new Rect(745, 300, 230, 380),
                        ShowNumber = true,
                        NumberArea = new Rect(890, 320, 85, 85)
                    },
                    new TemplateSlot
                    {
                        Bounds = new Rect(25, 700, 230, 380),
                        ShowNumber = true,
                        NumberArea = new Rect(170, 720, 85, 85)
                    },
                    new TemplateSlot
                    {
                        Bounds = new Rect(265, 700, 230, 380),
                        ShowNumber = true,
                        NumberArea = new Rect(410, 720, 85, 85)
                    },
                    new TemplateSlot
                    {
                        Bounds = new Rect(505, 700, 230, 380),
                        ShowNumber = true,
                        NumberArea = new Rect(650, 720, 85, 85)
                    },
                    new TemplateSlot
                    {
                        Bounds = new Rect(745, 700, 230, 380),
                        ShowNumber = true,
                        NumberArea = new Rect(890, 720, 85, 85)
                    },
                },
                CaptionAreas = new List<Rect>
                {
                    new Rect(25, 270, 925, 30),
                    new Rect(25, 670, 925, 30),
                    new Rect(25, 1100, 925, 30)
                }
            };
        }
    }
}