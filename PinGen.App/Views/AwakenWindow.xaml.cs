using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using Microsoft.Win32;
using PinGen.App.ViewModels;
using PinGen.Core.Models;

namespace PinGen.App.Views
{
    public partial class AwakenWindow : Window
    {
        private bool _isDragging;
        private Point _dragStartPoint;
        private FrameworkElement? _draggedElement;
        private double _elementStartLeft;
        private double _elementStartTop;

        private readonly List<Border> _imageElementBorders = new();
        private readonly List<Border> _numberElementBorders = new();
        private readonly List<Border> _textElementBorders = new();
        private readonly List<ItemImage> _subscribedSlots = new();

        private static Typeface? _horizonFont;
        
        private int _currentMaxZIndex = 0;

        // Supported image extensions for batch loading
        private static readonly string[] SupportedImageExtensions = { ".png", ".jpg", ".jpeg", ".bmp", ".gif" };

        public AwakenWindow()
        {
            InitializeComponent();
            DataContextChanged += OnDataContextChanged;
            Loaded += OnLoaded;
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            string appBase = AppDomain.CurrentDomain.BaseDirectory;
            string fontPath = System.IO.Path.Combine(appBase, "Assets", "Fonts", "horizon.otf");
            if (System.IO.File.Exists(fontPath))
            {
                var fonts = Fonts.GetTypefaces(fontPath);
                foreach (var tf in fonts) { _horizonFont = tf; break; }
            }
            _horizonFont ??= new Typeface("Arial");

            RebuildTextElementsOnCanvas();
        }

        private void OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (e.OldValue is AwakenWindowViewModel oldVm)
            {
                oldVm.EditorImageElements.CollectionChanged -= OnEditorImageElementsChanged;
                oldVm.EditorNumberElements.CollectionChanged -= OnEditorNumberElementsChanged;
                oldVm.PropertyChanged -= OnViewModelPropertyChanged;
                UnsubscribeFromSlotChanges();
            }

            if (e.NewValue is AwakenWindowViewModel newVm)
            {
                newVm.EditorImageElements.CollectionChanged += OnEditorImageElementsChanged;
                newVm.EditorNumberElements.CollectionChanged += OnEditorNumberElementsChanged;
                newVm.PropertyChanged += OnViewModelPropertyChanged;
                SubscribeToSlotChanges(newVm);
                RebuildAllCanvasElements();
            }
        }

