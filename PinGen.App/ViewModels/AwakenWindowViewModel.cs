using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace PinGen.App.ViewModels
{
    public class AwakenWindowViewModel : INotifyPropertyChanged
    {
        // INotifyPropertyChanged implementation
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string name)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

        // Properties bound to UI
    }
}
