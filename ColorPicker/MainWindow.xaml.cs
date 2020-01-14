using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace ColorPicker
{
    /// <summary>
    /// MainWindow.xaml 的互動邏輯
    /// </summary>
    public partial class MainWindow : Window
    {
        private ReadColorValue ReadColor = new ReadColorValue();
        private EllipseGeometry Ellipse = new EllipseGeometry(new Point(0, 0), 3, 3);
        public MainWindow()
        {
            InitializeComponent();
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            DataContext = ReadColor;
            ReadColor.DrawBlockEvent += new DrawBlockHandler(DrawBlock);
            DrawBox.Data = Ellipse;

        }
        private void Image_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (CustomColorBox.SelectedIndex == -1)
                return;
            
            Point MousePosition = e.GetPosition(ColorImg);
            Console.WriteLine(MousePosition);
            ReadColor.ReadPixel(MousePosition, CustomColorBox.SelectedIndex);

        }
        private void DrawBlock(DrawBlockEventArgs e)
        {            
            Ellipse.Center = e.BlockLocation;
            DrawBox.Data = Ellipse;
        }

        private void CustomColorBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (CustomColorBox.SelectedItem is ColorData)
            {
                ColorData item = CustomColorBox.SelectedItem as ColorData;
                if(item.Localtion.X == -1 || item.Localtion.Y == -1)
                {
                    DrawBox.Data = null;
                }
                else
                {
                    Ellipse.Center = item.Localtion;
                    DrawBox.Data = Ellipse;
                }
            }

        }
    }
}