        private void OnViewModelPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName is nameof(AwakenWindowViewModel.Title) or nameof(AwakenWindowViewModel.TitleFontSize)
                or nameof(AwakenWindowViewModel.Subtitle) or nameof(AwakenWindowViewModel.SubtitleFontSize)
                or nameof(AwakenWindowViewModel.Footer) or nameof(AwakenWindowViewModel.FooterFontSize)
                or nameof(AwakenWindowViewModel.IsFooterEnabled))
            {
                RebuildTextElementsOnCanvas();
            }
        }

        private void SubscribeToSlotChanges(AwakenWindowViewModel vm)
        {
            var slots = new[] { vm.Slot1, vm.Slot2, vm.Slot3, vm.Slot4, vm.Slot5, vm.Slot6, vm.Slot7, vm.Slot8 };
            foreach (var slot in slots)
            {
                slot.PropertyChanged += OnSlotPropertyChanged;
                _subscribedSlots.Add(slot);
            }

            vm.Caption1.PropertyChanged += OnCaptionPropertyChanged;
            vm.Caption2.PropertyChanged += OnCaptionPropertyChanged;
            vm.Caption3.PropertyChanged += OnCaptionPropertyChanged;
        }

        private void UnsubscribeFromSlotChanges()
        {
            foreach (var slot in _subscribedSlots)
                slot.PropertyChanged -= OnSlotPropertyChanged;
            _subscribedSlots.Clear();
        }

        private void OnCaptionPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            RebuildTextElementsOnCanvas();
        }

        private void OnSlotPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(ItemImage.Scale))
            {
                RebuildImageElementsOnCanvas();
                RebuildNumberElementsOnCanvas();
            }
            else if (e.PropertyName == nameof(ItemImage.NumberScale))
            {
                if (sender is ItemImage slot && DataContext is AwakenWindowViewModel vm)
                {
                    foreach (var numElement in vm.EditorNumberElements)
                    {
                        if (numElement.ImageElementRef?.ItemImageRef == slot)
                        {
                            numElement.Scale = slot.NumberScale;
                            break;
                        }
                    }
                }
                RebuildNumberElementsOnCanvas();
            }
        }

        private void OnEditorImageElementsChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            RebuildImageElementsOnCanvas();
            RebuildNumberElementsOnCanvas();
        }

        private void OnEditorNumberElementsChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            RebuildNumberElementsOnCanvas();
        }

        private void RebuildAllCanvasElements()
        {
            _currentMaxZIndex = 0;
            RebuildTextElementsOnCanvas();
            RebuildImageElementsOnCanvas();
            RebuildNumberElementsOnCanvas();
        }

        private void RebuildTextElementsOnCanvas()
        {
            foreach (var border in _textElementBorders)
                EditorCanvas.Children.Remove(border);
            _textElementBorders.Clear();

            if (DataContext is not AwakenWindowViewModel vm || _horizonFont == null) return;

            var titleBorder = CreateOutlinedTextElement(vm.Title, vm.TitlePosition, vm.TitleFontSize, 4, "Title");
            _textElementBorders.Add(titleBorder);
            EditorCanvas.Children.Add(titleBorder);

            var subtitleBorder = CreateOutlinedTextElement(vm.Subtitle, vm.SubtitlePosition, vm.SubtitleFontSize, 2, "Subtitle");
            _textElementBorders.Add(subtitleBorder);
            EditorCanvas.Children.Add(subtitleBorder);

            if (vm.IsFooterEnabled)
            {
                var footerBorder = CreateOutlinedTextElement(vm.Footer, vm.FooterPosition, vm.FooterFontSize, 4, "Footer");
                _textElementBorders.Add(footerBorder);
                EditorCanvas.Children.Add(footerBorder);
            }

            var caption1Border = CreateOutlinedTextElement(vm.Caption1.Text, vm.Caption1Position, vm.Caption1.FontSize, 2, "Caption1", Color.FromArgb(0x40, 0x00, 0x44, 0x00));
            _textElementBorders.Add(caption1Border);
            EditorCanvas.Children.Add(caption1Border);

            var caption2Border = CreateOutlinedTextElement(vm.Caption2.Text, vm.Caption2Position, vm.Caption2.FontSize, 2, "Caption2", Color.FromArgb(0x40, 0x00, 0x44, 0x00));
            _textElementBorders.Add(caption2Border);
            EditorCanvas.Children.Add(caption2Border);

            var caption3Border = CreateOutlinedTextElement(vm.Caption3.Text, vm.Caption3Position, vm.Caption3.FontSize, 2, "Caption3", Color.FromArgb(0x40, 0x00, 0x44, 0x00));
            _textElementBorders.Add(caption3Border);
            EditorCanvas.Children.Add(caption3Border);
        }

        private Border CreateOutlinedTextElement(string text, ElementPosition pos, double fontSize, double strokeWidth, string tag, Color? bgColor = null)
        {
            var path = new Path();

            if (!string.IsNullOrWhiteSpace(text))
            {
                var ft = new FormattedText(
                    text,
                    System.Globalization.CultureInfo.CurrentCulture,
                    FlowDirection.LeftToRight,
                    _horizonFont!,
                    fontSize,
                    Brushes.Black,
                    1.0)
                {
                    MaxTextWidth = pos.Width,
                    TextAlignment = TextAlignment.Center,
                    Trimming = TextTrimming.CharacterEllipsis
                };

                var geometry = ft.BuildGeometry(new Point(0, (pos.Height - ft.Height) / 2));
                path.Data = geometry;
                path.Stroke = Brushes.White;
                path.StrokeThickness = strokeWidth;
                path.Fill = Brushes.Black;
            }

            var border = new Border
            {
                Width = pos.Width,
                Height = pos.Height,
                Background = new SolidColorBrush(bgColor ?? Color.FromArgb(0x40, 0x00, 0x00, 0x00)),
                BorderBrush = Brushes.Transparent,
                BorderThickness = new Thickness(2),
                Cursor = Cursors.SizeAll,
                Tag = tag,
                Child = path
            };

            Canvas.SetLeft(border, pos.X);
            Canvas.SetTop(border, pos.Y);

            border.MouseLeftButtonDown += DraggableElement_MouseLeftButtonDown;
            border.MouseMove += DraggableElement_MouseMove;
            border.MouseLeftButtonUp += DraggableElement_MouseLeftButtonUp;
            border.MouseRightButtonDown += DraggableElement_MouseRightButtonDown;
            border.MouseEnter += (s, e) => border.BorderBrush = new SolidColorBrush(Color.FromRgb(0x00, 0xBF, 0xFF));
            border.MouseLeave += (s, e) => border.BorderBrush = Brushes.Transparent;

            return border;
        }

        private void RebuildImageElementsOnCanvas()
        {
            foreach (var border in _imageElementBorders)
                EditorCanvas.Children.Remove(border);
            _imageElementBorders.Clear();

            if (DataContext is not AwakenWindowViewModel vm) return;

            foreach (var element in vm.EditorImageElements)
            {
                var border = CreateImageElementBorder(element);
                _imageElementBorders.Add(border);
                EditorCanvas.Children.Add(border);
            }
        }

        private void RebuildNumberElementsOnCanvas()
        {
            foreach (var border in _numberElementBorders)
                EditorCanvas.Children.Remove(border);
            _numberElementBorders.Clear();

            if (DataContext is not AwakenWindowViewModel vm) return;

            foreach (var element in vm.EditorNumberElements)
            {
                var border = CreateNumberElementBorder(element);
                _numberElementBorders.Add(border);
                EditorCanvas.Children.Add(border);
            }
        }

        private Border CreateImageElementBorder(EditorImageElement element)
        {
            double scale = element.ItemImageRef?.Scale ?? 1.0;
            double scaledWidth = element.Width * scale;
            double scaledHeight = element.Height * scale;
            double offsetX = (element.Width - scaledWidth) / 2;
            double offsetY = (element.Height - scaledHeight) / 2;

            var image = new Image { Source = element.ThumbnailSource, Stretch = Stretch.Uniform, Margin = new Thickness(4) };
            var grid = new Grid { Background = new SolidColorBrush(Color.FromArgb(0x40, 0x44, 0x00, 0x44)) };
            grid.Children.Add(image);

            var border = new Border
            {
                Width = scaledWidth, Height = scaledHeight, Tag = element,
                BorderBrush = Brushes.Transparent, BorderThickness = new Thickness(2),
                Cursor = Cursors.SizeAll, Child = grid
            };

            Canvas.SetLeft(border, element.X + offsetX);
            Canvas.SetTop(border, element.Y + offsetY);

            border.MouseLeftButtonDown += DraggableElement_MouseLeftButtonDown;
            border.MouseMove += DraggableElement_MouseMove;
            border.MouseLeftButtonUp += DraggableElement_MouseLeftButtonUp;
            border.MouseRightButtonDown += DraggableElement_MouseRightButtonDown;
            border.MouseEnter += (s, e) => border.BorderBrush = new SolidColorBrush(Color.FromRgb(0x00, 0xBF, 0xFF));
            border.MouseLeave += (s, e) => border.BorderBrush = Brushes.Transparent;

            return border;
        }

        private Border CreateNumberElementBorder(EditorNumberElement element)
        {
            double shadowFontSize = 72 * element.Scale;
            double mainFontSize = 64 * element.Scale;

            var shadowText = new TextBlock
            {
                Text = element.Number.ToString(), Foreground = Brushes.Black, FontSize = shadowFontSize,
                FontWeight = FontWeights.Bold, HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center, Margin = new Thickness(6, 0, 0, 0)
            };
            var mainText = new TextBlock
            {
                Text = element.Number.ToString(), Foreground = Brushes.White, FontSize = mainFontSize,
                FontWeight = FontWeights.Bold, HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center
            };

            var grid = new Grid();
            grid.Children.Add(shadowText);
            grid.Children.Add(mainText);

            double size = 80 * element.Scale;
            var border = new Border
            {
                Width = size, Height = size, Tag = element, Background = Brushes.Transparent,
                BorderBrush = new SolidColorBrush(Color.FromArgb(0x80, 0xFF, 0xFF, 0x00)),
                BorderThickness = new Thickness(2), Cursor = Cursors.SizeAll, Child = grid
            };

            Canvas.SetLeft(border, element.X);
            Canvas.SetTop(border, element.Y);

            border.MouseLeftButtonDown += DraggableElement_MouseLeftButtonDown;
            border.MouseMove += DraggableElement_MouseMove;
            border.MouseLeftButtonUp += DraggableElement_MouseLeftButtonUp;
            border.MouseRightButtonDown += DraggableElement_MouseRightButtonDown;
            border.MouseEnter += (s, e) => border.BorderBrush = Brushes.Lime;
            border.MouseLeave += (s, e) => border.BorderBrush = new SolidColorBrush(Color.FromArgb(0x80, 0xFF, 0xFF, 0x00));

            return border;
        }

        private void BrowseImage_Click(object sender, RoutedEventArgs e)
        {
            if (sender is not Button button || button.Tag is not ItemImage slot) return;
            var dialog = new OpenFileDialog { Filter = "Image Files|*.png;*.jpg;*.jpeg;*.bmp;*.gif;" };
            if (dialog.ShowDialog() == true)
            {
                slot.SourcePath = dialog.FileName;
                (DataContext as AwakenWindowViewModel)?.RefreshEditorImageElements();
            }
        }

        /// <summary>
        /// Batch load images from a folder. Requires 4-8 supported image files.
        /// </summary>
        private void LoadFolder_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new OpenFolderDialog
            {
                Title = "Select a folder containing 4-8 images"
            };

            if (dialog.ShowDialog() != true)
                return;

            var folderPath = dialog.FolderName;

            // Get all supported image files, sorted by name
            var imageFiles = System.IO.Directory.GetFiles(folderPath)
                .Where(f => SupportedImageExtensions.Contains(System.IO.Path.GetExtension(f).ToLowerInvariant()))
                .OrderBy(f => f)
                .ToList();

            if (imageFiles.Count < 4)
            {
                MessageBox.Show(
                    $"Found {imageFiles.Count} image(s) in folder.\nNeed at least 4 images (supports up to 8).\n\nSupported formats: PNG, JPG, JPEG, BMP, GIF",
                    "Not Enough Images",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);
                return;
            }

            if (imageFiles.Count > 8)
            {
                MessageBox.Show(
                    $"Found {imageFiles.Count} images in folder.\nMaximum is 8 images. Only the first 8 will be loaded.",
                    "Too Many Images",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);
                imageFiles = imageFiles.Take(8).ToList();
            }

            if (DataContext is not AwakenWindowViewModel vm)
                return;

            // Clear existing slots first
            vm.ClearAllSlots();

            // Get slots array for easy indexing
            var slots = new[] { vm.Slot1, vm.Slot2, vm.Slot3, vm.Slot4, vm.Slot5, vm.Slot6, vm.Slot7, vm.Slot8 };

            // Load each image into corresponding slot
            for (int i = 0; i < imageFiles.Count; i++)
            {
                slots[i].SourcePath = imageFiles[i];
            }

            // Refresh the editor
            vm.RefreshEditorImageElements();

            MessageBox.Show(
                $"Loaded {imageFiles.Count} images from:\n{folderPath}",
                "Batch Load Complete",
                MessageBoxButton.OK,
                MessageBoxImage.Information);
        }

        private void ClearSlot_Click(object sender, RoutedEventArgs e)
        {
            if (sender is not Button button || button.Tag is not ItemImage slot) return;
            slot.SourcePath = string.Empty;
            slot.Scale = 1.0;
            slot.NumberScale = 1.0;
            (DataContext as AwakenWindowViewModel)?.RefreshEditorImageElements();
        }

        private void ClearImages_Click(object sender, RoutedEventArgs e)
        {
            (DataContext as AwakenWindowViewModel)?.ClearAllSlots();
            RebuildAllCanvasElements();
        }

        private void BringToFront(UIElement element)
        {
            _currentMaxZIndex++;
            Canvas.SetZIndex(element, _currentMaxZIndex);
        }

        private List<Border> GetElementsAtPoint(Point point)
        {
            var allElements = _textElementBorders
                .Concat(_imageElementBorders)
                .Concat(_numberElementBorders)
                .ToList();

            var elementsAtPoint = new List<Border>();

            foreach (var element in allElements)
            {
                double left = Canvas.GetLeft(element);
                double top = Canvas.GetTop(element);
                if (double.IsNaN(left)) left = 0;
                if (double.IsNaN(top)) top = 0;

                var bounds = new Rect(left, top, element.Width, element.Height);
                if (bounds.Contains(point))
                {
                    elementsAtPoint.Add(element);
                }
            }

            return elementsAtPoint.OrderByDescending(e => Canvas.GetZIndex(e)).ToList();
        }

        private string GetElementDisplayName(FrameworkElement element)
        {
            return element.Tag switch
            {
                EditorImageElement img => $"Image {img.SlotNumber} (Scale: {img.ItemImageRef?.Scale:F1}x)",
                EditorNumberElement num => $"Number {num.Number} (Scale: {num.Scale:F1}x)",
                string s => s,
                _ => "Unknown"
            };
        }

        #region Drag and Drop

        private void DraggableElement_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (sender is not FrameworkElement element) return;

            var clickPoint = e.GetPosition(EditorCanvas);

            BringToFront(element);
            
            _isDragging = true;
            _draggedElement = element;
            _dragStartPoint = clickPoint;
            _elementStartLeft = Canvas.GetLeft(element);
            _elementStartTop = Canvas.GetTop(element);
            if (double.IsNaN(_elementStartLeft)) _elementStartLeft = 0;
            if (double.IsNaN(_elementStartTop)) _elementStartTop = 0;
            element.CaptureMouse();

            if (DataContext is AwakenWindowViewModel vm)
            {
                vm.SelectedElementName = GetElementDisplayName(element);
            }
            e.Handled = true;
        }

        private void DraggableElement_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (sender is not FrameworkElement element) return;

            var clickPoint = e.GetPosition(EditorCanvas);
            var elementsAtPoint = GetElementsAtPoint(clickPoint);

            if (elementsAtPoint.Count > 1)
            {
                int currentIndex = elementsAtPoint.IndexOf((Border)element);
                int nextIndex = (currentIndex + 1) % elementsAtPoint.Count;
                var nextElement = elementsAtPoint[nextIndex];

                BringToFront(nextElement);

                if (DataContext is AwakenWindowViewModel vm)
                {
                    vm.SelectedElementName = GetElementDisplayName(nextElement) + " (Right-click to cycle)";
                }
            }

            e.Handled = true;
        }

        private void DraggableElement_MouseMove(object sender, MouseEventArgs e)
        {
            if (!_isDragging || _draggedElement == null) return;
            var pt = e.GetPosition(EditorCanvas);
            var newLeft = _elementStartLeft + (pt.X - _dragStartPoint.X);
            var newTop = _elementStartTop + (pt.Y - _dragStartPoint.Y);
            Canvas.SetLeft(_draggedElement, newLeft);
            Canvas.SetTop(_draggedElement, newTop);
            UpdateElementPosition(_draggedElement, newLeft, newTop);
            e.Handled = true;
        }

        private void DraggableElement_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (!_isDragging || _draggedElement == null) return;
            _draggedElement.ReleaseMouseCapture();
            UpdateElementPosition(_draggedElement, Canvas.GetLeft(_draggedElement), Canvas.GetTop(_draggedElement));
            _isDragging = false;
            _draggedElement = null;
            e.Handled = true;
        }

        private void UpdateElementPosition(FrameworkElement element, double x, double y)
        {
            if (DataContext is not AwakenWindowViewModel vm) return;

            switch (element.Tag)
            {
                case EditorImageElement img:
                    double scale = img.ItemImageRef?.Scale ?? 1.0;
                    img.X = x - (img.Width - img.Width * scale) / 2;
                    img.Y = y - (img.Height - img.Height * scale) / 2;
                    break;
                case EditorNumberElement num:
                    num.X = x; num.Y = y;
                    break;
                case "Title": vm.TitlePosition.X = x; vm.TitlePosition.Y = y; break;
                case "Subtitle": vm.SubtitlePosition.X = x; vm.SubtitlePosition.Y = y; break;
                case "Footer": vm.FooterPosition.X = x; vm.FooterPosition.Y = y; break;
                case "Caption1": vm.Caption1Position.X = x; vm.Caption1Position.Y = y; break;
                case "Caption2": vm.Caption2Position.X = x; vm.Caption2Position.Y = y; break;
                case "Caption3": vm.Caption3Position.X = x; vm.Caption3Position.Y = y; break;
            }
        }

        #endregion
    }
}
