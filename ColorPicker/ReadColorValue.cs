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
using System.Windows.Media.Imaging;

namespace ColorPicker
{
    class ReadColorValue
    {
        public DrawBlockHandler DrawBlockEvent;
        public ColorModel Model { get; } = new ColorModel();
        private Bitmap _Img;
        private Point Location = new Point();
        private int Index = -1;
        private Thread ReadValueThread;
        private ManualResetEvent _pauseEvent;
        private readonly Object ThisLock = new Object();
        private bool ThreadKey = true;
        public ReadColorValue(string imagePath = "Image/pop_ic_color.png")
        {
            for (int i = 0; i < 16; i++)
            {
                Brush defaultColor = Brushes.White;
                Model.ColorList.Add(new ColorData(defaultColor));
            }
            ReadValueThread = new Thread(ReadValue) { IsBackground = true };
            try
            {
                _Img = new Bitmap(imagePath);
            }
            catch (System.IO.FileNotFoundException) { }
        }

        public void StartReadPixel()
        {
            if ((ReadValueThread.ThreadState & ThreadState.WaitSleepJoin) == ThreadState.WaitSleepJoin)
            {
                _pauseEvent.Set();
            }
            else if ((ReadValueThread.ThreadState & ThreadState.Unstarted) == ThreadState.Unstarted)
            {
                ReadValueThread.Start();
                _pauseEvent = new ManualResetEvent(true);
            }
        }

        public void SuspendReadPixel()
        {
            _pauseEvent.Reset();
        }

        public void StopReadPixel()
        {
            ThreadKey = false;
            ReadValueThread.Abort();
        }

        public void UpdateInfo(Point newLocation, int index = -1)
        {
            lock (ThisLock)
            {
                Location = newLocation;
                if (index != -1)
                    Index = index;
            }
        }

        private void ReadValue()
        {
            Point tmpLocation = new Point();
            int tmpIndex;
            byte alphaMax = 255;
            while (ThreadKey)
            {
                _pauseEvent.WaitOne(Timeout.Infinite);
                if (tmpLocation != Location)
                {
                    tmpIndex = Index;
                    tmpLocation = Location;
                    Color pixelValue = new Color();
                    int x = (int)Math.Ceiling(tmpLocation.X);
                    int y = (int)Math.Ceiling(tmpLocation.Y);
                    try
                    {
                        pixelValue = _Img.GetPixel(x, y);
                    }
                    catch { }
                    if (pixelValue.A == alphaMax)
                    {
                        DrawBlockEvent?.Invoke(new DrawBlockEventArgs(new Point(x, y)));
                        if (tmpIndex > -1)
                        {
                            Application.Current.Dispatcher.Invoke(() =>
                            {
                                ColorData item = Model.ColorList[tmpIndex];
                                Brush brush = new SolidColorBrush(System.Windows.Media.Color.FromArgb(pixelValue.A, pixelValue.R, pixelValue.G, pixelValue.B));
                                item.ColorValue = brush;
                                item.Localtion = new Point(x, y);
                            });
                        }
                    }
                }
                Thread.Sleep(1);
            }
        }
    }
}
