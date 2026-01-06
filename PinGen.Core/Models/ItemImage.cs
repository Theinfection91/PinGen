using System;
using System.ComponentModel;
using System.IO;

namespace PinGen.Core.Models
{
    public class ItemImage : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged(string name)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

        private string _sourcePath = string.Empty;
        private double _scale = 1.0;
        private double _numberScale = 1.0;
        private bool _enableWhiteRemoval = true;
        private bool _isOption1Selected = true;

        public string SourcePath
        {
            get => _sourcePath;
            set { _sourcePath = value; OnPropertyChanged(nameof(SourcePath)); OnPropertyChanged(nameof(FileName)); OnPropertyChanged(nameof(HasImage)); }
        }

        public double Scale
        {
            get => _scale;
            set { _scale = value; OnPropertyChanged(nameof(Scale)); }
        }

        public double NumberScale
        {
            get => _numberScale;
            set { _numberScale = value; OnPropertyChanged(nameof(NumberScale)); }
        }

        public string FileName => Path.GetFileName(SourcePath);
        public bool HasImage => !string.IsNullOrEmpty(SourcePath);

        public string? AltText { get; set; }
        public string GroupName { get; set; } = Guid.NewGuid().ToString();

        public bool EnableWhiteRemoval
        {
            get => _enableWhiteRemoval;
            set { _enableWhiteRemoval = value; OnPropertyChanged(nameof(EnableWhiteRemoval)); }
        }

        public bool IsOption1Selected
        {
            get => _isOption1Selected;
            set { _isOption1Selected = value; OnPropertyChanged(nameof(IsOption1Selected)); }
        }
    }
}
