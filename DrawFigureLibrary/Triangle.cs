using System.Windows;

namespace DrawFigureLibrary
{
    public class Triangle : Polygon
    {
        public Triangle(Point p1, Point p2, Point p3) : base(new[] { p1, p2, p3 })
        {
        }
    }
}