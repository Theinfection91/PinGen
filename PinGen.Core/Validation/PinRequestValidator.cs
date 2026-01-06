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
