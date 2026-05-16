using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace DrawFigureApp
{
    public static class RenderTargetBitmapExtensions
    {
        public static void Clear(this RenderTargetBitmap bmp)
        {
            var dv = new DrawingVisual();
            using (var dc = dv.RenderOpen())
            {
                dc.DrawRectangle(Brushes.Transparent, null, new Rect(0, 0, bmp.PixelWidth, bmp.PixelHeight));
            }
            bmp.Render(dv);
        }
    }
}
