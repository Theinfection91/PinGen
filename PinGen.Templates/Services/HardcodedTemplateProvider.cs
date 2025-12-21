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
                    new Rect(250, 740, 925, 30),
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
                    new Rect(580, 720, 400, 30),
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
                        Bounds = new Rect(25, 300, 310, 450),
                        ShowNumber = true,
                        NumberArea = new Rect(250, 330, 85, 85)
                    },
                    new TemplateSlot
                    {
                        Bounds = new Rect(345, 300, 310, 450),
                        ShowNumber = true,
                        NumberArea = new Rect(570, 330, 85, 85)
                    },
                    new TemplateSlot
                    {
                        Bounds = new Rect(665, 300, 310, 450),
                        ShowNumber = true,
                        NumberArea = new Rect(890, 330, 85, 85)
                    },
                    new TemplateSlot
                    {
                        Bounds = new Rect(25, 810, 310, 470),
                        ShowNumber = true,
                        NumberArea = new Rect(250, 840, 85, 85)
                    },
                    new TemplateSlot
                    {
                        Bounds = new Rect(345, 810, 310, 470),
                        ShowNumber = true,
                        NumberArea = new Rect(570, 840, 85, 85)
                    },
                    new TemplateSlot
                    {
                        Bounds = new Rect(665, 810, 310, 470),
                        ShowNumber = true,
                        NumberArea = new Rect(890, 840, 85, 85)
                    },
                },
                CaptionAreas = new List<Rect>
                {
                    new Rect(25, 270, 925, 30),
                    new Rect(25, 770, 925, 30),
                    new Rect(25, 1300, 925, 30)
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
                        Bounds = new Rect(55, 310, 280, 410),
                        ShowNumber = true,
                        NumberArea = new Rect(255, 330, 70, 70)
                    },
                    new TemplateSlot
                    {
                        Bounds = new Rect(345, 310, 280, 410),
                        ShowNumber = true,
                        NumberArea = new Rect(545, 330, 70, 70)
                    },
                    new TemplateSlot
                    {
                        Bounds = new Rect(635, 310, 280, 410),
                        ShowNumber = true,
                        NumberArea = new Rect(835, 330, 70, 70)
                    },
                    new TemplateSlot
                    {
                        Bounds = new Rect(25, 780, 230, 500),
                        ShowNumber = true,
                        NumberArea = new Rect(185, 800, 70, 70)
                    },
                    new TemplateSlot
                    {
                        Bounds = new Rect(265, 780, 230, 500),
                        ShowNumber = true,
                        NumberArea = new Rect(425, 800, 70, 70)
                    },
                    new TemplateSlot
                    {
                        Bounds = new Rect(505, 780, 230, 500),
                        ShowNumber = true,
                        NumberArea = new Rect(665, 800, 70, 70)
                    },
                    new TemplateSlot
                    {
                        Bounds = new Rect(745, 780, 230, 500),
                        ShowNumber = true,
                        NumberArea = new Rect(905, 800, 70, 70)
                    },
                },
                CaptionAreas = new List<Rect>
                {
                    new Rect(25, 270, 925, 30),
                    new Rect(25, 740, 925, 30),
                    new Rect(25, 1300, 925, 30)
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
                        Bounds = new Rect(25, 320, 225, 420),
                        ShowNumber = true,
                        NumberArea = new Rect(180, 340, 70, 70)
                    },
                    new TemplateSlot
                    {
                        Bounds = new Rect(265, 320, 225, 420),
                        ShowNumber = true,
                        NumberArea = new Rect(420, 340, 70, 70)
                    },
                    new TemplateSlot
                    {
                        Bounds = new Rect(505, 320, 225, 420),
                        ShowNumber = true,
                        NumberArea = new Rect(660, 340, 70, 70)
                    },
                    new TemplateSlot
                    {
                        Bounds = new Rect(745, 320, 225, 420),
                        ShowNumber = true,
                        NumberArea = new Rect(900, 340, 70, 70)
                    },
                    new TemplateSlot
                    {
                        Bounds = new Rect(25, 860, 225, 420),
                        ShowNumber = true,
                        NumberArea = new Rect(180, 880, 70, 70)
                    },
                    new TemplateSlot
                    {
                        Bounds = new Rect(265, 860, 225, 420),
                        ShowNumber = true,
                        NumberArea = new Rect(420, 880, 70, 70)
                    },
                    new TemplateSlot
                    {
                        Bounds = new Rect(505, 860, 225, 420),
                        ShowNumber = true,
                        NumberArea = new Rect(660, 880, 70, 70)
                    },
                    new TemplateSlot
                    {
                        Bounds = new Rect(745, 860, 225, 420),
                        ShowNumber = true,
                        NumberArea = new Rect(900, 880, 70, 70)
                    },
                },
                CaptionAreas = new List<Rect>
                {
                    new Rect(25, 270, 925, 30),
                    new Rect(25, 780, 925, 30),
                    new Rect(25, 1300, 925, 30)
                }
            };
        }
    }
}