using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace DrawFigureLibrary
{
    public abstract class Figure
    {
        public int x;
        public int y;
        public int w;
        public int h;

        public abstract void Draw(DrawingContext drawingContext);
        public abstract void MoveTo(int deltaX, int deltaY);

        public override string ToString()
            => $"{GetType().Name}  x={x} y={y} w={w} h={h}";
    }
}