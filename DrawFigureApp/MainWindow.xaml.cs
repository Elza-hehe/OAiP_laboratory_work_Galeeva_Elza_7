using DrawFigureLibrary;
using System;
using System.Drawing;
using System.Globalization;
using System.Security.Policy;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace DrawFigureApp
{
    public partial class MainWindow : Window
    {
        private RenderTargetBitmap? _renderTarget;
        private Polygon? _currentPolygon;
        private int _currentPoint = 0;
        private int _windowWidth = 800;
        private int _windowHeight = 600;

        public MainWindow()
        {
            InitializeComponent();
            Loaded += MainWindow_Loaded;
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            Init.DrawingImage = DrawingImage;

            int width = (int)DrawingImage.ActualWidth;
            int height = (int)DrawingImage.ActualHeight;

            if (width <= 0) width = 800;
            if (height <= 0) height = 500;

            _windowWidth = width;
            _windowHeight = height;

            _renderTarget = new RenderTargetBitmap(width, height, 96, 96, PixelFormats.Pbgra32);
            DrawingImage.Source = _renderTarget;

            RefreshFigureList();
            Redraw();
        }

        private static bool TryReadInt(string text, out int value)
            => int.TryParse(text, NumberStyles.Integer, CultureInfo.InvariantCulture, out value);

        private bool CheckBounds(int x, int y, int width, int height)
        {
            if (x < 0 || y < 0)
            {
                MessageBox.Show("фигура не может выходить за левую или верхнюю границу", "ошибка");
                return false;
            }

            if (x + width > _windowWidth || y + height > _windowHeight)
            {
                MessageBox.Show("фигура не может выходить за правую или нижнюю границу", "ошибка");
                return false;
            }

            return true;
        }

        private void AddFigure_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!TryReadInt(InputX.Text, out int x) ||
                    !TryReadInt(InputY.Text, out int y) ||
                    !TryReadInt(InputW.Text, out int w))
                {
                    MessageBox.Show("некорректный формат числа!", "ошибка!", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                _ = TryReadInt(InputH.Text, out int h);

                var type = (FigureType.SelectedItem as System.Windows.Controls.ComboBoxItem)?.Content?.ToString() ?? "";

                Figure fig;
                switch (type)
                {
                    case "прямоугольник":
                        if (!TryReadInt(InputH.Text, out h))
                        {
                            MessageBox.Show("для прямоугольника нужно H", "ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                            return;
                        }
                        if(!CheckBounds(x, y, w, h)) return;
                        fig = new DrawFigureLibrary.Rectangle(x, y, w, h);
                        break;

                    case "квадрат":
                        if (!CheckBounds(x, y, w, h)) return;
                        fig = new Square(x, y, w);
                        break;

                    case "эллипс":
                        if (!TryReadInt(InputH.Text, out h))
                        {
                            MessageBox.Show("для эллипса нужно H", "ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                            return;
                        }
                        if (!CheckBounds(x, y, w, h)) return;
                        fig = new Ellipse(x, y, w, h);
                        break;

                    case "окружность":
                        if (!CheckBounds(x, y, w, h)) return;
                        fig = new Circle(x, y, w);
                        break;

                    case "треугольник":
                        if (!TryReadInt(InputH.Text, out h))
                        {
                            MessageBox.Show("для треугольника нужно H", "ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                            return;
                        }
                        if (!CheckBounds(x, y, w, h)) return;
                        fig = new Triangle(new Point(x, y), new Point(x + w, y), new Point(x, y + h));
                        break;

                    case "фигурист":
                        if (w <= 0 || h <= 0)
                        {
                            MessageBox.Show("ширина и высота должны быть положительными", "ошибка");
                            return;
                        }

                        int size = w;
                        int _size = size;
                        int headDiameter = _size / 8;
                        int bodyHeight = _size * 3 / 5;
                        int legLength = _size * 2 / 5;
                        int _platformY = y;
                        int pelvisY = _platformY - legLength;
                        int bodyY = pelvisY - bodyHeight;
                        int headY = bodyY - headDiameter;
                        int ry = headY - headDiameter;


                        if (x < 0 || y < 0 || x + w + 50 > _windowWidth || y - h - 50 > _windowHeight || ry > _windowHeight)
                        {
                            MessageBox.Show("сложная фигура выходит за границы холста", "оишбка");
                            return;
                        }

                        fig = new Skater(x, y, w);
                        break;

                    default:
                        MessageBox.Show("не выбран тип фигуры", "ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                }

                ShapeContainer.AddFigure(fig);
                RefreshFigureList();
                Redraw();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"ошибка: {ex.Message}", "ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ClearAll_Click(object sender, RoutedEventArgs e)
        {
            ShapeContainer.ClearAll();
            ResetPolygonInput();
            RefreshFigureList();
            Redraw();
        }

        private void PolyAddPoint_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!TryReadInt(PolyCount.Text, out int count) || count <= 2)
                {
                    MessageBox.Show("кол-во вершин должно быть меньше 3", "ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (!TryReadInt(PolyX.Text, out int px) || !TryReadInt(PolyY.Text, out int py))
                {
                    MessageBox.Show("некорректный формат числа!", "ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (_currentPoint == 0)
                {
                    _currentPolygon = new Polygon(count);
                    PolyCount.IsEnabled = false;
                }

                if (_currentPolygon == null) return;

                if (_currentPoint < _currentPolygon.Points.Length)
                {
                    _currentPolygon.Points[_currentPoint] = new Point(px, py);
                    _currentPoint++;

                    if (_currentPoint == _currentPolygon.Points.Length)
                    {
                        PolyDraw.IsEnabled = true;
                        PolyAddPoint.IsEnabled = false;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"ошибка: {ex.Message}", "ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void PolyDraw_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (_currentPolygon == null)
                {
                    MessageBox.Show("полигон не создан", "ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
                foreach (Point p in _currentPolygon.Points)
                {
                    if (p.X < 0 || p.X > _windowWidth || p.Y < 0 || p.Y > _windowHeight)
                    {
                        MessageBox.Show("Все точки многокгольника должны быть внутри холста", "Ошибка");
                        PolyAddPoint.IsEnabled = true;
                        PolyDraw.IsEnabled = false;
                        return;
                    }
                }

                ShapeContainer.AddFigure(_currentPolygon);
                ResetPolygonInput();
                RefreshFigureList();
                Redraw();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"ошибка: {ex.Message}", "ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ResetPolygonInput()
        {
            _currentPolygon = null;
            _currentPoint = 0;
            PolyCount.IsEnabled = true;
            PolyAddPoint.IsEnabled = true;
            PolyDraw.IsEnabled = false;
        }

        private void FigureList_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
        }

        private Figure? SelectedFigure => FigureList.SelectedItem as Figure;

        private void MoveSelected_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var fig = SelectedFigure;
                if (fig == null)
                {
                    MessageBox.Show("фигура для перемещения не выбрана", "ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (!TryReadInt(MoveDX.Text, out int dx) || !TryReadInt(MoveDY.Text, out int dy))
                {
                    MessageBox.Show("некорректный формат числа", "ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                int oldX = fig.x;
                int oldY = fig.y;

                fig.x += dx;
                fig.y += dy;

                if (fig.x < 0 || fig.y < 0 ||
                    fig.x + fig.w > _windowWidth ||
                    fig.y + fig.h > _windowHeight)
                {
                    fig.x = oldX;
                    fig.y = oldY;
                    MessageBox.Show("Фигура не может выйти за границы холста", "Ошибка");
                    return;
                }

                fig.x = oldX;
                fig.y = oldY;
                fig.MoveTo(dx, dy);
                fig.MoveTo(dx, dy);
                Redraw();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"ошибка: {ex.Message}", "ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void DeleteSelected_Click(object sender, RoutedEventArgs e)
        {
            var fig = SelectedFigure;
            if (fig == null)
            {
                MessageBox.Show("фигура для удаления не найдена!", "ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            ShapeContainer.RemoveFigure(fig);
            RefreshFigureList();
            Redraw();
        }

        private void ResizeSelected_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var fig = SelectedFigure;
                if (fig == null)
                {
                    MessageBox.Show("фигура не выбрана", "ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (!TryReadInt(UniqueW.Text, out int newW) || !TryReadInt(UniqueH.Text, out int newH))
                {
                    MessageBox.Show("некорректный формат числа!", "ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (fig is DrawFigureLibrary.Rectangle r)
                {
                    if (!r.Resize(newW, newH))
                        MessageBox.Show("нельзя изменить размер, тогда выходит за границы", "ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                    Redraw();
                    return;
                }

                MessageBox.Show("Resize доступен только для прямоугольников и квадратов", "инфо", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"ошибка: {ex.Message}", "ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void SetRadiusSelected_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var fig = SelectedFigure;
                if (fig == null)
                {
                    MessageBox.Show("фигура не выбрана", "ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (!TryReadInt(UniqueW.Text, out int r))
                {
                    MessageBox.Show("некорректный формат числа", "ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (fig is Circle c)
                {
                    if (!c.SetRadius(r))
                        MessageBox.Show("нельзя изменить радиус, тогда выходит за границы", "ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                    Redraw();
                    return;
                }

                MessageBox.Show("SetRadius доступен только для окружности", "инфо", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"ошибка: {ex.Message}", "ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Redraw_Click(object sender, RoutedEventArgs e) => Redraw();

        private void RefreshFigureList()
        {
            FigureList.ItemsSource = null;
            FigureList.ItemsSource = ShapeContainer.FigureList;
            FigureList.DisplayMemberPath = null;
        }

        private void Redraw()
        {
            if (_renderTarget == null) return;

            DrawingVisual drawingVisual = new DrawingVisual();
            using (var dc = drawingVisual.RenderOpen())
            {
                foreach (var figure in ShapeContainer.FigureList)
                    figure.Draw(dc);
            }

            _renderTarget.Clear();
            _renderTarget.Render(drawingVisual);
            DrawingImage.Source = _renderTarget;
        }
    }
}
