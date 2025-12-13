using PinGen.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace PinGen.Core.Validation
{
    public static class PinRequestValidator
    {
        public static void Validate(PinRequest request)
        {
            // Basic validation checks
            if (string.IsNullOrWhiteSpace(request.Title))
                throw new ArgumentException("Pin must have a title.");

            foreach (var item in request.ItemImages)
            {
                if (string.IsNullOrWhiteSpace(item.SourcePath))
                    throw new ArgumentException("Each image must have a valid source path.");
            }
        }
    }
}
