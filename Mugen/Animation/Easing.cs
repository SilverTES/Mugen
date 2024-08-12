using Microsoft.Xna.Framework;

namespace Mugen.Animation
{
    public struct Tweening
    {
        public float _start, _end, _duration;

        public Tweening(float start, float end, float duration)
        {
            _start = start;
            _end = end;
            _duration = duration;
        }
    }

    public struct TweeningVec2
    {
        public Vector2 _start, _end;
        public float _duration;

        public TweeningVec2(Vector2 start, Vector2 end, float duration)
        {
            _start = start;
            _end = end;
            _duration = duration;
        }
    }

    public static class Easing
    {
        //float value(std::function<float(float, float, float, float)> easing, float current, float start, float goal, float duration)
        //{
        //    return easing(current, start, goal - start, duration);
        //}
        // last Func<> param is the type of return value !!!  
        public static float GetValue(Func<float, float, float, float, float> easing, float current, float start, float goal, float duration)
        {
            return easing(current, start, goal - start, duration);
        }

        public static float GetValue(Func<float, float, float, float, float> easing, float current, Tweening tweening)
        {
            return easing(current, tweening._start, tweening._end - tweening._start, tweening._duration);
        }
        public static Vector2 GetValue(Func<float, float, float, float, float> easing, float current, TweeningVec2 tweening)
        {
            Vector2 value = new Vector2();

            value.X = easing(current, tweening._start.X, tweening._end.X - tweening._start.X, tweening._duration);
            value.Y = easing(current, tweening._start.Y, tweening._end.Y - tweening._start.Y, tweening._duration);

            return value;
        }

