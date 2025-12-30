using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Microsoft.Win32;
using PinGen.App.ViewModels;
using PinGen.Core.Models;

namespace PinGen.App.Views
{
    /// <summary>
    /// Interaction logic for AwakenWindow.xaml
    /// </summary>
    public partial class AwakenWindow : Window
    {
        public AwakenWindow()
        {
            InitializeComponent();
            DataContext = new AwakenWindowViewModel();
        }

        private void BrowseImage_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var slot = button?.Tag as ItemImage;
            if (slot == null) return;

            var dialog = new OpenFileDialog
            {
                Filter = "Image Files|*.png;*.jpg;*.jpeg;*.bmp;*.gif"
            };

            if (dialog.ShowDialog() == true)
            {
                slot.SourcePath = dialog.FileName;
            }
        }

        private void ClearSlot_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var slot = button?.Tag as ItemImage;
            if (slot == null) return;

            slot.SourcePath = string.Empty;
            slot.Scale = 1.0;
        }

        private void ClearImages_Click(object sender, RoutedEventArgs e)
        {
            (DataContext as AwakenWindowViewModel)?.ClearAllSlots();
        }
    }
}
