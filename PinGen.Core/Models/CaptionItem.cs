using System.ComponentModel;

namespace PinGen.Core.Models
{
    public class CaptionItem : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string name)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

        private string _text = string.Empty;
        public string Text
        {
            get => _text;
            set { _text = value; OnPropertyChanged(nameof(Text)); }
        }

        private int _fontSize = 20;
        public int FontSize
        {
            get => _fontSize;
            set { _fontSize = value; OnPropertyChanged(nameof(FontSize)); }
        }

        // Available font sizes for dropdown
        public static int[] AvailableFontSizes => [22, 24, 26, 28, 30, 32, 34, 36, 38, 40];
    }
}