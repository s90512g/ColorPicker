using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.ComponentModel;
using System.Windows;

namespace ColorPicker
{
    public class ColorData : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private Brush _colorValue;
        private Point _location;
        public ColorData(Brush color)
        {
            _location = new Point(-1, -1);
            _colorValue = color;
        }
        public Brush ColorValue
        {
            get { return _colorValue; }
            set
            {
                if (_colorValue != value)
                {
                    _colorValue = value;
                    NotifyPropertyChanged("ColorValue");
                }
            }
        }
        public Point Localtion
        {
            get { return _location; }
            set { _location = value; }
        }
        void NotifyPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
