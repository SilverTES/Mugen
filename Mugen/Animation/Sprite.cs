using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Mugen.Core;

namespace Mugen.Animation
{
    public enum Loops
    {
        NONE = -1,
        ONCE,
        REPEAT,
        PINGPONG
    }

    public class Animation // : IClone<Animation>
    {
        static Rectangle _destRect = new Rectangle(); // no need to copy !

        public Texture2D _spriteSheet;
        public Loops _loop = Loops.NONE;
        public string _name;
        public List<Frame> _frames = new List<Frame>();


        //public Animation Clone()
        //{
        //    Animation clone = (Animation)MemberwiseClone();

        //    return clone;
        //}

        public Animation(Texture2D spriteSheet, string name)
        {
            _spriteSheet = spriteSheet;
            _name = name;
        }
        public Animation SetName(string name)
        {
            _name = name;
            return this;
        }
        public Animation SetLoop(Loops loop)
        {
            _loop = loop;
            return this;
        }
        public Animation SetDuration(float duration)
        {
            for (int i = 0; i < _frames.Count; i++)
            {
                if (null != _frames[i])
                    _frames[i].SetDuration(duration);
            }
            return this;
        }
        public Animation Add(Frame frame)
        {
            _frames.Add(frame);
            return this;
        }
        public Frame? Get(int index)
        {
            if (index < 0 || index > _frames.Count) return null;
            return _frames[index];
        }
        public Animation AppendTo(Sprite sprite, string name = "")
        {
            if (null != sprite)
            {
                if (name == "")
                    sprite.Add(this, _name);
                else
                {
                    _name = name;
                    sprite.Add(this, _name);
                }
            }
            return this;
        }

        //[Obsolete]
        public void Draw(SpriteBatch batch, Frame frame, float x, float y, Color color, float scaleX = 1, float scaleY = 1, float rotation = 0.0f, float translateX = 0, float translateY = 0, SpriteEffects spriteEffects = SpriteEffects.None)
        {
            _destRect.X = (int)(x + translateX);
            _destRect.Y = (int)(y + translateY);
            _destRect.Width = (int)(frame._rectDest.Width * frame._scaleX * scaleX);
            _destRect.Height = (int)(frame._rectDest.Height * frame._scaleY * scaleY);

            SpriteEffects flip = (spriteEffects == SpriteEffects.None) ? (SpriteEffects)frame._flip : spriteEffects;

            batch.Draw
            (
                _spriteSheet,
                _destRect, // new Vector2(x,y)
                frame._rectSrc,
                color,
                frame._rotation + rotation,
                new Vector2(frame._oX, frame._oY),
                //new Vector2(GetFrame()._scaleX, GetFrame()._scaleY),
                flip,
                0
            );

        }
        public void Draw(SpriteBatch batch, int indexFrame, float x, float y, Color color, float scaleX = 1, float scaleY = 1, float rotation = 0.0f, float translateX = 0, float translateY = 0, SpriteEffects spriteEffects = SpriteEffects.None)
        {
            if (indexFrame < 0 || indexFrame > _frames.Count) return;
            Draw(batch, _frames[indexFrame], x, y, color, scaleX, scaleY, rotation, translateX, translateY, spriteEffects);
        }


    }

    public class Sprite : IClone<Sprite>
    {
        #region

        static Rectangle _rectDest = new Rectangle(); // no need to copy !

        string _curAnimation = "";
        public string CurAnimation => _curAnimation;
        public int CurFrame { get; private set; }
        public float CurDuration { get; private set; }
        public float DurationFactor { get; private set; } = 1f;

        int _direction = 1;
        int _firstIndex = 0;
        public int _lastIndex = 1;

        public bool IsPlay { get; private set; } = false; // status  : is play Animation
        public bool OnPlay { get; private set; } = false; // trigger : on play Animation
        public bool OffPlay { get; private set; } = false;// trigger : off play Animation
        public bool OnChangeFrame { get; private set; } = false;// trigger : on change Frame 

        Frame? _currentFrame; // Current frame !
        public Frame? CurrentFrame => _currentFrame; // Get current Frame played !
        Texture2D? _currentSpriteSheet; // Current spriteSheet !
        Loops _currentLoop; // Current loop !

        public Animation GetAnimation(string animationName) => _animations[animationName];
        public Frame? GetFrame(int index) => _animations[_curAnimation].Get(index);

        //Animate _currentAnimation; // Current Animation !

        Dictionary<string, Animation> _animations = new Dictionary<string, Animation>();
        public int NbAnimation => _animations.Count;

        #endregion

