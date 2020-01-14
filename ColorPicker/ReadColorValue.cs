using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using Brush = System.Windows.Media.Brush;
using Brushes = System.Windows.Media.Brushes;
using Point = System.Windows.Point;
using Color = System.Drawing.Color;
using System.Windows.Media;

namespace ColorPicker
{
    class ReadColorValue
    {
        public DrawBlockHandler DrawBlockEvent;
        public String Name { get; } = "123";
        public ColorModel Model { get; } = new ColorModel();
        private Bitmap _Img = new Bitmap(@"../../Image/pop_ic_color.png");
        public ReadColorValue()
        {
            Brush defaultColor = Brushes.White;
            for (int i = 0; i < 16; i++)
            {
                Model.ColorList.Add(new ColorData(defaultColor));
            }
        }
        public void ReadPixel(Point locatoin, int index)
        {
            Color pixelValue = new Color();
            try
            {
                pixelValue = _Img.GetPixel((int)Math.Ceiling(locatoin.X), (int)Math.Ceiling(locatoin.Y));
            }
            catch
            {
                DrawBlockEvent?.Invoke(new DrawBlockEventArgs(locatoin, true));
                return;
            }
            if (pixelValue.A == 0)
            {
                return;
            }
            ColorData item = Model.ColorList[index];
            Brush brush = new SolidColorBrush(System.Windows.Media.Color.FromArgb(pixelValue.A, pixelValue.R, pixelValue.G, pixelValue.B));
            item.ColorValue = brush;
            item.Localtion = new Point((int)Math.Ceiling(locatoin.X), (int)Math.Ceiling(locatoin.Y));
            DrawBlockEvent?.Invoke(new DrawBlockEventArgs(item.Localtion));
        }
    }
}
