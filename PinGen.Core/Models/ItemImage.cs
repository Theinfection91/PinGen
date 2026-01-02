using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Text;

namespace PinGen.Core.Models
{
    public class ItemImage : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string name)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

        private string _sourcePath = string.Empty;
        public string SourcePath
        {
            get => _sourcePath;
            set { _sourcePath = value; OnPropertyChanged(nameof(SourcePath)); OnPropertyChanged(nameof(FileName)); OnPropertyChanged(nameof(HasImage)); }
        }

        private double _scale = 1.0;
        public double Scale
        {
            get => _scale;
            set { _scale = value; OnPropertyChanged(nameof(Scale)); }
        }

        private bool _enableWhiteRemoval = true;
        private bool _isOption1Selected = true;       

        public string FileName => Path.GetFileName(SourcePath);
        public bool HasImage => !string.IsNullOrEmpty(SourcePath);

        

        public string? AltText { get; set; }

        // Unique group name for radio buttons in each slot
        public string GroupName { get; set; } = Guid.NewGuid().ToString();

        // White removal settings
        public bool EnableWhiteRemoval
        {
            get => _enableWhiteRemoval;
            set { _enableWhiteRemoval = value; OnPropertyChanged(nameof(EnableWhiteRemoval)); }
        }

        // Single radio option for now (placeholder for future tolerance options)
        public bool IsOption1Selected
        {
            get => _isOption1Selected;
            set { _isOption1Selected = value; OnPropertyChanged(nameof(IsOption1Selected)); }
        }
    }
}
