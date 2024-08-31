using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using Mugen.Core;
using System.Xml.Serialization;

namespace Mugen.Physics
{
    public struct Point2
    {
        public static readonly Point2 Zero = new Point2();
        public static readonly Point2 NaN = new Point2(float.NaN, float.NaN);

        public float X;
        public float Y;
        public Point2(float x, float y)
        {
            X = x;
            Y = y;
        }
        public static Point2 Minimum(Point2 first, Point2 second)
        {
            return new Point2(first.X < second.X ? first.X : second.X,
                first.Y < second.Y ? first.Y : second.Y);
        }
        public static Point2 Maximum(Point2 first, Point2 second)
        {
            return new Point2(first.X > second.X ? first.X : second.X,
                first.Y > second.Y ? first.Y : second.Y);
        }
        public static implicit operator Vector2(Point2 point)
        {
            return new Vector2(point.X, point.Y);
        }
        public static implicit operator Point2(Vector2 vector)
        {
            return new Point2(vector.X, vector.Y);
        }
        public static implicit operator Point2(Point point)
        {
            return new Point2(point.X, point.Y);
        }
        public override string ToString()
        {
            return $"({X}, {Y})";
        }
        internal string DebugDisplayString => ToString();
    }

    public struct Size2
    {
        public static readonly Size2 Empty = new Size2();

        public float Width;
        public float Height;

        public bool IsEmpty => (Width == 0) && (Height == 0);
        public Size2(float width, float height)
        {
            Width = width;
            Height = height;
        }
    }

    public struct Line
    {
        public Vector2 A;
        public Vector2 B;

        public Line(Vector2 a, Vector2 b)
        {
            A = a;
            B = b;
        }
        public Line(float xA, float yA, float xB, float yB)
        {
            A = new Vector2(xA, yA);
            B = new Vector2(xB, yB);
        }
    }

    public struct Triangle
    {
        public Vector3 A;
        public Vector3 B;
        public Vector3 C;
    }
    public struct RectangleF
    {
        public static readonly RectangleF Empty = new RectangleF();

        public const int VertexTopLeft = 0;
        public const int VertexTopCenter = 1;
        public const int VertexTopRight = 2;
        public const int VertexRightMiddle = 3;
        public const int VertexBottomRight = 4;
        public const int VertexBottomCenter = 5;
        public const int VertexBottomLeft = 6;
        public const int VertexLeftMiddle = 7;

        public float X;
        public float Y;
        public float Width;
        public float Height;

        public float Left => X;

        public float Right => X + Width;

        public float Top => Y;

        public float Bottom => Y + Height;

