namespace DrawFigureLibrary
{
    public class Circle : Ellipse
    {
        public Circle() : base() { }

        public Circle(int x, int y, int radius)
            : base(x, y, radius * 2, radius * 2)
        { }

        public int Radius 
        {
            get
            {
                return w / 2;
            }
        }
        public bool SetRadius(int radius)
        {
            if (Init.DrawingImage == null) return false;
            if (radius < 0) return false;

            int d = radius * 2;
            if (x + d <= Init.DrawingImage.ActualWidth &&
                y + d <= Init.DrawingImage.ActualHeight)
            {
                w = d;
                h = d;
                return true;
            }

            return false;
        }
    }
}
