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
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace PinGen.App.ViewModels
{
    class MainWindowViewModel : INotifyPropertyChanged
    {
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

        public ObservableCollection<string> ItemImagePaths { get; } = new();
        public ObservableCollection<string> Captions { get; } = new();

        public ICommand RenderCommand { get; }

        private readonly IPinRenderer _pinRenderer;
        private readonly ITemplateProvider _templateProvider;
        private readonly IFileSaver _fileSaver;

        public MainWindowViewModel(IPinRenderer pinRenderer, ITemplateProvider templateProvider, IFileSaver fileSaver)
        {
            _pinRenderer = pinRenderer;
            _templateProvider = templateProvider;
            _fileSaver = fileSaver;

            RenderCommand = new RelayCommand(Render);

            // Seed captions
            Captions.Add("");
            Captions.Add("");
            Captions.Add("");

            // Test images
            ItemImagePaths.Add("Assets/TestImages/test1.jpg");
            ItemImagePaths.Add("Assets/TestImages/test2.jpg");
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
        protected void OnPropertyChanged(string name)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }

}
