using PinGen.App.Commands;
using PinGen.Core.Models;
using PinGen.IO.Interfaces;
using PinGen.Rendering.Interfaces;
using PinGen.Templates.Interfaces;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Text;
using System.Windows.Input;

namespace PinGen.App.ViewModels
{
    class MainWindowViewModel : INotifyPropertyChanged
    {
        public string Title { get; set; }
        public string Subtitle { get; set; }
        public string Footer { get; set; }

        public ObservableCollection<string> ItemImagePaths { get; } = new();
        public ObservableCollection<string> Captions { get; } = new();

        public ICommand RenderCommand { get; set; }

        private readonly IPinRenderer _pinRenderer;
        private readonly ITemplateProvider _templateProvider;
        private readonly IFileSaver _fileSaver;

        public MainWindowViewModel(IPinRenderer pinRenderer, ITemplateProvider templateProvider, IFileSaver fileSaver)
        {
            _pinRenderer = pinRenderer;
            _templateProvider = templateProvider;
            _fileSaver = fileSaver;

            RenderCommand = new RelayCommand(Render);

            //new ItemImage { SourcePath = "C:\\Chase\\CSharpProjects\\PinGen\\PinGen.App\\Assets\\TestImages\\test1.jpg" },
            //            //new ItemImage { SourcePath = "C:\\Chase\\CSharpProjects\\PinGen\\PinGen.App\\Assets\\TestImages\\test2.jpg" },
            //            new ItemImage { SourcePath = "C:\\Chase\\CSharpProjects\\PinGen\\PinGen.App\\Assets\\TestImages\\test3.jpg" },
            //            new ItemImage { SourcePath = "C:\\Chase\\CSharpProjects\\PinGen\\PinGen.App\\Assets\\TestImages\\test4.jpg" },
            //            new ItemImage { SourcePath = "C:\\Chase\\CSharpProjects\\PinGen\\PinGen.App\\Assets\\TestImages\\test5.jpg" }
            // Add some default test data for item image paths
            ItemImagePaths.Add("Assets/TestImages/test1.jpg");
            ItemImagePaths.Add("Assets/TestImages/test3.jpg");
            ItemImagePaths.Add("Assets/TestImages/test4.jpg");
            ItemImagePaths.Add("Assets/TestImages/test5.jpg");
        }

        private void Render()
        {
            var request = new PinRequest
            {
                Title = Title,
                Subtitle = Subtitle,
                Footer = Footer,
                ItemImages = ItemImagePaths.Select(p => new ItemImage { SourcePath = p }).ToList(),
                Captions = Captions.ToList()
            };

            var template = _templateProvider.GetTemplate(request.ItemImages.Count);
            var bitmap = _pinRenderer.Render(request, template);

            var outputPath = Path.Combine("Output", $"pin_{DateTime.Now:HH-mm-ss}.png");
            _fileSaver.Save(bitmap, outputPath);
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
