using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
namespace ColorPicker
{
    public class ColorModel
    {
        public ColorModel()
        {
            ColorList = new ObservableCollection<ColorData>();
        }
        public ObservableCollection<ColorData> ColorList { get; private set; }

    }
}
