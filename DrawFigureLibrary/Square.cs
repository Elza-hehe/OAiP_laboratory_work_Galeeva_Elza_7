namespace DrawFigureLibrary
{
    public class Square : Rectangle
    {
        public Square() : base() { }

        public Square(int x, int y, int side)
            : base(x, y, side, side)
        { }
    }
}