        public Sprite Clone()
        {
            Sprite clone = (Sprite)MemberwiseClone();

            // -- Clone Addons

            //clone._animations = new Dictionary<string, Animation>();
            //foreach (KeyValuePair<string, Animation> entry in _animations)
            //{
            //    clone._animations[entry.Key] = entry.Value.Clone();
            //}

            return clone;
        }

        public Sprite Add(Animation animation, string name = "") // You can rename the Added action
        {
            if (name == "")
                _animations.Add(animation._name, animation);
            else
                _animations.Add(name, animation);
            return this;
        }

        public Sprite Start(string curAnimation, int direction, int curFrame, int firstIndex, int lastIndex)
        {
            if (_animations.ContainsKey(curAnimation))
            {
                _curAnimation = curAnimation;
                _direction = direction;
                CurFrame = curFrame;
                _firstIndex = firstIndex;
                _lastIndex = lastIndex;
            }

            Resume();

            return this;
        }
        public Sprite Start(string curAnimation, int direction, int curFrame, int firstIndex = 0)
        {
            if (_animations.ContainsKey(curAnimation))
            {
                _curAnimation = curAnimation;
                _direction = direction;
                CurFrame = curFrame;
                _firstIndex = firstIndex;
                _lastIndex = _animations[_curAnimation]._frames.Count - 1;
            }

            Resume();

            return this;
        }
        public int NbFrame()
        {
            if (_animations.ContainsKey(_curAnimation))
                return _animations[_curAnimation]._frames.Count;
            return 0;
        }
        public int NbFrame(string curAnimation)
        {
            if (_animations.ContainsKey(curAnimation))
                return _animations[curAnimation]._frames.Count;
            return 0;
        }
        public Animation? Animation(string animationName)
        {
            if (_animations.ContainsKey(animationName))
                return _animations[animationName];

            return null;
        }
        public Sprite SetAnimation(string curAnimation, int direction = 1, int curFrame = 0, int firstIndex = 0)
        {
            if (_curAnimation.Equals(curAnimation)) // If same curAnimation then do nothing !
                return this;

            if (_animations.ContainsKey(curAnimation)) // BugFix : Reset Animation Player !
            {
                _curAnimation = curAnimation;
                _direction = direction;
                _firstIndex = firstIndex;
                CurFrame = curFrame;
                _lastIndex = _animations[_curAnimation]._frames.Count - 1;
            }

            return this;
        }
        public Sprite SetDirection(int direction)
        {
            _direction = direction;
            return this;
        }
        public Sprite SetDurationFactor(float durationFactor)
        {
            DurationFactor = durationFactor;
            return this;
        }

        // Player

        public void PlayAt(int curFrame) { CurFrame = curFrame; }
        public void Pause() { IsPlay = false; }
        public void Resume()
        {
            IsPlay = true;
            OnPlay = true;
        }
        public void StopAt(int curFrame)
        {
            IsPlay = false;
            CurFrame = curFrame;
        }

        public void Update()
        {
            OffPlay = false;

            if (_animations.ContainsKey(_curAnimation))
            {
                _currentFrame = _animations[_curAnimation]._frames[CurFrame];
                _currentSpriteSheet = _animations[_curAnimation]._spriteSheet;
                _currentLoop = _animations[_curAnimation]._loop;
            }

            if (null != _currentFrame && IsPlay)
            {
                OnPlay = false;
                OnChangeFrame = false;

                ++CurDuration;
                if (CurDuration > _currentFrame._duration * DurationFactor) // Check CurDuration reach _duration Frame
                {
                    CurDuration = 0;

                    OnChangeFrame = true;
                    CurFrame += _direction;
                    if (CurFrame < _firstIndex) // Check CurFrame reach the _firstIndex Action
                    {
                        if (_currentLoop == Loops.REPEAT)
                        {
                            CurFrame = _lastIndex;
                        }
                        else if (_currentLoop == Loops.PINGPONG)
                        {
                            CurFrame = _firstIndex;
                            _direction = -_direction;
                        }
                        else
                        {
                            CurFrame = _firstIndex;
                            IsPlay = false;
                            OffPlay = true;
                        }
                    }
                    else if (CurFrame > _lastIndex) // Check CurFrame reach the _lastIndex Action
                    {
                        if (_currentLoop == Loops.REPEAT)
                        {
                            CurFrame = _firstIndex;
                        }
                        else if (_currentLoop == Loops.PINGPONG)
                        {
                            CurFrame = _lastIndex;
                            _direction = -_direction;
                        }
                        else
                        {
                            CurFrame = _lastIndex;
                            IsPlay = false;
                            OffPlay = true;
                        }

                    }
                }
            }
        }

