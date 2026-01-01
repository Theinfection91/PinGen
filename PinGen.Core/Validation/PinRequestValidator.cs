using PinGen.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace PinGen.Core.Validation
{
    public static class PinRequestValidator
    {
        public static bool Validate(PinRequest request)
        {
            // Basic validation checks
            if (string.IsNullOrWhiteSpace(request.Title))
                return false;

            if (string.IsNullOrWhiteSpace(request.Subtitle))
                return false;

            foreach (var caption in request.Captions)
            {
                if (string.IsNullOrWhiteSpace(caption))
                    return false;
            }

            if (request.ItemImages == null || request.ItemImages.Count == 0)
                return false;

            foreach (var item in request.ItemImages)
            {
                if (string.IsNullOrWhiteSpace(item.SourcePath))
                    return false;
            }

            return true;
        }
    }
}
