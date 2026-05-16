using System.Collections.Generic;

namespace DrawFigureLibrary
{
    public static class ShapeContainer
    {
        public static List<Figure> FigureList { get; set; } = new List<Figure>();

        public static void AddFigure(Figure figure)
        {
            FigureList.Add(figure);
        }

        public static void RemoveFigure(Figure figure)
        {
            FigureList.Remove(figure);
        }

        public static void ClearAll()
        {
            FigureList.Clear();
        }
    }
}
