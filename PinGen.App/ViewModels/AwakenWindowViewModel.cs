using PinGen.App.Commands;
using PinGen.Core.Models;
using PinGen.Core.Validation;
using PinGen.IO.Interfaces;
using PinGen.Rendering.Interfaces;
using PinGen.Templates.Interfaces;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace PinGen.App.ViewModels
{
    public class AwakenWindowViewModel : INotifyPropertyChanged
    {
        // INotifyPropertyChanged implementation
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string name)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

        private string _title;
        private string _subtitle;
        private string _footer;

        public string Title
        {
            get => _title;
            set { _title = value; OnPropertyChanged(nameof(Title)); }
        }

        public string Subtitle
        {
            get => _subtitle;
            set { _subtitle = value; OnPropertyChanged(nameof(Subtitle)); }
        }

        public string Footer
        {
            get => _footer;
            set { _footer = value; OnPropertyChanged(nameof(Footer)); }
        }

        // Captions with font size control
        public CaptionItem Caption1 { get; } = new();
        public CaptionItem Caption2 { get; } = new();
        public CaptionItem Caption3 { get; } = new();

        // Font size options for dropdowns
        public int[] FontSizeOptions => CaptionItem.AvailableFontSizes;

        // 8 fixed image slots
        public ItemImage Slot1 { get; } = new() { GroupName = "Slot1Tolerance" };
        public ItemImage Slot2 { get; } = new() { GroupName = "Slot2Tolerance" };
        public ItemImage Slot3 { get; } = new() { GroupName = "Slot3Tolerance" };
        public ItemImage Slot4 { get; } = new() { GroupName = "Slot4Tolerance" };
        public ItemImage Slot5 { get; } = new() { GroupName = "Slot5Tolerance" };
        public ItemImage Slot6 { get; } = new() { GroupName = "Slot6Tolerance" };
        public ItemImage Slot7 { get; } = new() { GroupName = "Slot7Tolerance" };
        public ItemImage Slot8 { get; } = new() { GroupName = "Slot8Tolerance" };

        // Footer toggle
        private bool _isFooterEnabled;
        public bool IsFooterEnabled
        {
            get => _isFooterEnabled;
            set { _isFooterEnabled = value; OnPropertyChanged(nameof(IsFooterEnabled)); }
        }

        // Background preview selection
        private bool _useBackground1 = true;
        private bool _useBackground2;

        public bool UseBackground1
        {
            get => _useBackground1;
            set { _useBackground1 = value; OnPropertyChanged(nameof(UseBackground1)); }
        }

        public bool UseBackground2
        {
            get => _useBackground2;
            set { _useBackground2 = value; OnPropertyChanged(nameof(UseBackground2)); }
        }

        // Preview Window Property
        private BitmapSource _previewImage;
        public BitmapSource PreviewImage
        {
            get => _previewImage;
            set { _previewImage = value; OnPropertyChanged(nameof(PreviewImage)); }
        }

        // Cached Y offsets from preview generation (ensures saved images match preview)
        private List<double> _cachedYOffsets = new();

        // Interfaces
        public ICommand GeneratePreviewCommand { get; }
        public ICommand RenderCommand { get; }

        private readonly IPinRenderer _pinRenderer;
        private readonly ITemplateProvider _templateProvider;
        private readonly IFileSaver _fileSaver;

        public AwakenWindowViewModel(IPinRenderer pinRenderer, ITemplateProvider templateProvider, IFileSaver fileSaver)
        {
            _pinRenderer = pinRenderer;
            _templateProvider = templateProvider;
            _fileSaver = fileSaver;

            GeneratePreviewCommand = new RelayCommand(GeneratePreview);
            RenderCommand = new RelayCommand(Render);
        }

        private List<ItemImage> GetItemImagesToList()
        {
            var itemImages = new List<ItemImage>();
            if (!string.IsNullOrEmpty(Slot1.SourcePath))
                itemImages.Add(Slot1);
            if (!string.IsNullOrEmpty(Slot2.SourcePath))
                itemImages.Add(Slot2);
            if (!string.IsNullOrEmpty(Slot3.SourcePath))
                itemImages.Add(Slot3);
            if (!string.IsNullOrEmpty(Slot4.SourcePath))
                itemImages.Add(Slot4);
            if (!string.IsNullOrEmpty(Slot5.SourcePath))
                itemImages.Add(Slot5);
            if (!string.IsNullOrEmpty(Slot6.SourcePath))
                itemImages.Add(Slot6);
            if (!string.IsNullOrEmpty(Slot7.SourcePath))
                itemImages.Add(Slot7);
            if (!string.IsNullOrEmpty(Slot8.SourcePath))
                itemImages.Add(Slot8);
            return itemImages;
        }

        private List<CaptionItem> GetCaptionsList()
        {
            return new List<CaptionItem> { Caption1, Caption2, Caption3 };
        }

        private string GetPreviewBackgroundPath()
        {
            string appBase = AppDomain.CurrentDomain.BaseDirectory;
            string bgFile = UseBackground1 ? "bg1.png" : "bg2.png";
            return Path.Combine(appBase, "Assets", "Backgrounds", bgFile);
        }

        private void GeneratePreview()
        {
            try
            {
                var request = new PinRequest
                {
                    Title = Title,
                    Subtitle = Subtitle,
                    Footer = IsFooterEnabled ? Footer : string.Empty,
                    ItemImages = GetItemImagesToList(),
                    Captions = GetCaptionsList(),
                };

                if (!PinRequestValidator.Validate(request))
                {
                    MessageBox.Show("The pin request is invalid. Please check your inputs.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                // Generate new random Y offsets for this preview
                _cachedYOffsets.Clear();
                for (int i = 0; i < request.ItemImages.Count; i++)
                {
                    _cachedYOffsets.Add(Random.Shared.Next(-15, 16));
                }

                var template = _templateProvider.GetTemplate(request.ItemImages.Count, IsFooterEnabled);
                var backgroundPath = GetPreviewBackgroundPath();
                var bitmap = _pinRenderer.Render(request, template, backgroundPath, _cachedYOffsets);
                PreviewImage = bitmap;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred while generating the preview:\n{ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Render()
        {
            try
            {
                var request = new PinRequest
                {
                    Title = Title,
                    Subtitle = Subtitle,
                    Footer = IsFooterEnabled ? Footer : string.Empty,
                    ItemImages = GetItemImagesToList(),
                    Captions = GetCaptionsList(),
                };

                if (!PinRequestValidator.Validate(request))
                {
                    MessageBox.Show("The pin request is invalid. Please check your inputs.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                // If no preview was generated, create offsets now
                if (_cachedYOffsets.Count != request.ItemImages.Count)
                {
                    _cachedYOffsets.Clear();
                    for (int i = 0; i < request.ItemImages.Count; i++)
                    {
                        _cachedYOffsets.Add(Random.Shared.Next(-15, 16));
                    }
                }

                var template = _templateProvider.GetTemplate(request.ItemImages.Count, IsFooterEnabled);

                // Sanitize title for filename (leave room for number suffix)
                var sanitizedTitle = _fileSaver.SanitizeFileName(Title, 199);

                // Create output folder based on sanitized title
                string appBase = AppDomain.CurrentDomain.BaseDirectory;
                string outputFolder = Path.Combine(appBase, "Output", sanitizedTitle);
                Directory.CreateDirectory(outputFolder);

                // Background paths
                string bg1Path = Path.Combine(appBase, "Assets", "Backgrounds", "bg1.png");
                string bg2Path = Path.Combine(appBase, "Assets", "Backgrounds", "bg2.png");

                // Render and save with bg1 (using cached offsets)
                var bitmap1 = _pinRenderer.Render(request, template, bg1Path, _cachedYOffsets);
                string file1Path = Path.Combine(outputFolder, $"{sanitizedTitle}1.png");
                _fileSaver.Save(bitmap1, file1Path);

                // Render and save with bg2 (using same cached offsets)
                var bitmap2 = _pinRenderer.Render(request, template, bg2Path, _cachedYOffsets);
                string file2Path = Path.Combine(outputFolder, $"{sanitizedTitle}2.png");
                _fileSaver.Save(bitmap2, file2Path);

                MessageBox.Show($"Images saved to:\n{outputFolder}", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred while rendering:\n{ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public void ClearAllSlots()
        {
            Slot1.SourcePath = string.Empty;
            Slot2.SourcePath = string.Empty;
            Slot3.SourcePath = string.Empty;
            Slot4.SourcePath = string.Empty;
            Slot5.SourcePath = string.Empty;
            Slot6.SourcePath = string.Empty;
            Slot7.SourcePath = string.Empty;
            Slot8.SourcePath = string.Empty;

            Slot1.Scale = 1.0;
            Slot2.Scale = 1.0;
            Slot3.Scale = 1.0;
            Slot4.Scale = 1.0;
            Slot5.Scale = 1.0;
            Slot6.Scale = 1.0;
            Slot7.Scale = 1.0;
            Slot8.Scale = 1.0;

            // Clear cached offsets when slots are cleared
            _cachedYOffsets.Clear();
        }
    }
}
