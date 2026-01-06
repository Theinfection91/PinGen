using PinGen.App.Commands;
using PinGen.Core.Models;
using PinGen.Core.Validation;
using PinGen.ImageProcessing.Interfaces;
using PinGen.IO.Interfaces;
using PinGen.Rendering.Interfaces;
using PinGen.Templates.Interfaces;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace PinGen.App.ViewModels
{
    public class AwakenWindowViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged(string name)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

        private string _title = string.Empty;
        private string _subtitle = string.Empty;
        private string _footer = string.Empty;
        private double _titleFontSize = 64;
        private double _subtitleFontSize = 32;
        private double _footerFontSize = 48;

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

        public double TitleFontSize
        {
            get => _titleFontSize;
            set { _titleFontSize = value; OnPropertyChanged(nameof(TitleFontSize)); }
        }

        public double SubtitleFontSize
        {
            get => _subtitleFontSize;
            set { _subtitleFontSize = value; OnPropertyChanged(nameof(SubtitleFontSize)); }
        }

        public double FooterFontSize
        {
            get => _footerFontSize;
            set { _footerFontSize = value; OnPropertyChanged(nameof(FooterFontSize)); }
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
            set { _useBackground1 = value; OnPropertyChanged(nameof(UseBackground1)); UpdateEditorBackground(); }
        }

        public bool UseBackground2
        {
            get => _useBackground2;
            set { _useBackground2 = value; OnPropertyChanged(nameof(UseBackground2)); UpdateEditorBackground(); }
        }

        #region Editor Properties

        private string _selectedElementName = "Drag elements to reposition";
        public string SelectedElementName
        {
            get => _selectedElementName;
            set { _selectedElementName = value; OnPropertyChanged(nameof(SelectedElementName)); }
        }

        private BitmapSource? _editorBackgroundImage;
        public BitmapSource? EditorBackgroundImage
        {
            get => _editorBackgroundImage;
            set { _editorBackgroundImage = value; OnPropertyChanged(nameof(EditorBackgroundImage)); }
        }

        public ElementPosition TitlePosition { get; } = new(25, 30, 950, 125);
        public ElementPosition SubtitlePosition { get; } = new(25, 155, 950, 100);
        public ElementPosition FooterPosition { get; } = new(25, 1325, 950, 140);
        public ElementPosition Caption1Position { get; } = new(25, 270, 925, 30);
        public ElementPosition Caption2Position { get; } = new(25, 740, 925, 30);
        public ElementPosition Caption3Position { get; } = new(25, 1290, 925, 30);

        public ObservableCollection<EditorImageElement> EditorImageElements { get; } = new();
        public ObservableCollection<EditorNumberElement> EditorNumberElements { get; } = new();

        #endregion

        public ICommand RenderCommand { get; }
        public ICommand ResetPositionsCommand { get; }

        private readonly IPinRenderer _pinRenderer;
        private readonly ITemplateProvider _templateProvider;
        private readonly IFileSaver _fileSaver;
        private readonly IImageProcessor _imageProcessor;
        private readonly IImageLoader _imageLoader;

        public AwakenWindowViewModel(
            IPinRenderer pinRenderer,
            ITemplateProvider templateProvider,
            IFileSaver fileSaver,
            IImageProcessor imageProcessor,
            IImageLoader imageLoader)
        {
            _pinRenderer = pinRenderer;
            _templateProvider = templateProvider;
            _fileSaver = fileSaver;
            _imageProcessor = imageProcessor;
            _imageLoader = imageLoader;

            RenderCommand = new RelayCommand(Render);
            ResetPositionsCommand = new RelayCommand(ResetPositions);

            UpdateEditorBackground();
        }

        private BitmapSource? ProcessImageWithWhiteRemoval(string sourcePath, int width, int height)
        {
            try
            {
                var imageSharp = _imageLoader.LoadImageSharp(sourcePath);
                return _imageProcessor.PrepareAndRemoveWhite(imageSharp, width, height);
            }
            catch
            {
                return null;
            }
        }

        private void UpdateEditorBackground()
        {
            try
            {
                var backgroundPath = GetPreviewBackgroundPath();
                if (File.Exists(backgroundPath))
                {
                    var bitmap = new BitmapImage();
                    bitmap.BeginInit();
                    bitmap.UriSource = new Uri(backgroundPath, UriKind.Absolute);
                    bitmap.CacheOption = BitmapCacheOption.OnLoad;
                    bitmap.EndInit();
                    bitmap.Freeze();
                    EditorBackgroundImage = bitmap;
                }
            }
            catch { }
        }

        private void ResetPositions()
        {
            TitlePosition.X = 25; TitlePosition.Y = 30;
            TitlePosition.Width = 950; TitlePosition.Height = 125;

            SubtitlePosition.X = 25; SubtitlePosition.Y = 155;
            SubtitlePosition.Width = 950; SubtitlePosition.Height = 100;

            FooterPosition.X = 25; FooterPosition.Y = 1325;
            FooterPosition.Width = 950; FooterPosition.Height = 140;

            Caption1Position.X = 25; Caption1Position.Y = 270;
            Caption2Position.X = 25; Caption2Position.Y = 740;
            Caption3Position.X = 25; Caption3Position.Y = 1290;

            TitleFontSize = 64;
            SubtitleFontSize = 32;
            FooterFontSize = 48;

            RefreshEditorImageElements();
            SelectedElementName = "Positions reset to defaults";
        }

        public void RefreshEditorImageElements()
        {
            EditorImageElements.Clear();
            EditorNumberElements.Clear();

            var itemImages = GetItemImagesToList();
            if (itemImages.Count == 0) return;

            try
            {
                var template = _templateProvider.GetTemplate(itemImages.Count, IsFooterEnabled);

                for (int i = 0; i < itemImages.Count && i < template.TemplateSlots.Count; i++)
                {
                    var slot = template.TemplateSlots[i];
                    var itemImage = itemImages[i];

                    var imageElement = new EditorImageElement
                    {
                        SlotNumber = i + 1,
                        ElementName = $"Image{i + 1}",
                        X = slot.Bounds.X,
                        Y = slot.Bounds.Y,
                        Width = slot.Bounds.Width,
                        Height = slot.Bounds.Height,
                        ItemImageRef = itemImage,
                        ProcessImageCallback = ProcessImageWithWhiteRemoval
                    };
                    imageElement.SourcePath = itemImage.SourcePath;
                    EditorImageElements.Add(imageElement);

                    if (slot.ShowNumber && slot.NumberArea.HasValue)
                    {
                        var numberArea = slot.NumberArea.Value;
                        EditorNumberElements.Add(new EditorNumberElement
                        {
                            Number = i + 1,
                            X = numberArea.X,
                            Y = numberArea.Y,
                            Scale = itemImage.NumberScale,
                            ImageElementRef = imageElement
                        });
                    }
                }
            }
            catch
            {
                var defaultSlots = GetDefaultSlotPositions();
                for (int i = 0; i < itemImages.Count; i++)
                {
                    var pos = i < defaultSlots.Count ? defaultSlots[i] : (100 + i * 120, 300, 200, 250);

                    var imageElement = new EditorImageElement
                    {
                        SlotNumber = i + 1,
                        ElementName = $"Image{i + 1}",
                        X = pos.x, Y = pos.y,
                        Width = pos.w, Height = pos.h,
                        ItemImageRef = itemImages[i],
                        ProcessImageCallback = ProcessImageWithWhiteRemoval
                    };
                    imageElement.SourcePath = itemImages[i].SourcePath;
                    EditorImageElements.Add(imageElement);

                    EditorNumberElements.Add(new EditorNumberElement
                    {
                        Number = i + 1,
                        X = pos.x + pos.w - 80,
                        Y = pos.y + 10,
                        Scale = itemImages[i].NumberScale,
                        ImageElementRef = imageElement
                    });
                }
            }
        }

        private List<(double x, double y, double w, double h)> GetDefaultSlotPositions() => new()
        {
            (100, 300, 350, 400), (550, 300, 350, 400),
            (100, 750, 350, 400), (550, 750, 350, 400),
            (325, 500, 350, 400), (100, 300, 280, 350),
            (360, 300, 280, 350), (620, 300, 280, 350),
        };

        private List<ItemImage> GetItemImagesToList()
        {
            var list = new List<ItemImage>();
            if (!string.IsNullOrEmpty(Slot1.SourcePath)) list.Add(Slot1);
            if (!string.IsNullOrEmpty(Slot2.SourcePath)) list.Add(Slot2);
            if (!string.IsNullOrEmpty(Slot3.SourcePath)) list.Add(Slot3);
            if (!string.IsNullOrEmpty(Slot4.SourcePath)) list.Add(Slot4);
            if (!string.IsNullOrEmpty(Slot5.SourcePath)) list.Add(Slot5);
            if (!string.IsNullOrEmpty(Slot6.SourcePath)) list.Add(Slot6);
            if (!string.IsNullOrEmpty(Slot7.SourcePath)) list.Add(Slot7);
            if (!string.IsNullOrEmpty(Slot8.SourcePath)) list.Add(Slot8);
            return list;
        }

        private List<CaptionItem> GetCaptionsList() => new() { Caption1, Caption2, Caption3 };
        private string GetPreviewBackgroundPath()
        {
            string appBase = AppDomain.CurrentDomain.BaseDirectory;
            return Path.Combine(appBase, "Assets", "Backgrounds", UseBackground1 ? "bg1.png" : "bg2.png");
        }
        private List<ElementPosition> GetCaptionPositions() => new() { Caption1Position, Caption2Position, Caption3Position };

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

                var sanitizedTitle = _fileSaver.SanitizeFileName(Title, 199);
                string appBase = AppDomain.CurrentDomain.BaseDirectory;
                string outputFolder = Path.Combine(appBase, "Output", sanitizedTitle);
                Directory.CreateDirectory(outputFolder);

                string bg1Path = Path.Combine(appBase, "Assets", "Backgrounds", "bg1.png");
                string bg2Path = Path.Combine(appBase, "Assets", "Backgrounds", "bg2.png");

                var captionPositions = GetCaptionPositions();
                var imageElements = new List<EditorImageElement>(EditorImageElements);
                var numberElements = new List<EditorNumberElement>(EditorNumberElements);

                var bitmap1 = _pinRenderer.RenderWithEditorPositions(
                    request, bg1Path, TitlePosition, SubtitlePosition,
                    IsFooterEnabled ? FooterPosition : null,
                    captionPositions, imageElements, numberElements,
                    TitleFontSize, SubtitleFontSize, FooterFontSize);
                _fileSaver.Save(bitmap1, Path.Combine(outputFolder, $"{sanitizedTitle}1.png"));

                var bitmap2 = _pinRenderer.RenderWithEditorPositions(
                    request, bg2Path, TitlePosition, SubtitlePosition,
                    IsFooterEnabled ? FooterPosition : null,
                    captionPositions, imageElements, numberElements,
                    TitleFontSize, SubtitleFontSize, FooterFontSize);
                _fileSaver.Save(bitmap2, Path.Combine(outputFolder, $"{sanitizedTitle}2.png"));

                MessageBox.Show($"Images saved to:\n{outputFolder}", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred while rendering:\n{ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public void ClearAllSlots()
        {
            Slot1.SourcePath = string.Empty; Slot1.Scale = 1.0; Slot1.NumberScale = 1.0;
            Slot2.SourcePath = string.Empty; Slot2.Scale = 1.0; Slot2.NumberScale = 1.0;
            Slot3.SourcePath = string.Empty; Slot3.Scale = 1.0; Slot3.NumberScale = 1.0;
            Slot4.SourcePath = string.Empty; Slot4.Scale = 1.0; Slot4.NumberScale = 1.0;
            Slot5.SourcePath = string.Empty; Slot5.Scale = 1.0; Slot5.NumberScale = 1.0;
            Slot6.SourcePath = string.Empty; Slot6.Scale = 1.0; Slot6.NumberScale = 1.0;
            Slot7.SourcePath = string.Empty; Slot7.Scale = 1.0; Slot7.NumberScale = 1.0;
            Slot8.SourcePath = string.Empty; Slot8.Scale = 1.0; Slot8.NumberScale = 1.0;
            EditorImageElements.Clear();
            EditorNumberElements.Clear();
        }
    }
}