        [Obsolete]
        public void Draw(SpriteBatch batch, float x, float y, Color color, float scaleX = 1, float scaleY = 1, float rotation = 0.0f, float translateX = 0, float translateY = 0, SpriteEffects spriteEffects = SpriteEffects.None)
        {
            if (null != _currentFrame && null != _currentSpriteSheet)
            {
                _rectDest.X = (int)(x + translateX);
                _rectDest.Y = (int)(y + translateY);
                _rectDest.Width = (int)(_currentFrame._rectDest.Width * _currentFrame._scaleX * scaleX);
                _rectDest.Height = (int)(_currentFrame._rectDest.Height * _currentFrame._scaleY * scaleY);

                SpriteEffects flip = (spriteEffects == SpriteEffects.None) ? (SpriteEffects)_currentFrame._flip : spriteEffects;

                batch.Draw
                (
                    _currentSpriteSheet,
                    _rectDest, // new Vector2(x,y)
                    _currentFrame._rectSrc,
                    color,
                    _currentFrame._rotation + rotation,
                    new Vector2(_currentFrame._oX, _currentFrame._oY),
                    //new Vector2(GetFrame()._scaleX, GetFrame()._scaleY),
                    flip,
                    0
                );

            }
        }

        public void Render(SpriteBatch batch, float x, float y, Color color, float scaleX = 1, float scaleY = 1, float rotation = 0.0f, float translateX = 0, float translateY = 0, SpriteEffects spriteEffects = SpriteEffects.None, bool flipX = false, bool flipY = false)
        {
            if (null != _currentFrame && null != _currentSpriteSheet)
            {
                //_rectDest.X = (int)(x + translateX);
                //_rectDest.Y = (int)(y + translateY);
                //_rectDest.Width = (int)(_currentFrame._rectDest.Width * _currentFrame._scaleX * scaleX);
                //_rectDest.Height = (int)(_currentFrame._rectDest.Height * _currentFrame._scaleY * scaleY);

                //SpriteEffects flip = (spriteEffects == SpriteEffects.None) ? (SpriteEffects)_currentFrame._flip : spriteEffects;

                Vector2 origin = new Vector2(_currentFrame._oX, _currentFrame._oY);

                if (flipX)
                    origin.X = _currentFrame._flipOrigin.X;

                if (flipY)
                    origin.Y = _currentFrame._flipOrigin.Y;


                //batch.Draw
                //(
                //    Retro2D.Draw._whitePixel,
                //     //_destRect,
                //     new Vector2(x, y),
                //    _currentFrame._rectSrc,
                //    Color.DarkSlateBlue,
                //    _currentFrame._rotation + rotation,
                //    //new Vector2(_currentFrame._oX, _currentFrame._oY),
                //    //new Vector2(_currentFrame._oX,
                //    //            _currentFrame._oY),
                //    origin,
                //    new Vector2(_currentFrame._scaleX * scaleX, _currentFrame._scaleY * scaleY),
                //    flip,
                //    0
                //);

                batch.Draw
                (
                    _currentSpriteSheet,
                     //_destRect,
                     new Vector2(x, y),
                    _currentFrame._rectSrc,
                    color,
                    _currentFrame._rotation + rotation,
                    //new Vector2(_currentFrame._oX, _currentFrame._oY),
                    //new Vector2(_currentFrame._oX,
                    //            _currentFrame._oY),
                    origin,
                    new Vector2(_currentFrame._scaleX * scaleX, _currentFrame._scaleY * scaleY),
                    spriteEffects,
                    0
                );

                // Debug Pivot Point
                //Retro2D.Draw.Point(batch, new Vector2(x, y), 4, Color.White);
                //Retro2D.Draw.Point(batch, new Vector2(x, y) + _currentFrame._centerOrigin, 4, Color.Gold);

                //Retro2D.Draw.Line(batch, new Vector2(x, y) + _currentFrame._center, new Vector2(x, y) + _currentFrame._centerOrigin, Color.Red, 2);

                //Retro2D.Draw.Point(batch, new Vector2(x + (_currentFrame._oX * _currentFrame._scaleX * scaleX), 
                //    y + (_currentFrame._oY * _currentFrame._scaleY * scaleY)) , 4, Color.HotPink);

                //Retro2D.Draw.Point(batch, new Vector2(x + _currentFrame._oX ,
                //    y + _currentFrame._oY ), 4, Color.OrangeRed);

                //Retro2D.Draw.Rectangle(batch, _currentFrame._rectSrc, Color.GreenYellow);
                //Retro2D.Draw.Rectangle(batch, _rectDest, Color.Yellow);


            }
        }

    }
}
