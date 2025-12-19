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
        }

        private void RenderButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Create a hardcoded PinRequest for testing
                var request = new PinRequest
                {
                    Title = "Tiny Closet,\nBig Wardrobe",
                    Subtitle = "5 Space-Saving Hangers & Organizers from Amazon",
                    ItemImages = new List<ItemImage>
                    {
                        new ItemImage { SourcePath = "C:\\Chase\\CSharpProjects\\PinGen\\PinGen.App\\Assets\\TestImages\\testb1.jpg" },
                        new ItemImage { SourcePath = "C:\\Chase\\CSharpProjects\\PinGen\\PinGen.App\\Assets\\TestImages\\testb2.jpg" },
                        new ItemImage { SourcePath = "C:\\Chase\\CSharpProjects\\PinGen\\PinGen.App\\Assets\\TestImages\\testb3.jpg" },
                        new ItemImage { SourcePath = "C:\\Chase\\CSharpProjects\\PinGen\\PinGen.App\\Assets\\TestImages\\testb4.jpg" },
                        new ItemImage { SourcePath = "C:\\Chase\\CSharpProjects\\PinGen\\PinGen.App\\Assets\\TestImages\\testb5.jpg" }
                    },
                    Captions = new List<string>
                    {
                        "Some text displayed here.",
                        "We have some text here.",
                        "Final text will be here.",
                    },
                    Footer = "Test Footer Will Be Here.",
                };

                // Get template based on item count (for now, we use a hardcoded provider that requires five)
                var templateProvider = new HardcodedTemplateProvider();
                var template = templateProvider.GetTemplate(request.ItemImages.Count);

                // Setup services
                var fontLoader = new FontLoader();
                var imageLoader = new ImageLoader();
                var imageProcessor = new ImageSharpProcessor();
                var renderer = new PinRenderer(fontLoader, imageLoader, imageProcessor);
                var fileSaver = new FileSaver();

                // Render the pin board
                var renderedBitmap = renderer.Render(request, template);

                // Save the rendered image
                string outputFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Output");
                Directory.CreateDirectory(outputFolder);
                string outputPath = Path.Combine(outputFolder, $"pin_{DateTime.Now:yyyy-MM-dd_HH-mm-ss}.png");
                fileSaver.Save(renderedBitmap, outputPath);

                MessageBox.Show($"Pin board rendered and saved to:\n{outputPath}", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}