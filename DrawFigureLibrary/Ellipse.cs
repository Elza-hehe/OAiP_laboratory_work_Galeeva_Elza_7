using System.Windows;
using System.Windows.Media;

namespace DrawFigureLibrary
{
    public class Ellipse : Figure
    {
        public Ellipse() { x = y = w = h = 0; }

        public Ellipse(int x, int y, int w, int h)
        {
            this.x = x;
            this.y = y;
            this.w = w;
            this.h = h;
        }

        public override void Draw(DrawingContext drawingContext)
        {
            double centerX = x + w / 2.0;
            double centerY = y + h / 2.0;
            double radiusX = w / 2.0;
            double radiusY = h / 2.0;

            drawingContext.DrawEllipse(null, Init.Pen, new Point(centerX, centerY), radiusX, radiusY);
        }

        public override void MoveTo(int deltaX, int deltaY)
        {
            if (Init.DrawingImage == null) return;

            if (x + deltaX >= 0 &&
                y + deltaY >= 0 &&
                x + deltaX + w <= Init.DrawingImage.ActualWidth &&
                y + deltaY + h <= Init.DrawingImage.ActualHeight)
            {
                x += deltaX;
                y += deltaY;
            }
        }
    }
}
