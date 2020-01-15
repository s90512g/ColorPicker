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
        public ColorModel Model { get; } = new ColorModel();
        private Bitmap _Img = new Bitmap(@"../../Image/pop_ic_color.png");
        private Point Location = new Point(0, 0);
        private Point tmpLocation;
        private int _index = -1;
        private Thread ReadValueThread;
        private ManualResetEvent _pauseEvent;
        private readonly Object ThisLock = new Object();
        private bool ThreadKey = true;
        private readonly byte AlphaMax = 255;
        public ReadColorValue()
        {
            for (int i = 0; i < 16; i++)
            {
                Brush defaultColor = Brushes.White;
                Model.ColorList.Add(new ColorData(defaultColor));
            }
            ReadValueThread = new Thread(ReadValue) { IsBackground = true };
        }
        public void StartReadPixelValue(int index)
        {
            if ((ReadValueThread.ThreadState & ThreadState.WaitSleepJoin) == ThreadState.WaitSleepJoin)
            {
                _index = index;
                _pauseEvent.Set();
            }
            else if ((ReadValueThread.ThreadState & ThreadState.Unstarted) == ThreadState.Unstarted)
            {
                _index = index;
                ReadValueThread.Start();
                tmpLocation = new Point(0, 0);
                _pauseEvent = new ManualResetEvent(true);
            }
        }
        public void StopReadPixelValue()
        {
            _pauseEvent.Reset();
        }
        public void UpdateLocation(Point newLocation)
        {
            lock (ThisLock)
            {
                Location = newLocation;
            }
        }
        private void ReadValue()
        {
            while (ThreadKey)
            {
                _pauseEvent.WaitOne(Timeout.Infinite);
                if (tmpLocation != Location && _index != -1)
                {
                    tmpLocation = Location;
                    Color pixelValue = new Color();
                    int x = (int)Math.Ceiling(tmpLocation.X);
                    int y = (int)Math.Ceiling(tmpLocation.Y);
                    try
                    {
                        pixelValue = _Img.GetPixel(x, y);
                    }
                    catch { }
                    if (pixelValue.A == AlphaMax)
                    {
                        DrawBlockEvent?.Invoke(new DrawBlockEventArgs(new Point((int)Math.Ceiling(tmpLocation.X), (int)Math.Ceiling(tmpLocation.Y))));
                        Application.Current.Dispatcher.Invoke(() =>
                        {
                            ColorData item = Model.ColorList[_index];
                            Brush brush = new SolidColorBrush(System.Windows.Media.Color.FromArgb(pixelValue.A, pixelValue.R, pixelValue.G, pixelValue.B));
                            item.ColorValue = brush;
                            item.Localtion = new Point((int)Math.Ceiling(tmpLocation.X), (int)Math.Ceiling(tmpLocation.Y));
                        });
                    }
                }
                Thread.Sleep(1);
            }
        }
    }
}