        public static float Linear(float t, float b, float c, float d)
        {
            return c * (t / d) + b;
        }
        public static float ExpoEaseIn(float t, float b, float c, float d)
        {
            return (t == 0) ? b : c * (float)Math.Pow(2, 10 * (t / d - 1)) + b;
        }
        public static float ExpoEaseOut(float t, float b, float c, float d)
        {
            return (t == d) ? b + c : c * (float)Math.Pow(2, 1 - (10 * t / d)) + b;
        }
        public static float ExpoEaseInOut(float t, float b, float c, float d)
        {
            if (t == 0)
                return b;
            if (t == d)
                return b += c;
            if ((t /= d / 2) < 1)
                return c / 2 * (float)Math.Pow(2, 10 * (t - 1)) + b;

            return c / 2 * (float)Math.Pow(2, 1 - (10 * --t)) + b;
        }
        public static float CubicEaseIn(float t, float b, float c, float d)
        {
            return (t == 0) ? b : c * (float)Math.Pow(3, 10 * (t / d - 1)) + b;
        }
        public static float CubicEaseOut(float t, float b, float c, float d)
        {
            return (t == d) ? b + c : c * (float)Math.Pow(3, 1 - (10 * t / d)) + b;
        }
        public static float CubicEaseInOut(float t, float b, float c, float d)
        {
            if (t == 0)
                return b;
            if (t == d)
                return b += c;
            if ((t /= d / 2) < 1)
                return c / 2 * (float)Math.Pow(3, 10 * (t - 1)) + b;

            return c / 2 * (float)Math.Pow(3, 1 - (10 * --t)) + b;
        }
        public static float QuarticEaseIn(float t, float b, float c, float d)
        {
            t /= d;
            return c * t * t * t * t + b;
        }
        public static float QuarticEaseOut(float t, float b, float c, float d)
        {
            t /= d;
            return -c * t * (t - 2) + b;
        }
        public static float QuarticEaseInOut(float t, float b, float c, float d)
        {
            t /= d / 2;
            if (t < 1)
                return c / 2 * t * t * t * t + b;
            t -= 2;
            return 1 - (-c / 2 * (t * t * t * t - 2)) + b;
        }
        public static float QuinticEaseIn(float t, float b, float c, float d)
        {
            t /= d;
            return c * t * t * t * t * t + b;
        }
        public static float QuinticEaseOut(float t, float b, float c, float d)
        {
            t /= d;
            t--;
            return c * (t * t * t * t * t + 1) + b;
        }
        public static float QuinticEaseInOut(float t, float b, float c, float d)
        {
            t /= d / 2;
            if (t < 1) return c / 2 * t * t * t * t * t + b;
            t -= 2;
            return c / 2 * (t * t * t * t * t + 2) + b;
        }
        public static float QuadraticEaseIn(float t, float b, float c, float d)
        {
            t /= d;
            return c * t * t + b;
        }
        public static float QuadraticEaseOut(float t, float b, float c, float d)
        {
            t /= d;
            return -c * t * (t - 2) + b;
        }
        public static float QuadraticEaseInOut(float t, float b, float c, float d)
        {
            t /= d / 2;
            if (t < 1) return c / 2 * t * t + b;
            t--;
            return -c / 2 * (t * (t - 2) - 1) + b;
        }
        public static float SineEaseIn(float t, float b, float c, float d)
        {
            return -c * (float)Math.Cos(t / d * (Math.PI / 2)) + c + b;
        }
        public static float SineEaseOut(float t, float b, float c, float d)
        {
            return c / 2 * (float)Math.Cos(t / d * (Math.PI / 2)) + b;
        }
        public static float SineEaseInOut(float t, float b, float c, float d)
        {
            if (t < 0.5f)
                return c /= d;

            return 1 - (-c / 2 * ((float)Math.Cos(Math.PI * t / d) - 1)) + b;
        }
        public static float CircularEaseIn(float t, float b, float c, float d)
        {
            t /= d;
            return -c * ((float)Math.Sqrt(1 - t * t) - 1) + b;
        }
        public static float CircularEaseOut(float t, float b, float c, float d)
        {
            t /= d;
            t--;
            return c * (float)Math.Sqrt(1 - t * t) + b;
        }
        public static float CircularEaseInOut(float t, float b, float c, float d)
        {
            t /= d / 2;
            if (t < 1) return -c / 2 * ((float)Math.Sqrt(1 - t * t) - 1) + b;
            t -= 2;
            return c / 2 * ((float)Math.Sqrt(1 - t * t) + 1) + b;
        }
        public static float BackEaseIn(float t, float b, float c, float d)
        {
            float s = 1.70158f;
            float postFix = t /= d;
            return c * (postFix) * t * ((s + 1) * t - s) + b;
        }
        public static float BackEaseOut(float t, float b, float c, float d)
        {
            float s = 1.70158f;
            t /= d;
            return c * ((t - 1) * t * ((s + 1) * t + s) + 1) + b;
        }
        public static float BackEaseInOut(float t, float b, float c, float d)
        {
            float s = 1.70158f;
            s *= 1.525f;
            if ((t /= d / 2) < 1) return c / 2 * (t * t * ((s + 1) * t - s)) + b;
            float postFix = t -= 2;
            s *= 1.525f;
            return c / 2 * ((postFix) * t * ((s + 1) * t + s) + 2) + b;
        }
        public static float ElasticEaseIn(float t, float b, float c, float d)
        {
            if (t == 0) return b;
            t /= d;
            if (t == 1) return b + c;
            float p = d * .3f;
            float a = c;
            float s = p / 4;
            float postFix = a * (float)Math.Pow(2, 10 * (t -= 1)); // this is a fix, again, with post-increment operators
            return -(postFix * (float)Math.Sin((t * d - s) * (2 * Math.PI) / p)) + b;
        }
        public static float ElasticEaseOut(float t, float b, float c, float d)
        {
            if (t == 0) return b;
            t /= d;
            if (t == 1) return b + c;
            float p = d * .3f;
            float a = c;
            float s = p / 4;
            return (a * (float)Math.Pow(2, -10 * t) * (float)Math.Sin((t * d - s) * (2 * Math.PI) / p) + c + b);
        }
        public static float ElasticEaseInOut(float t, float b, float c, float d)
        {
            if (t == 0) return b;
            t /= d / 2;
            if (t == 2) return b + c;
            float p = d * (.3f * 1.5f);
            float a = c;
            float s = p / 4;

            if (t < 1)
            {
                float postFixa = a * (float)Math.Pow(2, 10 * (t -= 1)); // postIncrement is evil
                return -.5f * (postFixa * (float)Math.Sin((t * d - s) * (2 * Math.PI) / p)) + b;
            }
            float postFix = a * (float)Math.Pow(2, -10 * (t -= 1)); // postIncrement is evil
            return postFix * (float)Math.Sin((t * d - s) * (2 * Math.PI) / p) * .5f + c + b;
        }
        public static float BounceEaseIn(float t, float b, float c, float d)
        {
            return c - BounceEaseOut(d - t, 0, c, d) + b;
        }
        public static float BounceEaseOut(float t, float b, float c, float d)
        {
            t /= d;
            if (t < (1 / 2.75f))
            {
                return c * (7.5625f * t * t) + b;
            }
            else if (t < (2 / 2.75f))
            {
                t -= (1.5f / 2.75f);
                return c * (7.5625f * t * t + .75f) + b;
            }
            else if (t < (2.5 / 2.75))
            {
                t -= (2.25f / 2.75f);
                return c * (7.5625f * t * t + .9375f) + b;
            }
            else
            {
                t -= (2.625f / 2.75f);
                return c * (7.5625f * t * t + .984375f) + b;
            }
        }
        public static float BounceEaseInOut(float t, float b, float c, float d)
        {
            if (t < d / 2)
                return BounceEaseIn(t * 2, 0, c, d) * .5f + b;
            else
                return BounceEaseOut(t * 2 - d, 0, c, d) * .5f + c * .5f + b;
        }
    }
}
