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
using System.Windows;
using System.Windows.Input;

namespace PinGen.App.ViewModels
{
    class MainWindowViewModel : INotifyPropertyChanged
    {
        // INotifyPropertyChanged implementation
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string name)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

        // Properties bound to UI
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
            //ItemImagePaths.Add("Assets/TestImages/test1.jpg");
            //ItemImagePaths.Add("Assets/TestImages/test2.jpg");
            //ItemImagePaths.Add("Assets/TestImages/test3.jpg");
            //ItemImagePaths.Add("Assets/TestImages/test4.jpg");
            //ItemImagePaths.Add("Assets/TestImages/test5.jpg");
        }

        private void Render()
        {
            try
            {
                var request = new PinRequest
                {
                    Title = Title,
                    Subtitle = Subtitle,
                    Footer = Footer,
                    ItemImages = ItemImagePaths.Select(p => new ItemImage { SourcePath = p }).ToList(),
                    Captions = Captions.ToList()

                    // Need hardcoded data for testing that Ill comment in and out later
                    //Title = "Sample Pin Title Here,\nLine Breaks When Using Enter",
                    //Subtitle = "This is a subtitle for the sample pin being generated.",
                    //Footer = "Bunch stuff here in the footer area that'll be optional later.",
                    //ItemImages = ItemImagePaths.Select(p => new ItemImage { SourcePath = p }).ToList(),
                    //Captions = new List<string>
                    //{
                    //    "Snappy quick zinger here.",
                    //    "You gotta freaking get it.",
                    //    "Omfg it's just amazing."
                    //}
                };

                var template = _templateProvider.GetTemplate(request.ItemImages.Count);
                var bitmap = _pinRenderer.Render(request, template);

                var outputPath = Path.Combine("Output", $"pin_{DateTime.Now:HH-mm-ss}.png");
                if (!Directory.Exists("Output"))
                {
                    Directory.CreateDirectory("Output");
                }
                _fileSaver.Save(bitmap, outputPath);

                //ItemImagePaths.Clear();
                MessageBox.Show($"Pin board generated and saved to {outputPath}", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (NotImplementedException ex)
            {
                // Open error message box
                MessageBox.Show($"Feature not implemented: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public void AddImagePath(string path)
        {
            if (!ItemImagePaths.Contains(path))
                ItemImagePaths.Add(path);
        }

        public void ClearAllText()
        {
            Title = string.Empty;
            Subtitle = string.Empty;
            Footer = string.Empty;
            Captions.Clear();
            Captions.Add("");
            Captions.Add("");
            Captions.Add("");
        }
    }

}
