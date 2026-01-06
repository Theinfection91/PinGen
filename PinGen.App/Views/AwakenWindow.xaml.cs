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

        private int _currentNonNumberZIndex = 0;
        private int _currentNumberZIndex = 0;
        private const int NumberElementBaseZIndex = 1000;

        private List<Border>? _lastCycleElements;
        private int _lastCycleIndex;
        private Point _lastCyclePoint;

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
            _currentNonNumberZIndex = 0;
            _currentNumberZIndex = 0;
            ResetCycleState();
            RebuildTextElementsOnCanvas();
            RebuildImageElementsOnCanvas();
            RebuildNumberElementsOnCanvas();
        }

        private void ResetCycleState()
        {
            _lastCycleElements = null;
            _lastCycleIndex = 0;
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
            double actualTextHeight = pos.Height;

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
                    Trimming = TextTrimming.None
                };

                actualTextHeight = Math.Max(ft.Height + strokeWidth * 2, pos.Height);
                var geometry = ft.BuildGeometry(new Point(0, (actualTextHeight - ft.Height) / 2));
                path.Data = geometry;
                path.Stroke = Brushes.White;
                path.StrokeThickness = strokeWidth;
                path.Fill = Brushes.Black;
            }

            RenderOptions.SetEdgeMode(path, EdgeMode.Unspecified);

            var border = new Border
            {
                Width = pos.Width,
                Height = actualTextHeight,
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

            int numberIndex = 0;
            foreach (var element in vm.EditorNumberElements)
            {
                var border = CreateNumberElementBorder(element);
                _numberElementBorders.Add(border);
                EditorCanvas.Children.Add(border);
                Canvas.SetZIndex(border, NumberElementBaseZIndex + numberIndex);
                numberIndex++;
            }

            _currentNumberZIndex = _numberElementBorders.Count;
        }

        private Border CreateImageElementBorder(EditorImageElement element)
        {
            double scale = element.ItemImageRef?.Scale ?? 1.0;
            double scaledWidth = element.Width * scale;
            double scaledHeight = element.Height * scale;
            double offsetX = (element.Width - scaledWidth) / 2;
            double offsetY = (element.Height - scaledHeight) / 2;

            var image = new Image { Source = element.ThumbnailSource, Stretch = Stretch.Uniform, Margin = new Thickness(4) };
            RenderOptions.SetBitmapScalingMode(image, BitmapScalingMode.HighQuality);

            var grid = new Grid { Background = new SolidColorBrush(Color.FromArgb(0x40, 0x44, 0x00, 0x44)) };
            grid.Children.Add(image);

            var border = new Border
            {
                Width = scaledWidth, Height = scaledHeight, Tag = element,
                BorderBrush = Brushes.Transparent, BorderThickness = new Thickness(2),
                Cursor = Cursors.SizeAll, Child = grid
            };
            RenderOptions.SetBitmapScalingMode(border, BitmapScalingMode.HighQuality);

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

            TextOptions.SetTextRenderingMode(shadowText, TextRenderingMode.ClearType);
            TextOptions.SetTextRenderingMode(mainText, TextRenderingMode.ClearType);

            var grid = new Grid();
            grid.Children.Add(shadowText);
            grid.Children.Add(mainText);

            double size = 80 * element.Scale;
            var border = new Border
            {
                Width = size, Height = size, Tag = element,
                Background = new SolidColorBrush(Color.FromArgb(0x01, 0x00, 0x00, 0x00)),
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
            var dialog = new OpenFileDialog { Filter = "Image Files|*.png;*.jpg;*.jpeg;*.bmp;*.gif" };
            if (dialog.ShowDialog() == true)
            {
                slot.SourcePath = dialog.FileName;
                (DataContext as AwakenWindowViewModel)?.RefreshEditorImageElements();
            }
        }

        private void LoadFolder_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new OpenFolderDialog { Title = "Select a folder containing 4-8 images" };

            if (dialog.ShowDialog() != true) return;

            var imageFiles = System.IO.Directory.GetFiles(dialog.FolderName)
                .Where(f => SupportedImageExtensions.Contains(System.IO.Path.GetExtension(f).ToLowerInvariant()))
                .OrderBy(f => f)
                .ToList();

            if (imageFiles.Count < 4)
            {
                MessageBox.Show($"Found {imageFiles.Count} image(s). Need 4-8 images.\n\nSupported: PNG, JPG, JPEG, BMP, GIF",
                    "Not Enough Images", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (imageFiles.Count > 8)
            {
                MessageBox.Show($"Found {imageFiles.Count} images. Only first 8 will be loaded.",
                    "Too Many Images", MessageBoxButton.OK, MessageBoxImage.Information);
                imageFiles = imageFiles.Take(8).ToList();
            }

            if (DataContext is not AwakenWindowViewModel vm) return;

            vm.ClearAllSlots();
            var slots = new[] { vm.Slot1, vm.Slot2, vm.Slot3, vm.Slot4, vm.Slot5, vm.Slot6, vm.Slot7, vm.Slot8 };
            for (int i = 0; i < imageFiles.Count; i++)
                slots[i].SourcePath = imageFiles[i];

            vm.RefreshEditorImageElements();
            MessageBox.Show($"Loaded {imageFiles.Count} images.", "Batch Load Complete", MessageBoxButton.OK, MessageBoxImage.Information);
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

        private void ClearAllText_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is not AwakenWindowViewModel vm) return;
            vm.Title = string.Empty;
            vm.Subtitle = string.Empty;
            vm.Footer = string.Empty;
            vm.Caption1.Text = string.Empty;
            vm.Caption2.Text = string.Empty;
            vm.Caption3.Text = string.Empty;
        }

        private void BringToFront(UIElement element)
        {
            if (element is Border border && border.Tag is EditorNumberElement)
            {
                // Number elements stay in their high Z-index range
                _currentNumberZIndex++;
                Canvas.SetZIndex(element, NumberElementBaseZIndex + _currentNumberZIndex);
            }
            else
            {
                // Non-number elements use their own counter, capped below NumberElementBaseZIndex
                _currentNonNumberZIndex++;
                if (_currentNonNumberZIndex >= NumberElementBaseZIndex)
                    _currentNonNumberZIndex = 1;
                Canvas.SetZIndex(element, _currentNonNumberZIndex);
            }
        }

        private List<Border> GetElementsAtPoint(Point point)
        {
            var allElements = _textElementBorders.Concat(_imageElementBorders).Concat(_numberElementBorders).ToList();
            var elementsAtPoint = new List<Border>();

            foreach (var element in allElements)
            {
                double left = Canvas.GetLeft(element);
                double top = Canvas.GetTop(element);
                if (double.IsNaN(left)) left = 0;
                if (double.IsNaN(top)) top = 0;

                if (new Rect(left, top, element.Width, element.Height).Contains(point))
                    elementsAtPoint.Add(element);
            }

            return elementsAtPoint;
        }

        private string GetElementDisplayName(FrameworkElement element) => element.Tag switch
        {
            EditorImageElement img => $"Image {img.SlotNumber} (Scale: {img.ItemImageRef?.Scale:F1}x)",
            EditorNumberElement num => $"Number {num.Number} (Scale: {num.Scale:F1}x)",
            string s => s,
            _ => "Unknown"
        };

        #region Drag and Drop

        private void DraggableElement_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (sender is not FrameworkElement element) return;

            ResetCycleState();
            BringToFront(element);

            _isDragging = true;
            _draggedElement = element;
            _dragStartPoint = e.GetPosition(EditorCanvas);
            _elementStartLeft = Canvas.GetLeft(element);
            _elementStartTop = Canvas.GetTop(element);
            if (double.IsNaN(_elementStartLeft)) _elementStartLeft = 0;
            if (double.IsNaN(_elementStartTop)) _elementStartTop = 0;
            element.CaptureMouse();

            if (DataContext is AwakenWindowViewModel vm)
                vm.SelectedElementName = GetElementDisplayName(element);

            e.Handled = true;
        }

        private void DraggableElement_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (sender is not FrameworkElement element) return;

            var clickPoint = e.GetPosition(EditorCanvas);
            bool isSameLocation = _lastCycleElements != null &&
                                  Math.Abs(clickPoint.X - _lastCyclePoint.X) < 5 &&
                                  Math.Abs(clickPoint.Y - _lastCyclePoint.Y) < 5;

            if (!isSameLocation)
            {
                _lastCycleElements = GetElementsAtPoint(clickPoint);
                _lastCyclePoint = clickPoint;
                _lastCycleIndex = 0;
            }

            if (_lastCycleElements == null || _lastCycleElements.Count <= 1)
            {
                e.Handled = true;
                return;
            }

            _lastCycleIndex = (_lastCycleIndex + 1) % _lastCycleElements.Count;
            var nextElement = _lastCycleElements[_lastCycleIndex];
            BringToFront(nextElement);

            if (DataContext is AwakenWindowViewModel vm)
                vm.SelectedElementName = $"{GetElementDisplayName(nextElement)} ({_lastCycleIndex + 1}/{_lastCycleElements.Count})";

            e.Handled = true;
        }

        private void DraggableElement_MouseMove(object sender, MouseEventArgs e)
        {
            if (!_isDragging || _draggedElement == null) return;
            var pt = e.GetPosition(EditorCanvas);
            Canvas.SetLeft(_draggedElement, _elementStartLeft + (pt.X - _dragStartPoint.X));
            Canvas.SetTop(_draggedElement, _elementStartTop + (pt.Y - _dragStartPoint.Y));
            UpdateElementPosition(_draggedElement, Canvas.GetLeft(_draggedElement), Canvas.GetTop(_draggedElement));
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
