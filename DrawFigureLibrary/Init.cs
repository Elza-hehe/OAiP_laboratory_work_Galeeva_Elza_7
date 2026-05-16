using System.Windows.Controls;
using System.Windows.Media;

namespace DrawFigureLibrary
{
    public static class Init
    {
        public static Pen Pen = new Pen(Brushes.Black, 2);
        public static Image? DrawingImage { get; set; }
    }
}
