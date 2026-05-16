using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows;

namespace DrawFigureLibrary
{
    public class Skater : Figure
    {
        private int _platformX, _platformY, _size;

        public Rectangle Platform { get; private set; }
        public Rectangle Body { get; private set; }
        public Circle Head { get; private set; }

        public Polygon LeftArm { get; private set; }
        public Polygon LeftHand { get; private set; }
        public Polygon RightArm { get; private set; }
        public Polygon RightHand { get; private set; }

        public Polygon RightLeg { get; private set; }      
        public Polygon RightFoot { get; private set; }
        public Polygon LeftLeg { get; private set; }     
        public Polygon LeftFoot { get; private set; }

        public Skater(int x, int y, int size)
        {
            _platformX = x;
            _platformY = y;
            _size = size;
            UpdateParts();
        }

        private void UpdateParts()
        {
            int platformWidth = _size * 3;
            int platformHeight = _size / 10;
            int bodyWidth = _size / 16;
            int bodyHeight = _size * 3 / 5;
            int headDiameter = _size / 8;
            int armLength = _size * 2 / 5;
            int armWidth = _size / 20;
            int legLength = _size * 2 / 5;
            int legWidth = _size / 18;
            int triangleSize = armWidth * 2;

            Platform = new Rectangle(_platformX, _platformY, platformWidth, platformHeight);

            int centerX = _platformX + platformWidth / 2;
            int pelvisY = _platformY - legLength;
            int pelvisX = centerX;

            int bodyX = pelvisX - bodyWidth / 2;
            int bodyY = pelvisY - bodyHeight;
            Body = new Rectangle(bodyX, bodyY, bodyWidth, bodyHeight);

            int headX = pelvisX - headDiameter / 2;
            int headY = bodyY - headDiameter;
            Head = new Circle(headX, headY, headDiameter);

            int shoulderY = bodyY + bodyHeight / 2;
            int leftShoulderX = pelvisX - bodyWidth / 2;
            int rightShoulderX = pelvisX + bodyWidth / 2;

            Point leftArmStart = new Point(leftShoulderX, shoulderY);
            Point leftArmEnd = new Point(leftShoulderX - armLength, shoulderY);
            LeftArm = CreateRotatedRectangle(leftArmStart, leftArmEnd, armWidth);
            LeftHand = CreateTriangle(leftArmEnd, new Vector(-triangleSize, 0), armWidth);

            double angle45 = Math.PI / 4;
            Vector rightArmDir = new Vector(armLength * Math.Cos(angle45), -armLength * Math.Sin(angle45));
            Point rightArmStart = new Point(rightShoulderX, shoulderY);
            Point rightArmEnd = rightArmStart + rightArmDir;
            RightArm = CreateRotatedRectangle(rightArmStart, rightArmEnd, armWidth);
            RightHand = CreateTriangle(rightArmEnd, new Vector(0, -triangleSize), armWidth);

            int leftHipX = pelvisX - bodyWidth / 3;
            int rightHipX = pelvisX + bodyWidth / 3;
            Point leftHip = new Point(leftHipX, pelvisY);
            Point rightHip = new Point(rightHipX, pelvisY);

            Point rightFootEnd = new Point(rightHipX, _platformY);
            RightLeg = CreateRotatedRectangle(rightHip, rightFootEnd, legWidth);
            RightFoot = CreateTriangle(rightFootEnd, new Vector(0, triangleSize), legWidth);

            Vector leftLegDir = new Vector(legLength * Math.Cos(angle45), -legLength * Math.Sin(angle45));
            Point leftFootEnd = leftHip + leftLegDir;
            LeftLeg = CreateRotatedRectangle(leftHip, leftFootEnd, legWidth);
            LeftFoot = CreateTriangle(leftFootEnd, new Vector(0, triangleSize), legWidth);

            var allParts = new Figure[]
            {
                Platform, Body, Head,
                LeftArm, LeftHand, RightArm, RightHand,
                RightLeg, RightFoot, LeftLeg, LeftFoot
            };

            int minX = allParts.Min(p => p.x);
            int minY = allParts.Min(p => p.y);
            int maxX = allParts.Max(p => p.x + p.w);
            int maxY = allParts.Max(p => p.y + p.h);

            x = minX;
            y = minY;
            w = maxX - minX;
            h = maxY - minY;
        }

        private Polygon CreateRotatedRectangle(Point start, Point end, int width)
        {
            Vector dir = end - start;
            if (dir.Length == 0)
                return new Polygon(new Point[0]);

            Vector perp = new Vector(-dir.Y, dir.X);
            perp.Normalize();
            perp *= width / 2.0;

            Point[] points = new Point[4]
            {
                start + perp,
                start - perp,
                end - perp,
                end + perp
            };
            return new Polygon(points);
        }

        private Polygon CreateTriangle(Point baseCenter, Vector direction, int width)
        {
            if (direction.Length == 0)
                return new Polygon(new Point[0]);

            Vector perp = new Vector(-direction.Y, direction.X);
            perp.Normalize();
            perp *= width / 2.0;

            Point leftBase = baseCenter - perp;
            Point rightBase = baseCenter + perp;
            Point tip = baseCenter + direction;

            return new Polygon(new Point[] { leftBase, rightBase, tip });
        }

        public override void Draw(DrawingContext drawingContext)
        {
            Platform.Draw(drawingContext);
            Body.Draw(drawingContext);
            Head.Draw(drawingContext);
            LeftArm.Draw(drawingContext);
            LeftHand.Draw(drawingContext);
            RightArm.Draw(drawingContext);
            RightHand.Draw(drawingContext);
            RightLeg.Draw(drawingContext);
            RightFoot.Draw(drawingContext);
            LeftLeg.Draw(drawingContext);
            LeftFoot.Draw(drawingContext);
        }

        public override void MoveTo(int deltaX, int deltaY)
        {
            if (Init.DrawingImage == null) return;

            int newX = _platformX + deltaX;
            int newY = _platformY + deltaY;
            int oldX = _platformX;
            int oldY = _platformY;

            _platformX = newX;
            _platformY = newY;
            UpdateParts();

            if (x >= 0 && y >= 0 &&
                x + w <= Init.DrawingImage.ActualWidth &&
                y + h <= Init.DrawingImage.ActualHeight)
            {
            }
            else
            {
                _platformX = oldX;
                _platformY = oldY;
                UpdateParts();
            }
        }
    }
}
