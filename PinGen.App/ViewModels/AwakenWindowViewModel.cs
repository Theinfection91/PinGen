using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using PinGen.Core.Models;

namespace PinGen.App.ViewModels
{
    public class AwakenWindowViewModel : INotifyPropertyChanged
    {
        // INotifyPropertyChanged implementation
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string name)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

        // 8 fixed image slots
        public ItemImage Slot1 { get; } = new() { GroupName = "Slot1Tolerance" };
        public ItemImage Slot2 { get; } = new() { GroupName = "Slot2Tolerance" };
        public ItemImage Slot3 { get; } = new() { GroupName = "Slot3Tolerance" };
        public ItemImage Slot4 { get; } = new() { GroupName = "Slot4Tolerance" };
        public ItemImage Slot5 { get; } = new() { GroupName = "Slot5Tolerance" };
        public ItemImage Slot6 { get; } = new() { GroupName = "Slot6Tolerance" };
        public ItemImage Slot7 { get; } = new() { GroupName = "Slot7Tolerance" };
        public ItemImage Slot8 { get; } = new() { GroupName = "Slot8Tolerance" };

        // Footer toggle
        private bool _isFooterEnabled;
        public bool IsFooterEnabled
        {
            get => _isFooterEnabled;
            set { _isFooterEnabled = value; OnPropertyChanged(nameof(IsFooterEnabled)); }
        }

        // Background options
        private bool _useBackground1 = true;
        private bool _useBackground2;
        private bool _useCustomColor;

        public bool UseBackground1
        {
            get => _useBackground1;
            set { _useBackground1 = value; OnPropertyChanged(nameof(UseBackground1)); }
        }

        public bool UseBackground2
        {
            get => _useBackground2;
            set { _useBackground2 = value; OnPropertyChanged(nameof(UseBackground2)); }
        }

        public bool UseCustomColor
        {
            get => _useCustomColor;
            set { _useCustomColor = value; OnPropertyChanged(nameof(UseCustomColor)); }
        }

        // Color palette selections
        private bool _isWhiteSelected = true;
        private bool _isBlackSelected;
        private bool _isRedSelected;
        private bool _isBlueSelected;
        private bool _isGreenSelected;
        private bool _isYellowSelected;
        private bool _isPurpleSelected;
        private bool _isOrangeSelected;

        public bool IsWhiteSelected
        {
            get => _isWhiteSelected;
            set { _isWhiteSelected = value; OnPropertyChanged(nameof(IsWhiteSelected)); }
        }

        public bool IsBlackSelected
        {
            get => _isBlackSelected;
            set { _isBlackSelected = value; OnPropertyChanged(nameof(IsBlackSelected)); }
        }

        public bool IsRedSelected
        {
            get => _isRedSelected;
            set { _isRedSelected = value; OnPropertyChanged(nameof(IsRedSelected)); }
        }

        public bool IsBlueSelected
        {
            get => _isBlueSelected;
            set { _isBlueSelected = value; OnPropertyChanged(nameof(IsBlueSelected)); }
        }

        public bool IsGreenSelected
        {
            get => _isGreenSelected;
            set { _isGreenSelected = value; OnPropertyChanged(nameof(IsGreenSelected)); }
        }

        public bool IsYellowSelected
        {
            get => _isYellowSelected;
            set { _isYellowSelected = value; OnPropertyChanged(nameof(IsYellowSelected)); }
        }

        public bool IsPurpleSelected
        {
            get => _isPurpleSelected;
            set { _isPurpleSelected = value; OnPropertyChanged(nameof(IsPurpleSelected)); }
        }

        public bool IsOrangeSelected
        {
            get => _isOrangeSelected;
            set { _isOrangeSelected = value; OnPropertyChanged(nameof(IsOrangeSelected)); }
        }

        public AwakenWindowViewModel()
        {
        }

        public void ClearAllSlots()
        {
            Slot1.SourcePath = string.Empty;
            Slot2.SourcePath = string.Empty;
            Slot3.SourcePath = string.Empty;
            Slot4.SourcePath = string.Empty;
            Slot5.SourcePath = string.Empty;
            Slot6.SourcePath = string.Empty;
            Slot7.SourcePath = string.Empty;
            Slot8.SourcePath = string.Empty;

            Slot1.Scale = 1.0;
            Slot2.Scale = 1.0;
            Slot3.Scale = 1.0;
            Slot4.Scale = 1.0;
            Slot5.Scale = 1.0;
            Slot6.Scale = 1.0;
            Slot7.Scale = 1.0;
            Slot8.Scale = 1.0;
        }
    }
}