        public bool IsEmpty => Width.Equals(0) && Height.Equals(0) && X.Equals(0) && Y.Equals(0);
        public Point2 Position
        {
            get { return new Point2(X, Y); }
            set
            {
                X = value.X;
                Y = value.Y;
            }
        }
        public Size2 Size
        {
            get { return new Size2(Width, Height); }
            set
            {
                Width = value.Width;
                Height = value.Height;
            }
        }
        public Point2 Center => new Point2(X + Width * 0.5f, Y + Height * 0.5f);
        public Point2 TopLeft => new Point2(X, Y);
        public Point2 TopCenter => new Point2(X + Width/2, Y);
        public Point2 TopRight => new Point2(X + Width, Y);
        public Point2 RightMiddle => new Point2(X + Width, Y + Height/2);
        public Point2 BottomRight => new Point2(X + Width, Y + Height);
        public Point2 BottomCenter => new Point2(X + Width/2, Y + Height);
        public Point2 BottomLeft => new Point2(X, Y + Height);
        public Point2 LeftMiddle => new Point2(X, Y + Height/2);
        public RectangleF(float x, float y, float width, float height)
        {
            X = x;
            Y = y;
            Width = width;
            Height = height;
        }
        public RectangleF(float width, float height)
        {
            X = 0;
            Y = 0;
            Width = width;
            Height = height;
        }
        public RectangleF(Point2 position, Size2 size)
        {
            X = position.X;
            Y = position.Y;
            Width = size.Width;
            Height = size.Height;
        }
        public static Vector2 GetSize(RectangleF rectangle)
        {
            return new Vector2(rectangle.Width, rectangle.Height);
        }
        public Vector2 GetSize()
        {
            return new Vector2(Width, Height);
        }
        public static void Union(ref RectangleF first, ref RectangleF second, out RectangleF result)
        {
            result.X = Math.Min(first.X, second.X);
            result.Y = Math.Min(first.Y, second.Y);
            result.Width = Math.Max(first.Right, second.Right) - result.X;
            result.Height = Math.Max(first.Bottom, second.Bottom) - result.Y;
        }
        public static RectangleF Union(RectangleF first, RectangleF second)
        {
            RectangleF result;
            Union(ref first, ref second, out result);
            return result;
        }
        public RectangleF Union(RectangleF rectangle)
        {
            RectangleF result;
            Union(ref this, ref rectangle, out result);
            return result;
        }
        public static bool Intersects(ref RectangleF first, ref RectangleF second)
        {
            return first.X < second.X + second.Width && first.X + first.Width > second.X &&
                   first.Y < second.Y + second.Height && first.Y + first.Height > second.Y;
        }
        public static bool Intersects(RectangleF first, RectangleF second)
        {
            return Intersects(ref first, ref second);
        }
        public bool Intersects(RectangleF rectangle)
        {
            return Intersects(ref this, ref rectangle);
        }
        public static bool Contains(ref RectangleF rectangle, ref Point2 point)
        {
            return rectangle.X <= point.X && point.X < rectangle.X + rectangle.Width && rectangle.Y <= point.Y && point.Y < rectangle.Y + rectangle.Height;
        }
        public static bool Contains(RectangleF rectangle, Point2 point)
        {
            return Contains(ref rectangle, ref point);
        }
        public bool Contains(Point2 point)
        {
            return Contains(ref this, ref point);
        }
        public void Inflate(float horizontalAmount, float verticalAmount)
        {
            X -= horizontalAmount;
            Y -= verticalAmount;
            Width += horizontalAmount * 2;
            Height += verticalAmount * 2;
        }
        public void Offset(float offsetX, float offsetY)
        {
            X += offsetX;
            Y += offsetY;
        }
        public void Offset(Vector2 amount)
        {
            X += amount.X;
            Y += amount.Y;
        }
        public static bool operator ==(RectangleF first, RectangleF second)
        {
            return first.Equals(ref second);
        }
        public static bool operator !=(RectangleF first, RectangleF second)
        {
            return !(first == second);
        }
        public bool Equals(RectangleF rectangle)
        {
            return Equals(ref rectangle);
        }
        public bool Equals(ref RectangleF rectangle)
        {
            // ReSharper disable CompareOfFloatsByEqualityOperator
            return X == rectangle.X && Y == rectangle.Y && Width == rectangle.Width && Height == rectangle.Height;
            // ReSharper restore CompareOfFloatsByEqualityOperator
        }
        public override bool Equals(object? obj)
        {
            return obj is RectangleF && Equals((RectangleF)obj);
        }
        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = X.GetHashCode();
                hashCode = (hashCode * 397) ^ Y.GetHashCode();
                hashCode = (hashCode * 397) ^ Width.GetHashCode();
                hashCode = (hashCode * 397) ^ Height.GetHashCode();
                return hashCode;
            }
        }
        public static implicit operator RectangleF(Rectangle rectangle) // Cast Rectangle to RectangleF
        {
            return new RectangleF
            {
                X = rectangle.X,
                Y = rectangle.Y,
                Width = rectangle.Width,
                Height = rectangle.Height
            };
        }
        public static explicit operator Rectangle(RectangleF rectangle) // Cast RectangleF to Rectangle
        {
            return new Rectangle((int)rectangle.X, (int)rectangle.Y, (int)rectangle.Width, (int)rectangle.Height);
        }
        public override string ToString()
        {
            return $"{{X: {X}, Y: {Y}, Width: {Width}, Height: {Height}";
        }

        internal string DebugDisplayString => string.Concat(X, "  ", Y, "  ", Width, "  ", Height);

        public static RectangleF CloneSize(RectangleF rectangleToCopy)
        {
            return new RectangleF(0, 0, rectangleToCopy.Width, rectangleToCopy.Height);
        }
        public RectangleF CloneSize()
        {
            return new RectangleF(0, 0, Width, Height);
        }
        public static RectangleF Clone(RectangleF rect)
        {
            return new RectangleF(rect.X, rect.Y, rect.Width, rect.Height);
        }
        public RectangleF Clone()
        {
            return new RectangleF(X, Y, Width, Height);
        }
        public static RectangleF Extend(RectangleF rect, float step)
        {
            return new RectangleF(rect.X - step, rect.Y - step, rect.Width + step * 2f, rect.Height + step * 2f);
        }
        public RectangleF Extend(float step)
        {
            return Extend(this, step);
        }
        public static RectangleF Add(RectangleF a, RectangleF b)
        {
            return new RectangleF(a.X + b.X, a.Y + b.Y, a.Width + b.Width, a.Height + b.Height);
        }
        public static RectangleF operator +(RectangleF a, RectangleF b)
        {
            return Add(a, b);
        }
        public static RectangleF operator +(RectangleF a, Vector2 offset)
        {
            return new RectangleF(a.X + offset.X, a.Y + offset.Y, a.Width, a.Height);
        }
        public static RectangleF operator -(RectangleF a, Vector2 offset)
        {
            return new RectangleF(a.X - offset.X, a.Y - offset.Y, a.Width, a.Height);
        }
        public static RectangleF Add(RectangleF a, float x, float y, float width, float height)
        {
            return new RectangleF(a.X + x, a.Y + y, a.Width + width, a.Height + height);
        }        
        public static RectangleF Translate(RectangleF a, Vector2 b)
        {
            return new RectangleF(a.X + b.X, a.Y + b.Y, a.Width, a.Height);
        }
        public RectangleF Translate(Vector2 offset)
        {
            return Translate(this, offset);
        }
        public static RectangleF GetRectangleCentered(Vector2 location, Vector2 size)
        {
            return new RectangleF(location.X - size.X / 2, location.Y - size.Y / 2, size.X, size.Y);
        }
        public static RectangleF GetRectangleByPivot(Vector2 location, Vector2 size, Vector2 pivot)
        {
            return new RectangleF(location.X - pivot.X, location.Y - pivot.Y, size.X, size.Y);
        }
    }

    public struct CircleF
    {
        public Point2 Center;
        public float Radius;
        public Point2 Position
        {
            get => Center;
            set => Center = value;
        }
        public float Diameter => 2 * Radius;
        public float Circumference => 2 * MathHelper.Pi * Radius;
        public CircleF(Point2 center, float radius)
        {
            Center = center;
            Radius = radius;
        }

    }

    public class Shape : IClone<Shape>
    {

        public Vector2 _origin = new Vector2();
        public Vector2[] _vertexsModel;
        public Vector2[] _vertexsFinal;
        public float[] _vertexsAngle;

        public bool _isClosed = false;
        public float _angle = 0;
        public float _scale = 1;

        public Shape(float x, float y, Vector2[] vertexs, bool isClosed = false)
        {
            SetPosition(x, y);
            _vertexsModel = vertexs;
            _vertexsFinal = new Vector2[_vertexsModel.Length];
            _vertexsAngle = new float[_vertexsModel.Length];

            _isClosed = isClosed;

            for (int i = 0; i < _vertexsModel.Length; ++i)
            {
                _vertexsAngle[i] = Geo.GetRadian(Vector2.Zero, _vertexsModel[i]);
            }

            Update();
        }
        public Shape(float x, float y, Line line)
        {
            SetPosition(x, y);

            _vertexsModel = new Vector2[2];
            _vertexsFinal = new Vector2[2];

            _vertexsModel[0] = line.A;
            _vertexsModel[1] = line.B;

            _vertexsAngle = new float[2];

            _vertexsAngle[0] = Geo.GetRadian(Vector2.Zero, line.A);
            _vertexsAngle[1] = Geo.GetRadian(Vector2.Zero, line.B);

            Update();
        }
        public Shape(float x, float y, float[] rawPairData, bool isClosed = false)
        {
            Vector2[] vertexs = new Vector2[rawPairData.Length / 2];

            int index = 0;
            // read raw Data index 2 by 2
            for (int i = 0; i < rawPairData.Length; i += 2)
            {
                float rawPairX = rawPairData[i];
                float rawPairY = rawPairData[i + 1];

                vertexs[index] = new Vector2(rawPairX, rawPairY);
                index++;
            }

            SetPosition(x, y);
            _vertexsModel = vertexs;
            _vertexsFinal = new Vector2[_vertexsModel.Length];
            _vertexsAngle = new float[_vertexsModel.Length];

            _isClosed = isClosed;

            for (int i = 0; i < _vertexsModel.Length; ++i)
            {
                _vertexsAngle[i] = Geo.GetRadian(Vector2.Zero, _vertexsModel[i]);
            }

            Update();
        }
        public Shape Clone()
        {
            Shape clone = (Shape)MemberwiseClone();

            clone._vertexsModel = new Vector2[_vertexsModel.Length];
            clone._vertexsFinal = new Vector2[_vertexsModel.Length];
            clone._vertexsAngle = new float[_vertexsModel.Length];

            for (int i = 0; i < _vertexsModel.Length; ++i)
            {
                clone._vertexsFinal[i].X = clone._vertexsModel[i].X = _vertexsModel[i].X;
                clone._vertexsFinal[i].Y = clone._vertexsModel[i].Y = _vertexsModel[i].Y;

                clone._vertexsAngle[i] = _vertexsAngle[i];

                Console.WriteLine("copy shape vertexes : {0} ", _vertexsModel[i]);
            }

            return clone;
        }
        public void SetPosition(float x, float y)
        {
            _origin.X = x;
            _origin.Y = y;
        }
        public void SetPosition(Vector2 position)
        {
            _origin.X = position.X;
            _origin.Y = position.Y;
        }
        public Line GetLine(int index = 0)
        {
            Line line = new Line();

            if (_vertexsFinal.Length > 1)
            {
                line.A = _vertexsFinal[index];
                line.B = _vertexsFinal[index + 1];
            }

            return line;
        }
        public void Transform(bool modelTransform = false) // true if you want change model
        {
            for (int i = 0; i < _vertexsModel.Length; ++i)
            {
                _vertexsFinal[i].X = _origin.X + ((_vertexsModel[i].X) * (float)Math.Cos(_angle) - (_vertexsModel[i].Y) * (float)Math.Sin(_angle)) * _scale;
                _vertexsFinal[i].Y = _origin.Y + ((_vertexsModel[i].X) * (float)Math.Sin(_angle) + (_vertexsModel[i].Y) * (float)Math.Cos(_angle)) * _scale;

                if (modelTransform)
                {
                    _vertexsModel[i].X = _vertexsFinal[i].X;
                    _vertexsModel[i].Y = _vertexsFinal[i].Y;
                }
            }
        }
        public void Update()
        {
            for (int i = 0; i < _vertexsModel.Length; ++i)
            {
                _vertexsFinal[i] = _vertexsModel[i] + _origin;
            }
        }
        public void Render(SpriteBatch batch, Color color, float thickness = 1f, bool isShowCenter = false)
        {
            for (int i = 0; i < _vertexsFinal.Length; ++i)
            {
                if (i < _vertexsFinal.Length - 1)
                    GFX.GFX.Line(batch, _vertexsFinal[i], _vertexsFinal[i + 1], color, thickness);
                else if (_isClosed)
                    GFX.GFX.Line(batch, _vertexsFinal[i], _vertexsFinal[0], color, thickness);
            }

            if (isShowCenter)
                GFX.GFX.Point(batch, _origin, thickness, color);
        }
    }

    public enum Position
    {
        M = -1,
        NW, NE, SW, SE,
        N, S, W, E,
        NM, SM, WM, EM,

        MIDDLE = M, CENTER = M,
        NORTH = N, TOP = N, UP = N,
        SOUTH = S, BOTTOM = S, DOWN = S,
        WEST = W, LEFT = W,
        EAST = E, RIGHT = E,

        NORTH_WEST = NW, TOP_LEFT = NW,
        NORTH_EAST = NE, TOP_RIGHT = NE,
        SOUTH_WEST = SW, BOTTOM_LEFT = SW,
        SOUTH_EAST = SE, BOTTOM_RIGHT = SE,

        MIDDLE_NORTH = NW, TOP_CENTER = NM,
        MIDDLE_SOUTH = SM, BOTTOM_CENTER = SM,
        MIDDLE_WEST = WM, LEFT_CENTER = WM,
        MIDDLE_EAST = EM, RIGHT_CENTER = EM,

        VERTICAL,
        HORIZONTAL,
        DIAGONAL,
    }

    public static class Direction
    {
        public static readonly Point Up = new(0, -1);
        public static readonly Point Down = new(0, 1);
        public static readonly Point Left = new(-1, 0);
        public static readonly Point Right = new(1, 0);

        public static readonly Point UpLeft = new(-1, -1);
        public static readonly Point DownLeft = new(-1, 1);
        public static readonly Point UpRight = new(1, -1);
        public static readonly Point DownRight = new(1, 1);

        public static readonly Point LeftUp = UpLeft;
        public static readonly Point LeftDown = DownLeft;
        public static readonly Point RightUp = UpRight;
        public static readonly Point RightDown = DownRight;
    }

    public static class Geo
    {

        public const float RAD_0 = 0;
        public const float RAD_22_5 = 0.3839724f;
        public const float RAD_45 = 0.785398f;
        public const float RAD_90 = 1.5708f;
        public const float RAD_135 = 2.35619f;
        public const float RAD_180 = 3.14159f;
        public const float RAD_225 = 3.92699f;
        public const float RAD_270 = 4.71239f;
        public const float RAD_315 = 5.49779f;
        public const float RAD_360 = 6.28319f;

        public const float RAD_U = -RAD_90;
        public const float RAD_D = RAD_90;
        public const float RAD_L = RAD_180;
        public const float RAD_R = RAD_0;

        public const float RAD_UL = -RAD_135;
        public const float RAD_UR = -RAD_45;
        public const float RAD_DL = RAD_135;
        public const float RAD_DR = RAD_45;

        public const float RAD_LU = RAD_UL;
        public const float RAD_RU = RAD_UR;
        public const float RAD_LD = RAD_DL;
        public const float RAD_RD = RAD_DR;

        public static double DegToRad(double angle)
        {
            return Math.PI * angle / 180.0;
        }
        public static double RadToDeg(double angle)
        {
            return angle * (180.0 / Math.PI);
        }

        public static float GetDistancePow2(Vector2 start, Vector2 goal)
        {
            return (start.X - goal.X) * (start.X - goal.X) + (start.Y - goal.Y) * (start.Y - goal.Y);
        }
        public static double GetDistance(Vector2 start, Vector2 goal)
        {
            return Math.Sqrt((start.X - goal.X) * (start.X - goal.X) + (start.Y - goal.Y) * (start.Y - goal.Y));
        }

        public static bool IsNear(Vector2 start, Vector2 goal, float distance)
        {
            return (start.X - goal.X) * (start.X - goal.X) + (start.Y - goal.Y) * (start.Y - goal.Y) <= distance * distance;
        }

        public static Vector2 GetVector(Vector2 start, Vector2 goal, float speed = 1f)
        {
            float vecX = goal.X - start.X;
            float vecY = goal.Y - start.Y;

            Vector2 moveVector = new Vector2(0, 0);

            moveVector.X = speed * (float)Math.Cos(-Math.Atan2(vecX, vecY) + RAD_90);
            moveVector.Y = speed * (float)Math.Sin(-Math.Atan2(vecX, vecY) + RAD_90);

            return moveVector;
        }

        public static Vector2 GetVector(float angle)
        {
            return new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle));
        }

        public static float GetRadian(Vector2 start, Vector2 goal)
        {
            double x = goal.X - start.X;
            double y = goal.Y - start.Y;

            return (float)Math.Atan2(y, x);
        }
        public static Vector2 GetCenter(Vector2 pointA, Vector2 pointB)
        {
            float vx = pointB.X - pointA.X;
            float vy = pointB.Y - pointA.Y;

            float x = pointA.X + (vx / 2);
            float y = pointA.Y + (vy / 2);

            Vector2 center = new Vector2(x, y);

            return center;
        }


    }

    public static class Gfx
    {

        [Serializable]
        public class Point
        {
            public Vector2 _vec = new Vector2();
            [XmlIgnore] public bool _isSelected = false;
            [XmlIgnore] public bool _isOver = false;
            [XmlIgnore] public bool _isDrag = false;

            [XmlIgnore] public float _dragX = 0, _dragY = 0;

            public Point Clone()
            {
                return (Point)MemberwiseClone();
            }

            public Point() { } // Need For serialization
            public Point(float x, float y)
            {
                _vec = new Vector2(x, y);
            }
            public void SetVec(float x, float y)
            {
                _vec.X = x;
                _vec.Y = y;
            }
            public static void Swap(ref Point pointA, ref Point pointB)
            {
                Point tmp = pointA;
                pointA = pointB;
                pointB = pointA;
            }
        }

        [Serializable]
        public class Line
        {
            public Point _pointA = new Point();
            public Point _pointB = new Point();
            [XmlIgnore] public bool _isSelected = false;
            [XmlIgnore] public bool _isOver = false;
            [XmlIgnore] public bool _isDrag = false;

            [XmlIgnore] public float _dragAX = 0, _dragAY = 0;
            [XmlIgnore] public float _dragBX = 0, _dragBY = 0;

            public Line Clone()
            {
                return (Line)MemberwiseClone();
            }
            public Line() { } // Need For serialization
            public Line(Point pointA, Point pointB)
            {
                _pointA = pointA;
                _pointB = pointB;
            }
            public Vector2 Center()
            {
                float vx = _pointB._vec.X - _pointA._vec.X;
                float vy = _pointB._vec.Y - _pointA._vec.Y;

                float x = _pointA._vec.X + (vx / 2);
                float y = _pointA._vec.Y + (vy / 2);

                Vector2 center = new Vector2(x, y);

                return center;
            }
            public Vector2 Percent(float percent)
            {
                float vx = _pointB._vec.X - _pointA._vec.X;
                float vy = _pointB._vec.Y - _pointA._vec.Y;

                float x = _pointA._vec.X + (vx * (percent / 100));
                float y = _pointA._vec.Y + (vy * (percent / 100));

                Vector2 center = new Vector2(x, y);

                return center;
            }
        }

        [Serializable]
        public class Path
        {
            [Flags]
            public enum Flag
            {
                None = 0,
                Name = 1,
                Spline = 2,
                Border = 4,
                Interval = 8,
                ControlPoint = 16,
                ControlLine = 32,
                IndexPoint = 64,
                IndexLine = 128,
                IntervalPoint = 256,
                Editable = 512
            }

            #region Attributes

            public string _name = "";

            float selectedT = 0;
            //Input.Mouse _mouse;
            public Flag _flag;

            public bool IsFocus = false; // Path is focused

            public float PathSize = 32;
            public float PathDivSize = .2f;

            public List<Point> _points = new List<Point>(); // Must be public for serialization
            List<Line> _lines = new List<Line>(); // Must be public for serialization

            Point? _pointOver = null;
            Line? _lineOver = null;

            Vector2 _mousePosOverLine;
            Vector2 _mousePosOverPoint;

            bool _isMoveAll = false;

            List<Line> _pathLines = new List<Line>();

            // Render
            string _strOnLine = "";
            string _strOnPoint = "";

            #endregion

            public Path() { } // Need For serialization
            public Path(string name, Flag flag)
            {
                _name = name;
                _flag = flag;
            }


            public int ReconnectPointLine() // Return number of point reconnected
            {
                int numPoint = 0;
                _lines.Clear();

                Point prevPoint = _points[0];
                for (int i = 1; i < _points.Count; i++)
                {
                    Line line = new Line(prevPoint, _points[i]);
                    _lines.Add(line);
                    prevPoint = _points[i];

                    numPoint = i;
                }
                return numPoint;
            }
            // Spline Path + refresh _pathLines 
            public List<Line> SetPathLine(float pathWidth, float precision = 0.4f)
            {
                List<Line> lines = new List<Line>();

                //lines.Add(GetSplineVector(-1, pathWidth)); // First Line

                for (float t = 0; t <= _points.Count - 1f; t += precision)
                {
                    //Vector2 pos = GetSplinePoint(t);
                    Line line = GetSplineVector(t, pathWidth);

                    lines.Add(line);
                }

                lines.Add(GetSplineVector(_points.Count - 1f, pathWidth)); // Last Line

                _pathLines = lines;

                return lines;
            }
            public List<Line> GetPathLine()
            {
                return _pathLines;
            }

            // Spline
            public Vector2 GetVecPointAt(int index)
            {
                if (index <= 0)
                    return _points[0]._vec;

                if (index > _points.Count - 1)
                    return _points[_points.Count - 1]._vec;

                return _points[index]._vec;
            }
            public Vector2 GetSplinePoint(float t, bool bLooped = false)
            {
                if (_points.Count > 1)
                {
                    int p0, p1, p2, p3;
                    if (!bLooped)
                    {
                        p1 = (int)t;
                        p2 = p1 + 1;
                        p3 = p2 + 1;
                        p0 = p1 - 1;
                    }
                    else
                    {
                        p1 = (int)t;
                        p2 = (p1 + 1) % _points.Count;
                        p3 = (p2 + 1) % _points.Count;
                        p0 = p1 >= 1 ? p1 - 1 : _points.Count - 1;
                    }

                    t = t - (int)t;

                    float tt = t * t;
                    float ttt = tt * t;

                    float q1 = -ttt + 2.0f * tt - t;
                    float q2 = 3.0f * ttt - 5.0f * tt + 2.0f;
                    float q3 = -3.0f * ttt + 4.0f * tt + t;
                    float q4 = ttt - tt;

                    Vector2 point0 = GetVecPointAt(p0);
                    Vector2 point1 = GetVecPointAt(p1);
                    Vector2 point2 = GetVecPointAt(p2);
                    Vector2 point3 = GetVecPointAt(p3);

                    float tx = 0.5f * (point0.X * q1 + point1.X * q2 + point2.X * q3 + point3.X * q4);
                    float ty = 0.5f * (point0.Y * q1 + point1.Y * q2 + point2.Y * q3 + point3.Y * q4);

                    return new Vector2(tx, ty);
                }
                return new Vector2();
            }
            public Vector2 GetSplineGradient(float t, bool bLooped = false)
            {
                if (_points.Count > 1)
                {
                    int p0, p1, p2, p3;
                    if (!bLooped)
                    {
                        p1 = (int)t;
                        p2 = p1 + 1;
                        p3 = p2 + 1;
                        p0 = p1 - 1;
                    }
                    else
                    {
                        p1 = (int)t;
                        p2 = (p1 + 1) % _points.Count;
                        p3 = (p2 + 1) % _points.Count;
                        p0 = p1 >= 1 ? p1 - 1 : _points.Count - 1;
                    }

                    t = t - (int)t;

                    float tt = t * t;
                    float ttt = tt * t;

                    float q1 = -3.0f * tt + 4.0f * t - 1;
                    float q2 = 9.0f * tt - 10.0f * t;
                    float q3 = -9.0f * tt + 8.0f * t + 1.0f;
                    float q4 = 3.0f * tt - 2.0f * t;

                    Vector2 point0 = GetVecPointAt(p0);
                    Vector2 point1 = GetVecPointAt(p1);
                    Vector2 point2 = GetVecPointAt(p2);
                    Vector2 point3 = GetVecPointAt(p3);

                    float tx = 0.5f * (point0.X * q1 + point1.X * q2 + point2.X * q3 + point3.X * q4);
                    float ty = 0.5f * (point0.Y * q1 + point1.Y * q2 + point2.Y * q3 + point3.Y * q4);

                    return new Vector2(tx, ty);
                }
                return new Vector2();
            }

            public Line DrawSplineVector(SpriteBatch batch, float t, float size, Color color, Vector2 camera, float tickness = 1, bool perpendicular = false)
            {
                Vector2 p1 = GetSplinePoint(t);
                Vector2 g1 = GetSplineGradient(t);

                float r = (float)Math.Atan2(-g1.Y, g1.X);
                float r2 = r + (float)Math.PI / 2;

                float selectorSize = size;

                GFX.GFX.Line(batch,
                    selectorSize * (float)Math.Sin(r) + p1.X + camera.X, selectorSize * (float)Math.Cos(r) + p1.Y + camera.Y,
                    -selectorSize * (float)Math.Sin(r) + p1.X + camera.X, -selectorSize * (float)Math.Cos(r) + p1.Y + camera.Y,
                    color, tickness);

                if (perpendicular)
                    GFX.GFX.Line(batch,
                        selectorSize * (float)Math.Sin(r2) + p1.X + camera.X, selectorSize * (float)Math.Cos(r2) + p1.Y + camera.Y,
                        -selectorSize * (float)Math.Sin(r2) + p1.X + camera.X, -selectorSize * (float)Math.Cos(r2) + p1.Y + camera.Y,
                        color, tickness);

                Point pointA = new Point(0, 0);
                Point pointB = new Point(0, 0);

                pointA._vec.X = selectorSize * (float)Math.Sin(r) + p1.X;
                pointA._vec.Y = selectorSize * (float)Math.Cos(r) + p1.Y;

                pointB._vec.X = -selectorSize * (float)Math.Sin(r) + p1.X;
                pointB._vec.Y = -selectorSize * (float)Math.Cos(r) + p1.Y;

                Line line = new Line(pointA, pointB);

                return line;
            }
            public Line GetSplineVector(float t, float selectorSize)
            {

                Vector2 p1 = GetSplinePoint(t);
                Vector2 g1 = GetSplineGradient(t);

                float r = (float)Math.Atan2(-g1.Y, g1.X);

                Point pointA = new Point(0, 0);
                Point pointB = new Point(0, 0);

                pointA._vec.X = selectorSize * (float)Math.Sin(r) + p1.X;
                pointA._vec.Y = selectorSize * (float)Math.Cos(r) + p1.Y;

                pointB._vec.X = -selectorSize * (float)Math.Sin(r) + p1.X;
                pointB._vec.Y = -selectorSize * (float)Math.Cos(r) + p1.Y;

                Line line = new Line(pointA, pointB);

                return line;
            }
            public bool IsOverOnePoint() { return (null != _pointOver); }
            public bool IsOverOneLine() { return (null != _lineOver); }
            public int GetIndexOverOnePoint()
            {
                return _points.FindIndex(point => point._isOver == true);
            }
            public int GetIndexOverOneLine()
            {
                return _lines.FindIndex(line => line._isOver == true);
            }
            public void SetStrOnLine(string strOnLine)
            {
                _strOnLine = strOnLine;
            }
            public void SetStrOnPoint(string strOnPoint)
            {
                _strOnPoint = strOnPoint;
            }
            public Point? GetPointAt(int index)
            {
                if (index >= 0 && index < _points.Count)
                {
                    return _points[index];
                }
                return null;
            }
            public Line? GetLineAt(int index)
            {
                if (index >= 0 && index < _lines.Count)
                {
                    return _lines[index];
                }
                return null;
            }
            public void SetMoveAll(bool isMoveAll)
            {
                _isMoveAll = isMoveAll;
            }
            public void Add(float x, float y)
            {
                if (_points.Count > 0)
                {
                    if (IsOverOneLine() && !IsOverOnePoint()) // If over one line then Add one point and two lines !
                    {
                        int indexLine = GetIndexOverOneLine();

                        if (indexLine >= 0)
                        {
                            Point pointA = _lines[indexLine]._pointA;
                            Point pointB = _lines[indexLine]._pointB;

                            int indexPointA = _points.FindIndex(pA => ReferenceEquals(pA, pointA));
                            int indexPointB = _points.FindIndex(pb => ReferenceEquals(pb, pointB));

                            Point point = new Point(x, y);

                            _points.Insert(indexPointB, point);

                            Line lineA = new Line(pointA, point);
                            Line lineB = new Line(point, pointB);

                            _lines.Insert(indexLine + 1, lineA);
                            _lines.Insert(indexLine + 2, lineB);

                            _lines.Remove(_lines[indexLine]);
                        }
                    }
                    else
                    {
                        if (!IsOverOnePoint()) // If not over one point then Add one point !
                        {

                            Point point = new Point(x, y);
                            _points.Add(point);

                            int index = _points.Count;

                            Line? line = null;

                            if (index >= 2)
                            {
                                Point prevPoint = _points[index - 2];
                                line = new Line(prevPoint, point);
                                _lines.Add(line);
                            }

                        }
                    }
                }
                else // if path has not Point add the first point !
                {
                    Point point = new Point(x, y);
                    _points.Add(point);
                }


            }
            public void Del(float x, float y, int solution = 0)
            {
                if (_points.Count > 0)
                {
                    if (IsOverOnePoint()) // if over one point !
                    {
                        Console.WriteLine("Del over One Point !");

                        int indexPoint = GetIndexOverOnePoint();

                        if (indexPoint >= 0)
                        {
                            if (indexPoint == 0) // if delete First point !
                            {
                                _points.RemoveAt(0);

                                if (_lines.Count > 0)
                                    _lines.RemoveAt(0);
                                return;
                            }

                            if (indexPoint > 0 && indexPoint < _points.Count - 1) // if delete inbetween point !
                            {
                                // lines       :      1      2 
                                // points      :  A-------B-------C
                                // indexPoint  : -1       0      +1    

                                // Get the three points
                                //Point pointA = _points[indexPoint - 1];
                                Point? pointB = _pointOver;
                                Point pointC = _points[indexPoint + 1];
                                // Get the two lines
                                Line line1 = _lines[indexPoint - 1];
                                Line line2 = _lines[indexPoint];

                                line1._pointB = pointC; // connect line to point B
                                _points.Remove(pointB!); // remove point C
                                _lines.Remove(line2);   // remove line 2 

                                Console.WriteLine("Remove Point Inbetween points : " + indexPoint);

                                return;
                            }

                            if (indexPoint == _points.Count - 1 && _lines.Count > 0) // if delete Last point !
                            {
                                _points.RemoveAt(indexPoint);
                                _lines.RemoveAt(indexPoint - 1);
                                return;
                            }

                        }
                        return;
                    }
                    else
                    if (IsOverOneLine()) // if over one line !
                    {
                        Console.WriteLine("Del over One Line !");

                        int indexLine = GetIndexOverOneLine();

                        if (indexLine >= 0)
                        {
                            if (indexLine == 0) // if Delete First line !
                            {
                                _lines.RemoveAt(0);
                                _points.RemoveAt(0);
                                return;
                            }

                            //Solution 1
                            if (indexLine > 0 && indexLine < _points.Count - 2 && solution == 0) // id delete inbetween line !
                            {
                                // lines       :      1       2       3
                                // points      :  A-------B-------C-------D
                                // indexLine   : -1       0      +1      +2 

                                // Get the three points
                                //Point pointA = _points[indexPoint - 1];
                                Point pointA = _points[indexLine - 1];
                                Point pointB = _points[indexLine + 0];
                                Point pointC = _points[indexLine + 1];
                                Point pointD = _points[indexLine + 2];
                                // Get the two lines
                                Line line1 = _lines[indexLine - 1];
                                Line line2 = _lines[indexLine];
                                Line line3 = _lines[indexLine + 1];

                                line1._pointB = pointD;

                                _points.Remove(pointB);
                                _points.Remove(pointC);

                                _lines.Remove(line2);
                                _lines.Remove(line3);


                                return;
                            }

                            // Solution 2
                            if (indexLine > 0 && indexLine < _points.Count - 2 && solution == 1) // id delete inbetween line !
                            {
                                // lines       :      1       2       3
                                // points      :  A-------B-------C-------D
                                // indexLine   : -1       0      +1      +2 

                                // Get the three points
                                //Point pointA = _points[indexPoint - 1];
                                Point pointA = _points[indexLine - 1];
                                Point pointB = _points[indexLine + 0];
                                Point pointC = _points[indexLine + 1];
                                Point pointD = _points[indexLine + 2];
                                // Get the two lines
                                Line line1 = _lines[indexLine - 1];
                                Line line2 = _lines[indexLine];
                                Line line3 = _lines[indexLine + 1];

                                pointC.SetVec(x, y);

                                line1._pointB = pointC;

                                _points.Remove(pointB);

                                _lines.Remove(line2);


                                return;
                            }


                            if (indexLine == _lines.Count - 1 && _lines.Count > 0) // if delete Last line !
                            {
                                _points.RemoveAt(indexLine + 1);
                                _lines.RemoveAt(indexLine);
                                return;
                            }


                        }

                    }



                }
            }
            public void Update(Input.MouseControl mouse)
            {

                //_mouse = mouse;

                // Avoid isMoveAll if no points or lines over !
                if (!IsOverOneLine() && !IsOverOnePoint())
                {
                    _isMoveAll = false;

                    if (mouse._onClick && !Keyboard.GetState().IsKeyDown(Keys.LeftShift))
                        IsFocus = false;
                }

                if (_points.Count > 0 && _flag.HasFlag(Flag.Editable))
                {
                    _pointOver = null;

                    for (int i = 0; i < _points.Count; ++i)
                    {
                        Point point = _points[i];
                        point._isOver = false;
                        point._isSelected = false;

                        if (Misc.CircleInCircle(mouse.AbsX, mouse.AbsY, 1, point._vec.X, point._vec.Y, 8) && !mouse._isActiveDrag && !mouse._isOverAny) // mouse over point
                        {
                            mouse._isOverAny = true;
                            point._isOver = true;
                            _pointOver = point;
                            _mousePosOverPoint = new Vector2(mouse._x, mouse._y);

                            if (mouse._onClick)
                                IsFocus = true;

                            // Remove line Over, Point Over is priority !
                            if (_lineOver != null)
                            {
                                _lineOver._isOver = false;
                                _lineOver = null;
                            }

                            break; // immediate exit loop after one over !
                        }

                    }

                    for (int i = 0; i < _points.Count; ++i)
                    {
                        Point point = _points[i];

                        //if (Misc.InCircle(mouse._x, mouse._y, 1,point._vec.X, point._vec.Y,4) && !mouse._drag) // mouse over point
                        if (point._isOver)
                        {
                            if (mouse._onClick)
                            {
                                mouse._isActiveDrag = true;
                                point._isDrag = true;

                            }
                        }



                        if (mouse._isMove && point._isDrag)
                        {
                            point._vec.X = mouse._x - point._dragX;
                            point._vec.Y = mouse._y - point._dragY;
                        }

                        if (point._isDrag)
                        {
                            point._isSelected = true;

                            point._dragX = mouse._x - point._vec.X;
                            point._dragY = mouse._y - point._vec.Y;
                        }
                    }



                    // If move all Points
                    if (_isMoveAll)
                    {
                        for (int i = 0; i < _points.Count; ++i)
                        {
                            Point point = _points[i];

                            if (IsOverOnePoint())
                            {
                                if (mouse._onClick)
                                {
                                    point._dragX = mouse._x - point._vec.X;
                                    point._dragY = mouse._y - point._vec.Y;
                                    point._isDrag = true;
                                }

                                if (point._isDrag)
                                {
                                    point._isSelected = true;

                                    point._dragX = mouse._x - point._vec.X;
                                    point._dragY = mouse._y - point._vec.Y;
                                }
                            }
                        }
                    }

                    for (int i = 0; i < _points.Count; ++i)
                    {
                        Point point = _points[i];
                        if (mouse._up)
                        {
                            point._isDrag = false;
                        }
                    }

                    // If mouse cursor over one point then don't continue detect others
                    if (null == _pointOver)
                    {

                        _lineOver = null;

                        for (int i = 0; i < _lines.Count; ++i)
                        {
                            Line line = _lines[i];
                            line._isOver = false;
                            line._isSelected = false;

                            if (Collision2D.SegmentCircleF(line._pointA._vec, line._pointB._vec, new CircleF(new Point2(mouse.AbsX, mouse.AbsY), 4)) && !mouse._isActiveDrag && !mouse._isOverAny && !IsOverOnePoint())
                            {
                                mouse._isOverAny = true;
                                line._isOver = true;
                                _lineOver = line;
                                _mousePosOverLine = new Vector2(mouse._x, mouse._y);

                                if (mouse._onClick)
                                    IsFocus = true;
                                break; // immediate exit loop after one over !
                            }
                        }

                        for (int i = 0; i < _lines.Count; ++i)
                        {
                            Line line = _lines[i];


                            //if (Collision2D.SegmentCircleF(line._pointA._vec, line._pointB._vec, new CircleF(new Point2(mouse._x, mouse._y), 4)) && !mouse._drag)
                            if (line._isOver)
                            {
                                if (mouse._onClick)
                                {
                                    mouse._isActiveDrag = true;
                                    line._isDrag = true;
                                }
                            }


                            if (mouse._isMove && line._isDrag)
                            {
                                line._pointA._vec.X = mouse._x - line._dragAX;
                                line._pointA._vec.Y = mouse._y - line._dragAY;
                                line._pointB._vec.X = mouse._x - line._dragBX;
                                line._pointB._vec.Y = mouse._y - line._dragBY;
                            }

                            if (line._isDrag)
                            {
                                line._isSelected = true;

                                line._dragAX = mouse._x - line._pointA._vec.X;
                                line._dragAY = mouse._y - line._pointA._vec.Y;
                                line._dragBX = mouse._x - line._pointB._vec.X;
                                line._dragBY = mouse._y - line._pointB._vec.Y;
                            }
                        }

                        // If move all lines
                        if (_isMoveAll)
                        {
                            for (int i = 0; i < _lines.Count; ++i)
                            {
                                Line line = _lines[i];

                                if (IsOverOneLine())
                                {
                                    if (mouse._onClick)
                                    {
                                        line._dragAX = mouse._x - line._pointA._vec.X;
                                        line._dragAY = mouse._y - line._pointA._vec.Y;
                                        line._dragBX = mouse._x - line._pointB._vec.X;
                                        line._dragBY = mouse._y - line._pointB._vec.Y;
                                        line._isDrag = true;
                                    }

                                    if (line._isDrag)
                                    {
                                        line._isSelected = true;

                                        line._dragAX = mouse._x - line._pointA._vec.X;
                                        line._dragAY = mouse._y - line._pointA._vec.Y;
                                        line._dragBX = mouse._x - line._pointB._vec.X;
                                        line._dragBY = mouse._y - line._pointB._vec.Y;
                                    }
                                }
                            }
                        }

                        for (int i = 0; i < _lines.Count; ++i)
                        {
                            Line line = _lines[i];
                            if (mouse._up)
                            {
                                line._isDrag = false;
                            }
                        }
                    }


                }

                #region Edit Path
                //if (IsFocus)
                if (_flag.HasFlag(Flag.Editable) && IsFocus)
                {
                    //_path.Update(Input._mouse);
                    SetStrOnPoint("");
                    SetStrOnLine("");

                    if (IsOverOnePoint())
                        SetStrOnPoint("Point " + GetIndexOverOnePoint());
                    else if (IsOverOneLine())
                        SetStrOnLine("Line " + GetIndexOverOneLine());

                    if (Keyboard.GetState().IsKeyDown(Keys.LeftShift))
                    {
                        SetStrOnLine("[+] Point");
                        SetStrOnPoint("");

                        if (mouse._onClick)
                        {
                            Add(mouse.AbsX, mouse.AbsY);
                        }
                    }

                    if (Keyboard.GetState().IsKeyDown(Keys.LeftAlt))
                    {
                        SetStrOnLine("[-] Line");
                        SetStrOnPoint("[-] Point");

                        //Console.WriteLine("Begin Delete SomeThing !");

                        if (mouse._onClick)
                        {
                            Console.WriteLine("Delete SomeThing !");

                            if (Keyboard.GetState().IsKeyDown(Keys.X))
                                Del(mouse.AbsX, mouse.AbsY, 0);
                            else
                                Del(mouse.AbsX, mouse.AbsY, 1);
                        }
                    }

                    SetMoveAll((Keyboard.GetState().IsKeyDown(Keys.LeftControl)));
                }
                #endregion

            }
            public void Render(SpriteBatch batch, SpriteFont font, Input.MouseControl mouse, Vector2 camera, float offsetY = 16)
            {
                if (_points.Count > 0)
                {
                    float alpha = .2f;
                    if (IsFocus) alpha = .8f;

                    if (_flag.HasFlag(Flag.Name))
                    {
                        string info = _name; // + " >> Over Point : " + GetIndexOverOnePoint() + " | Line : " + GetIndexOverOneLine() ;
                        batch.DrawString(font, info, _points[0]._vec + new Vector2(8, -8) + camera, Color.Ivory * alpha);
                    }

                    bool onePointOver = false;

                    Line prevLine = GetSplineVector(0, PathSize);
                    Vector2 prevPointA = prevLine._pointA._vec;
                    Vector2 prevPointB = prevLine._pointB._vec;

                    if (_flag.HasFlag(Flag.Interval))
                        DrawSplineVector(batch, 0, PathSize, Color.GreenYellow * alpha, camera);

                    Vector2 prevPos = GetSplinePoint(0);

                    // Draw Spline
                    for (float t = 0; t <= _points.Count - 1f; t += PathDivSize)
                    {
                        Vector2 pos = GetSplinePoint(t);
                        //batch.DrawPoint(pos, Color.Fuchsia * .8f, 2);

                        Line intervalLine = GetSplineVector(t, PathSize);

                        if (_flag.HasFlag(Flag.Interval))
                            DrawSplineVector(batch, t, PathSize, Color.OrangeRed * alpha, camera);

                        //Line line = GetSplineVector(t, 32);

                        if (_flag.HasFlag(Flag.Border))
                        {
                            GFX.GFX.Line(batch, prevPointA + camera, intervalLine._pointA._vec + camera, Color.GhostWhite * alpha, 2);
                            GFX.GFX.Line(batch, prevPointB + camera, intervalLine._pointB._vec + camera, Color.GhostWhite * alpha, 2);
                        }

                        //batch.DrawPoint(line._pointA._vec, Color.Red * .8f, 2);
                        //batch.DrawPoint(line._pointB._vec, Color.Green * .8f, 2);

                        prevPointA = intervalLine._pointA._vec;
                        prevPointB = intervalLine._pointB._vec;

                        if (Misc.CircleInCircle(mouse.AbsX, mouse.AbsY, 0, pos.X, pos.Y, 4) && !onePointOver && _flag.HasFlag(Flag.IntervalPoint))
                        {

                            if (mouse._onClick) selectedT = t;

                            onePointOver = true;
                            GFX.GFX.Line(batch, pos + camera, 4, 8, Color.Gold);
                            batch.DrawString(font, "t=" + t, pos + new Vector2(8, -8) + camera, Color.Ivory);
                        }

                        if (_flag.HasFlag(Flag.Spline))
                            GFX.GFX.Line(batch, prevPos + camera, pos + camera, Color.Cyan * alpha, 2);

                        prevPos = pos;

                        //batch.DrawPoint(line.Percent(15), Color.MonoGameOrange, 4); // point to go : unit path
                    }

                    //Line lastLine = GetSplineVector(_points.Count - 2f, 32);
                    //Vector2 lastPointA = lastLine._pointA._vec;
                    //Vector2 lastPointB = lastLine._pointB._vec;
                    //DrawSplineVector(batch, _points.Count - 2f, PathSize, Color.GreenYellow * .8f);

                    Point prevPoint;
                    int i = 0;

                    prevPoint = _points[0];
                    GFX.GFX.Circle(batch, prevPoint._vec + camera, 4, 8, Color.Gold * alpha);

                    if (_flag.HasFlag(Flag.ControlLine))
                    {

                        foreach (Line line in _lines)
                        {
                            GFX.GFX.Line(batch, line._pointA._vec + camera, line._pointB._vec + camera, Color.Red * alpha, 1);

                            if (line._isOver)
                            {
                                GFX.GFX.Line(batch, line._pointA._vec + camera, line._pointB._vec + camera, Color.OrangeRed, 2);
                                batch.DrawString(font, _strOnLine, _mousePosOverLine + new Vector2(8, -8), Color.Ivory);
                            }

                            if (line._isSelected)
                                GFX.GFX.Line(batch, line._pointA._vec + camera, line._pointB._vec + camera, Color.Orange, 2);

                            if (_flag.HasFlag(Flag.IndexLine))
                            {
                                Vector2 labelIndexPos = Geo.GetCenter(line._pointA._vec, line._pointB._vec);

                                batch.DrawString(font, i.ToString(), labelIndexPos + new Vector2(0, -offsetY) + camera, Color.Orange * alpha);
                            }

                            //batch.DrawCircle(line.Center(), 3, 4, Color.GreenYellow, 1);
                            //batch.DrawString(font, i.ToString(), line.Center() + new Vector2(0, -16), Color.Honeydew);

                            ++i;
                        }
                    }


                    if (_flag.HasFlag(Flag.ControlPoint))
                    {
                        i = 0;

                        foreach (Point point in _points)
                        {
                            //batch.DrawLine(prevPoint._vec, point._vec, Color.Red, 1);
                            GFX.GFX.Circle(batch, point._vec + camera, 4, 8, Color.Gold * alpha, 4);

                            prevPoint = point;

                            if (point._isOver)
                            {
                                GFX.GFX.Circle(batch, point._vec + camera, 4, 8, Color.Gold, 4);
                                GFX.GFX.Circle(batch, point._vec + camera, 8, 8, Color.OrangeRed, 1);
                                batch.DrawString(font, _strOnPoint, _mousePosOverPoint + new Vector2(8, -8), Color.Ivory);
                            }

                            if (point._isSelected)
                                GFX.GFX.Circle(batch, point._vec + camera, 8, 8, Color.Orange, 1);

                            if (_flag.HasFlag(Flag.IndexPoint))
                                batch.DrawString(font, i.ToString(), point._vec + new Vector2(0, -offsetY) + camera, Color.Yellow * alpha);

                            ++i;
                        }

                    }

                    //DrawSplineVector(batch, selectedT, 16, Color.LightSkyBlue, 2, true);
                    //selectedT += 0.01f;

                    //if (selectedT > _points.Count - 2)
                    //    selectedT = 0;

                }
            }

        }
    }
}
