using System;
using System.Linq;
using System.Windows;
using System.Windows.Media;

namespace DrawFigureLibrary
{
    public class Polygon : Figure
    {
        public Point[] Points {get; set;}
        public Polygon(int vertexCount)
        {
            Points = new Point[vertexCount];
        }
        public Polygon(Point[] points)
        {
            Points = points ?? throw new ArgumentNullException(nameof(points));
        }
        public override void Draw(DrawingContext drawingContext)
        {
            if (Points == null || Points.Length == 0)
                return;

            StreamGeometry geometry = new StreamGeometry();
            using (var ctx = geometry.Open())
            {
                ctx.BeginFigure(Points[0], true, true);
                ctx.PolyLineTo(Points, true, false);
            }
            drawingContext.DrawGeometry(null, Init.Pen, geometry);
        }

        public override void MoveTo(int deltaX, int deltaY)
        {
            if (Points == null || Points.Length == 0)
                return;

            double maxX = Init.DrawingImage.ActualWidth;
            double maxY = Init.DrawingImage.ActualHeight;

            foreach (var p in Points)
            {
                double newX = p.X + deltaX;
                double newY = p.Y + deltaY;
                if (newX < 0 || newX > maxX || newY < 0 || newY > maxY)
                    return;
            }
            for (int i = 0; i < Points.Length; i++)
            {
                Points[i] = new Point(Points[i].X + deltaX, Points[i].Y + deltaY);
            }
        }
    }
}
