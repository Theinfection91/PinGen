using System;
using System.ComponentModel;
using System.IO;
using System.Windows.Media.Imaging;

namespace PinGen.Core.Models
{
    /// <summary>
    /// Represents a draggable image element in the editor canvas.
    /// </summary>
    public class EditorImageElement : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged(string name)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

        private int _slotNumber;
        private string _elementName = string.Empty;
        private double _x;
        private double _y;
        private double _width;
        private double _height;
        private string _sourcePath = string.Empty;
        private BitmapSource? _thumbnailSource;

        /// <summary>
        /// Callback to process image with white removal. Set by ViewModel.
        /// </summary>
        public Func<string, int, int, BitmapSource?>? ProcessImageCallback { get; set; }

        public int SlotNumber
        {
            get => _slotNumber;
            set { _slotNumber = value; OnPropertyChanged(nameof(SlotNumber)); }
        }

        public string ElementName
        {
            get => _elementName;
            set { _elementName = value; OnPropertyChanged(nameof(ElementName)); }
        }

        public double X
        {
            get => _x;
            set { _x = value; OnPropertyChanged(nameof(X)); }
        }

        public double Y
        {
            get => _y;
            set { _y = value; OnPropertyChanged(nameof(Y)); }
        }

        public double Width
        {
            get => _width;
            set { _width = value; OnPropertyChanged(nameof(Width)); }
        }

        public double Height
        {
            get => _height;
            set { _height = value; OnPropertyChanged(nameof(Height)); }
        }

        public string SourcePath
        {
            get => _sourcePath;
            set
            {
                _sourcePath = value;
                OnPropertyChanged(nameof(SourcePath));
                OnPropertyChanged(nameof(HasNoImage));
                LoadThumbnail();
            }
        }

        public bool HasNoImage => string.IsNullOrEmpty(SourcePath);

        public BitmapSource? ThumbnailSource
        {
            get => _thumbnailSource;
            set { _thumbnailSource = value; OnPropertyChanged(nameof(ThumbnailSource)); }
        }

        /// <summary>
        /// Reference to the original ItemImage for syncing position changes back
        /// </summary>
        public ItemImage? ItemImageRef { get; set; }

        private void LoadThumbnail()
        {
            if (string.IsNullOrEmpty(SourcePath) || !File.Exists(SourcePath))
            {
                ThumbnailSource = null;
                return;
            }

            // Try to use the white removal callback if available
            if (ProcessImageCallback != null)
            {
                try
                {
                    // Use reasonable thumbnail size for the callback
                    int thumbWidth = (int)Math.Max(200, Width);
                    int thumbHeight = (int)Math.Max(200, Height);
                    var processed = ProcessImageCallback(SourcePath, thumbWidth, thumbHeight);
                    if (processed != null)
                    {
                        ThumbnailSource = processed;
                        return;
                    }
                }
                catch
                {
                    // Fall through to basic loading
                }
            }

            // Fallback: basic thumbnail without white removal
            try
            {
                var bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.UriSource = new Uri(SourcePath, UriKind.Absolute);
                bitmap.DecodePixelWidth = 200;
                bitmap.CacheOption = BitmapCacheOption.OnLoad;
                bitmap.EndInit();
                bitmap.Freeze();
                ThumbnailSource = bitmap;
            }
            catch
            {
                ThumbnailSource = null;
            }
        }

        /// <summary>
        /// Force reload the thumbnail (call after setting ProcessImageCallback)
        /// </summary>
        public void RefreshThumbnail()
        {
            LoadThumbnail();
        }
    }
}