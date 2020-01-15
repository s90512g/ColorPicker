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
using System.Threading;
using System.Windows;

namespace ColorPicker
{
    class ReadColorValue
    {
        public DrawBlockHandler DrawBlockEvent;
        public String Name { get; } = "123";
        public ColorModel Model { get; } = new ColorModel();
        private Bitmap _Img = new Bitmap(@"../../Image/pop_ic_color.png");
        public Point Location = new Point(0, 0);
        private Point _location = new Point(0, 0);
        public int index;
        private Thread ReadValueThread;
        public bool ThreadKey = true;
        public ReadColorValue()
        {
            for (int i = 0; i < 16; i++)
            {
                Brush defaultColor = Brushes.White;
                Model.ColorList.Add(new ColorData(defaultColor));
            }
            ReadValueThread = new Thread(ReadValue) { IsBackground = true };
        }
        public void StartReadPixelValue()
        {
            if ((ReadValueThread.ThreadState & ThreadState.Suspended) == ThreadState.Suspended)
            {
                ReadValueThread.Resume();
            }
            else if ((ReadValueThread.ThreadState & ThreadState.Unstarted) == ThreadState.Unstarted)
            {
                ReadValueThread.Start();
            }
        }
        public void StopReadPixelValue()
        {
            ReadValueThread.Suspend();
        }
        private void ReadValue()
        {
            while (ThreadKey)
            {
                if (_location != Location)
                {
                    _location = Location;
                    Color pixelValue = new Color();
                    int x = (int)Math.Ceiling(_location.X);
                    int y = (int)Math.Ceiling(_location.Y);
                    try
                    {
                        pixelValue = _Img.GetPixel(x, y);
                    }
                    catch { }
                    if (pixelValue.A == 255)
                    {
                        DrawBlockEvent?.Invoke(new DrawBlockEventArgs(new Point((int)Math.Ceiling(_location.X), (int)Math.Ceiling(_location.Y))));
                        Application.Current.Dispatcher.Invoke(() =>
                        {
                            ColorData item = Model.ColorList[index];
                            Brush brush = new SolidColorBrush(System.Windows.Media.Color.FromArgb(pixelValue.A, pixelValue.R, pixelValue.G, pixelValue.B));
                            item.ColorValue = brush;
                            item.Localtion = new Point((int)Math.Ceiling(_location.X), (int)Math.Ceiling(_location.Y));
                        });
                    }
                }
                Thread.Sleep(1);
            }
        }
    }
}
