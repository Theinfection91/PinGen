using Microsoft.Win32;
using PinGen.App.ViewModels;
using PinGen.Core.Models;
using PinGen.ImageProcessing.Interfaces;
using PinGen.ImageProcessing.Services;
using PinGen.IO.Services;
using PinGen.Rendering.Services;
using PinGen.Templates.Services;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;

namespace PinGen.App
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = new MainWindowViewModel(
                new PinRenderer(
                    new FontLoader(),
                    new ImageLoader(),
                    new ImageSharpProcessor()),
                new HardcodedTemplateProvider(),
                new FileSaver());
        }

        private void AddItemImages_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new OpenFileDialog
            {
                Filter = "Image Files|*.png;*.jpg;*.jpeg;*.bmp;*.gif",
                Multiselect = true
            };

            bool? result = dialog.ShowDialog();
            if (result == true)
            {
                foreach (var path in dialog.FileNames)
                    (DataContext as MainWindowViewModel)?.AddImagePath(path);
            }
        }
    }
}