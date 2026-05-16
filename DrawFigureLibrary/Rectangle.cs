using System.Windows;
using System.Windows.Media;

namespace DrawFigureLibrary
{
    public class Rectangle : Figure
    {
        public Rectangle() {x = y = w = h = 0;}

        public Rectangle(int x, int y, int w, int h)
        {
            this.x = x;
            this.y = y;
            this.w = w;
            this.h = h;
        }

        public override void Draw(DrawingContext drawingContext)
        {
            var rect = new Rect(x, y, w, h);
            drawingContext.DrawRectangle(null, Init.Pen, rect);
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
        public bool Resize(int newW, int newH)
        {
            if (Init.DrawingImage == null) return false;
            if (newW < 0 || newH < 0) return false;

            if (x + newW <= Init.DrawingImage.ActualWidth &&
                y + newH <= Init.DrawingImage.ActualHeight)
            {
                w = newW;
                h = newH;
                return true;
            }

            return false;
        }
    }
}
