using System.ComponentModel;

namespace PinGen.Core.Models
{
    /// <summary>
    /// Represents a draggable number overlay element in the editor canvas.
    /// </summary>
    public class EditorNumberElement : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged(string name)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

        private int _number;
        private double _x;
        private double _y;
        private double _scale = 1.0;

        public int Number
        {
            get => _number;
            set { _number = value; OnPropertyChanged(nameof(Number)); }
        }

        public double X
        {
            get => _x;
            set { _x = value; OnPropertyChanged(nameof(X)); }
        }

        public double Y
        {
            get => _y;
            set { _y = value; OnPropertyChanged(nameof(Y)); }
        }

        public double Scale
        {
            get => _scale;
            set { _scale = value; OnPropertyChanged(nameof(Scale)); OnPropertyChanged(nameof(FontSize)); OnPropertyChanged(nameof(Size)); }
        }

        /// <summary>
        /// Calculated font size based on scale (base 48)
        /// </summary>
        public double FontSize => 48 * Scale;

        /// <summary>
        /// Calculated box size based on scale (base 70)
        /// </summary>
        public double Size => 70 * Scale;

        /// <summary>
        /// Reference to the associated EditorImageElement
        /// </summary>
        public EditorImageElement? ImageElementRef { get; set; }
    }
}