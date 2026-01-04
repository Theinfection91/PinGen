using PinGen.IO.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Media.Imaging;

namespace PinGen.IO.Services
{
    public class FileSaver : IFileSaver
    {
        // Windows invalid filename characters: \ / : * ? " < > |
        private static readonly Regex InvalidFileNameChars = new Regex(
            @"[\\/:*?""<>|]",
            RegexOptions.Compiled);

        public void Save(BitmapSource bitmap, string path)
        {
            // Save the BitmapSource as a PNG file
            var encoder = new PngBitmapEncoder();

            // Add the frame to the encoder
            encoder.Frames.Add(BitmapFrame.Create(bitmap));

            // Write the encoded image to the specified path
            using var fileStream = new FileStream(path, FileMode.Create);
            encoder.Save(fileStream);
        }

        public string SanitizeFileName(string title, int maxLength = 200)
        {
            if (string.IsNullOrWhiteSpace(title))
                return "output";

            // Remove invalid characters
            var sanitized = InvalidFileNameChars.Replace(title, "");

            // Remove leading/trailing whitespace and periods (Windows doesn't like trailing periods)
            sanitized = sanitized.Trim().TrimEnd('.');

            // Replace multiple spaces with single space
            sanitized = Regex.Replace(sanitized, @"\s+", " ");

            // Truncate to maxLength (leave room for number suffix and extension)
            // maxLength - 1 for the number suffix (1 or 2)
            if (sanitized.Length > maxLength)
                sanitized = sanitized.Substring(0, maxLength);

            // Fallback if everything was stripped
            if (string.IsNullOrWhiteSpace(sanitized))
                return "output";

            return sanitized;
        }
    }
}
