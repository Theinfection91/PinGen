using System.ComponentModel;

namespace PinGen.Core.Models
{
    /// <summary>
    /// Represents a draggable element's position and size on the editor canvas.
    /// </summary>
    public class ElementPosition : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged(string name)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

        private double _x;
        private double _y;
        private double _width;
        private double _height;

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

        public double Width
        {
            get => _width;
            set { _width = value; OnPropertyChanged(nameof(Width)); }
        }

        public double Height
        {
            get => _height;
            set { _height = value; OnPropertyChanged(nameof(Height)); }
        }

        public ElementPosition() { }

        public ElementPosition(double x, double y, double width, double height)
        {
            X = x;
            Y = y;
            Width = width;
            Height = height;
        }

        /// <summary>
        /// Creates from a System.Windows.Rect
        /// </summary>
        public static ElementPosition FromRect(System.Windows.Rect rect)
            => new(rect.X, rect.Y, rect.Width, rect.Height);

        /// <summary>
        /// Converts to System.Windows.Rect for renderer
        /// </summary>
        public System.Windows.Rect ToRect()
            => new(X, Y, Width, Height);
    }
}