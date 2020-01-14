using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ColorPicker
{
    public delegate void DrawBlockHandler(DrawBlockEventArgs e);
    public class DrawBlockEventArgs
    {
        private Point _location;
        private bool _isClear;
        public DrawBlockEventArgs(Point location,bool isClear = false)
        {
            _location = location;
            _isClear = isClear;
        }
        public Point BlockLocation
        {
            get { return _location; }
        }
        public bool IsClear
        {
            get { return _isClear; }
        }
    }
}
