using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System.Reflection;
using Newtonsoft.Json.Linq;
using System.Xml;
using Mugen.Physics;

namespace Mugen.Core
{
    public class Token
    {
        public static bool Exist(JObject jObject, string token)
        {
            return jObject.SelectToken(token) != null;
        }
        public static T? Get<T>(JObject jObject, string token)
        {
            var keyToken = jObject.SelectToken(token);

            if (keyToken != null)
                return jObject.SelectToken(token)!.Value<T>();

            return default;
        }
    }
    public class Field
    {
        /// <summary>
        /// Get Field by string name !
        /// </summary>
        /// <typeparam name="T">Class : Type</typeparam>
        /// <typeparam name="V">Value : Field</typeparam>
        /// <param name="name"> Name of the Field</param>
        /// <returns></returns>
        public static V? Get<T, V>(string name)
        {
            try
            {
                FieldInfo field = typeof(T).GetField(name)!;
                if (field != null)
                    return (V)field.GetValue(null)!;

            }
            catch (ArgumentNullException)
            {

            }

            return default;
        }
    }


    public static class Const
    {
        public const int NoIndex = -1;
    }

    public static class StaticType<T>
    {
        public static int _type = 0;
    }
    public static class UID
    {
        static int _uniqueType = 0;

        public static int Get<T>()
        {
            if (StaticType<T>._type != 0)
            {
                return StaticType<T>._type;
            }
            else
            {
                ++_uniqueType;
                StaticType<T>._type = _uniqueType;
            }

            return StaticType<T>._type;
        }

    }


    public static class XML
    {
        public static string? GetAttributeValue(XmlNode xmlNode, string attribute)
        {
            return null != xmlNode.Attributes![attribute] ? xmlNode.Attributes[attribute]!.Value : "auto";
        }
        public static string SetAttributeValue(XmlNode xmlNode, string attribute, string value)
        {
            if (null != xmlNode.Attributes![attribute])
                xmlNode.Attributes[attribute]!.Value = value;

            return value;
        }
    }

    public static class FrameCounter
    {
        static float _deltaTime;

        static public long TotalFrames { get; private set; }
        static public float TotalSeconds { get; private set; }
        static public float AverageFramesPerSecond { get; private set; }
        static public float CurrentFramesPerSecond { get; private set; }

        public const int MAXIMUM_SAMPLES = 100;

        static private Queue<float> _sampleBuffer = new Queue<float>();

        static public string Fps()
        {
            return string.Format("{0}", Math.Round(AverageFramesPerSecond));
        }

        static public bool Update(GameTime gameTime)
        {
            _deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

            CurrentFramesPerSecond = 1.0f / _deltaTime;

            _sampleBuffer.Enqueue(CurrentFramesPerSecond);

            if (_sampleBuffer.Count > MAXIMUM_SAMPLES)
            {
                _sampleBuffer.Dequeue();
                AverageFramesPerSecond = _sampleBuffer.Average(i => i);
            }
            else
            {
                AverageFramesPerSecond = CurrentFramesPerSecond;
            }

            TotalFrames++;
            TotalSeconds += _deltaTime;
            return true;
        }

        static public void Draw(SpriteBatch batch, SpriteFont font, Color color, float x = 1, float y = 1)
        {
            batch.DrawString(font, Fps(), new Vector2(x, y), color);
        }

    }

    public static class LimitFPS
    {
        static bool _limitFPS = false;

        public static void Toogle(Game game, GraphicsDeviceManager graphicsDeviceManager)
        {
            _limitFPS = !_limitFPS;

            game.IsFixedTimeStep = _limitFPS;
            graphicsDeviceManager.SynchronizeWithVerticalRetrace = _limitFPS;
            graphicsDeviceManager.ApplyChanges();

            Console.WriteLine("LimitFPS = " + _limitFPS);
        }
    }

    public class Misc
    {
        public static string? ParentFolderName(string fullPathName) // Return the direct Parent only of a fullPath with fileName !
        {
            return Path.GetFileName(Path.GetDirectoryName(fullPathName));
        }

        public static Random Rng = new Random();

        static public int Log(string message, int error = 0)
        {
#if DEBUG_ON
                System.Console.Write(message);
#endif
            return error;
        }

        // Point in Range, Rect, Circle
        public static bool ValueInRange(int value, int mini, int maxi)
        {
            return value > mini && value < maxi;
        }
        public static bool ValueInRange(float value, float mini, float maxi)
        {
            return value > mini && value < maxi;
        }
        public static bool PointInRect(int x, int y, Rectangle rect)
        {
            return x > rect.X && x < rect.X + rect.Width &&
                    y > rect.Y && y < rect.Y + rect.Height;
        }
        public static bool PointInRect(float x, float y, RectangleF rect)
        {
            return x > rect.X && x < rect.X + rect.Width &&
                    y > rect.Y && y < rect.Y + rect.Height;
        }
        public static bool PointInRect(Point point, Rectangle rect)
        {
            return PointInRect(point.X, point.Y, rect);
        }
        public static bool PointInRect(Vector2 point, RectangleF rect)
        {
            return PointInRect(point.X, point.Y, rect);
        }
        public static bool CircleInCircle(int x, int y, int r, int x1, int y1, int r1)
        {
            int dx = x - x1;
            int dy = y - y1;
            int distance = (int)Math.Sqrt(dx * dx + dy * dy);

            if (distance < r + r1)
                return true;
            else
                return false;
        }
        public static bool CircleInCircle(float x, float y, float r, float x1, float y1, float r1)
        {
            float dx = x - x1;
            float dy = y - y1;
            float distance = (float)Math.Sqrt(dx * dx + dy * dy);

            if (distance < r + r1)
                return true;
            else
                return false;
        }
        // List utils
        public static void Swap<T>(IList<T> list, int indexA, int indexB)
        {
            T tmp = list[indexA];
            list[indexA] = list[indexB];
            list[indexB] = tmp;
        }
        public static void MoveToFront<T>(List<T> listNode, T element) where T : ZIndex
        {
            int index = element._index;

            for (int i = index; i < listNode.Count - 1; ++i)
            {
                Swap(listNode, i, i + 1);
            }

            listNode[listNode.Count - 1] = element;
        }
    }

    public class Array
    {
        public static bool IsExist<T>(List<T> list, int index)
        {
            if (index >= 0 && index < list.Count)
                if (null != list[index])
                    return true;

            return false;
        }
    }


    public static class ListExtra
    {
        public static void Resize<T>(this List<T> list, int sz, T c)
        {
            if (null != list)
            {
                int cur = list.Count;
                if (sz < cur)
                    list.RemoveRange(sz, cur - sz);
                else if (sz > cur)
                {
                    if (sz > list.Capacity)//this bit is purely an optimisation, to avoid multiple automatic capacity changes.
                        list.Capacity = sz;
                    list.AddRange(Enumerable.Repeat(c, sz - cur));
                }
            }
            else
            {
                Console.WriteLine("Error >> ListExtra Resize : list is null !");
            }
        }
        public static void Resize<T>(this List<T> list, int sz) where T : class, new()
        {
            //Resize(list, sz, new T());
            list!.Resize(sz, default);
        }
    }
}
