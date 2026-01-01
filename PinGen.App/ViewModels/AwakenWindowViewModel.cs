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

        public ObservableCollection<string> Captions { get; } = new();

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

        // Background options
        private bool _useBackground1 = true;
        private bool _useBackground2;
        private bool _useCustomColor;

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

        public bool UseCustomColor
        {
            get => _useCustomColor;
            set { _useCustomColor = value; OnPropertyChanged(nameof(UseCustomColor)); }
        }

        // Color palette selections
        private bool _isWhiteSelected = true;
        private bool _isBlackSelected;
        private bool _isRedSelected;
        private bool _isBlueSelected;
        private bool _isGreenSelected;
        private bool _isYellowSelected;
        private bool _isPurpleSelected;
        private bool _isOrangeSelected;

        public bool IsWhiteSelected
        {
            get => _isWhiteSelected;
            set { _isWhiteSelected = value; OnPropertyChanged(nameof(IsWhiteSelected)); }
        }

        public bool IsBlackSelected
        {
            get => _isBlackSelected;
            set { _isBlackSelected = value; OnPropertyChanged(nameof(IsBlackSelected)); }
        }

        public bool IsRedSelected
        {
            get => _isRedSelected;
            set { _isRedSelected = value; OnPropertyChanged(nameof(IsRedSelected)); }
        }

        public bool IsBlueSelected
        {
            get => _isBlueSelected;
            set { _isBlueSelected = value; OnPropertyChanged(nameof(IsBlueSelected)); }
        }

        public bool IsGreenSelected
        {
            get => _isGreenSelected;
            set { _isGreenSelected = value; OnPropertyChanged(nameof(IsGreenSelected)); }
        }

        public bool IsYellowSelected
        {
            get => _isYellowSelected;
            set { _isYellowSelected = value; OnPropertyChanged(nameof(IsYellowSelected)); }
        }

        public bool IsPurpleSelected
        {
            get => _isPurpleSelected;
            set { _isPurpleSelected = value; OnPropertyChanged(nameof(IsPurpleSelected)); }
        }

        public bool IsOrangeSelected
        {
            get => _isOrangeSelected;
            set { _isOrangeSelected = value; OnPropertyChanged(nameof(IsOrangeSelected)); }
        }

        // Preview Window Property
        private BitmapSource _previewImage;
        public BitmapSource PreviewImage
        {
            get => _previewImage;
            set { _previewImage = value; OnPropertyChanged(nameof(PreviewImage)); }
        }

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

            // Seed captions
            Captions.Add("");
            Captions.Add("");
            Captions.Add("");
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
                    Captions = Captions.ToList(),
                };

                if (!PinRequestValidator.Validate(request))
                {
                    MessageBox.Show("The pin request is invalid. Please check your inputs.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                var template = _templateProvider.GetTemplate(request.ItemImages.Count);
                var bitmap = _pinRenderer.Render(request, template);
                PreviewImage = bitmap;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred while generating the preview:\n{ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            // Implementation for generating preview image based on current settings
            // This is a placeholder for actual preview generation logic
            // You would typically call your rendering service here
        }

        private void Render()
        {
            // Implementation for rendering the final pin image based on current settings
            // This is a placeholder for actual rendering logic
            // You would typically call your rendering service here
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
        }
    }
}
