using Microsoft.Xna.Framework;
using Vector2 = Microsoft.Xna.Framework.Vector2;

namespace Mugen.Animation
{
    public class Frame //: IClone<Frame>
    {
        public float _duration; // Delay of the current frame
        public Rectangle _rectSrc; // rect of the frame X,Y,W,H
        public Rectangle _rectDest; // rect of the final frame X,Y,W,H

        public float _oX;
        public float _oY;
        public Vector2 _center = new();
        public Vector2 _flipOrigin = new();
        public float _scaleX;
        public float _scaleY;
        public float _rotation;
        public int _flip;

        //public Frame Clone()
        //{
        //    Frame clone = (Frame)MemberwiseClone();

        //    return clone;
        //}

        public Frame(Rectangle rectSrc = default(Rectangle), float duration = 0, float oX = 0, float oY = 0, Rectangle rectDest = default(Rectangle), float scaleX = 1, float scaleY = 1, float rotation = 0.0f, int flip = 0)
        {
            _rectSrc = rectSrc;

            if (rectDest != default(Rectangle))
                _rectDest = rectDest;
            else
                _rectDest = _rectSrc;

            _duration = duration;

            _oX = oX;
            _oY = oY;

            _center.X = _rectSrc.Width / 2;
            _center.Y = _rectSrc.Height / 2;
            _flipOrigin.X = _center.X - (_oX - _center.X);
            _flipOrigin.Y = _center.Y - (_oY - _center.Y);
            _scaleX = scaleX;
            _scaleY = scaleY;
            _rotation = rotation;
            _flip = flip;
        }

        public Frame SetRectSrc(Rectangle rectSrc) { _rectSrc = rectSrc; return this; }
        public Frame SetRectDest(Rectangle rectDest) { _rectDest = rectDest; return this; }

        public Frame SetSrcPosition(int top, int left)
        {
            _rectSrc.X = top;
            _rectSrc.Y = left;
            return this;
        }
        public Frame SetSrcSize(int width, int height)
        {
            _rectSrc.Width = width;
            _rectSrc.Height = height;
            return this;
        }
        public Frame SetDesPosition(int top, int left)
        {
            _rectDest.X = top;
            _rectDest.Y = left;
            return this;
        }
        public Frame SetDestSize(int width, int height)
        {
            _rectDest.Width = width;
            _rectDest.Height = height;
            return this;
        }
        public Frame SetDuration(float duration) { _duration = duration; return this; }
        public Frame SetRotation(float rotation) { _rotation = rotation; return this; }

        public Frame SetPivot(float oX, float oY)
        {
            _oX = oX;
            _oY = oY;
            return this;
        }

        public Frame SetScaleX(float scaleX) { _scaleX = scaleX; return this; }
        public Frame SetScaleY(float scaleY) { _scaleY = scaleY; return this; }

        public Frame SetScale(float scaleX, float scaleY)
        {
            _scaleX = scaleX;
            _scaleY = scaleY;
            return this;
        }
        public Frame SetFlip(int flip)
        {
            _flip = flip;
            return this;
        }

    }
}
