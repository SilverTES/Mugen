using Microsoft.Xna.Framework;
using System.Security.Cryptography;

namespace Mugen.Animation
{
    public class Motion
    {
        string _name = "";

        public Tweening _tweening;

        public Func<float, float, float, float, float> _easing;

        public Motion(string name)
        {
            _name = name;
            _easing = Easing.Linear;
            _tweening._start = 0f;
            _tweening._goal = 1f;
            _tweening._duration = 1f;
        }

        public Motion(string name, Func<float, float, float, float, float> easing, float start = 0f, float end = 1f, float duration = 1f)
        {
            _name = name;
            _easing = easing;
            _tweening._start = start;
            _tweening._goal = end;
            _tweening._duration = duration;
        }

        List<Frame> _frames = new List<Frame>();
    }

    public class MotionVec2
    {
        string _name = "";

        public TweeningVec2 _tweening;

        public Func<float, float, float, float, float> _easing;

        public MotionVec2(string name)
        {
            _name = name;
            _easing = Easing.Linear;
            _tweening._start = Vector2.Zero;
            _tweening._goal = Vector2.One;
            _tweening._duration = 1f;
        }

        public MotionVec2(string name, Func<float, float, float, float, float> easing, Vector2 start, Vector2 end, float duration)
        {
            _name = name;
            _easing = easing;
            _tweening._start = start;
            _tweening._goal = end;
            _tweening._duration = duration;
        }

        //List<Frame> _frames = new List<Frame>();
    }

    public class Animate
    {
        bool _isPlay = false;
        string _curMotion = "";
        public float _curFrame = 0;
        Dictionary<string, Motion> _motions = new Dictionary<string, Motion>();

        public void Start(string name)
        {
            _curMotion = name;
            _curFrame = 0;
        }
        public Dictionary<string, Motion> GetAll()
        {
            return _motions;
        }
        public void SetMotion(string name, Func<float, float, float, float, float> easing, float start, float goal, float duration)
        {
            if (_motions.ContainsKey(name))
            {
                _motions[name] = new Motion(name, easing, start, goal, duration);
            }
        }
        public void SetMotion(string name, Func<float, float, float, float, float> easing, Tweening tweening)
        {
            if (_motions.ContainsKey(name))
            {
                _motions[name] = new Motion(name, easing, tweening._start, tweening._goal, tweening._duration);
            }
        }
        public void Add(string name)
        {
            _motions.Add(name, new Motion(name));
        }
        public void Add(string name, Func<float, float, float, float, float> easing, float start, float goal, float duration)
        {
            _motions.Add(name, new Motion(name, easing, start, goal, duration));
        }
        public void Add(string name, Func<float, float, float, float, float> easing, Tweening tweening)
        {
            _motions.Add(name, new Motion(name, easing, tweening._start, tweening._goal, tweening._duration));
        }
        public Motion Of(string name)
        {
            return _motions[name];
        }
        private Tweening GetTweening()
        {
            if (_motions.ContainsKey(_curMotion))
                return _motions[_curMotion]._tweening;
            else
                return new Tweening();
        }

        public string StringCurMotion()
        {
            return _curMotion;
        }
        public bool IsPlay()
        {
            return _isPlay;
        }
        public void NextFrame(float step = 1f)
        {
            _isPlay = false;
            if (_curFrame < GetTweening()._duration)
            {
                _isPlay = true;
                _curFrame += step;
            }
        }
        public float Value()
        {

            if (_motions.ContainsKey(_curMotion))
                return Easing.GetValue(_motions[_curMotion]._easing, _curFrame, GetTweening());
            else
                return 0;
        }
        public void Transit(ref float value) // Copy reference to another var !
        {
            value = Value();
        }
        //public float Value(int curFrame)
        //{
        //    return Easing.GetValue(_sequences[_curSequence]._easing, curFrame, GetTransition());
        //}

        // Event
        [Obsolete]
        public bool OnBegin(string name)
        {
            if (name == _curMotion)
                return _curFrame == 0;
            else
                return false;
        }
        [Obsolete]
        public bool OnEnd(string name)
        {
            if (name == _curMotion)
                return (_curFrame == GetTweening()._duration && _isPlay);
            else
                return false;
        }
        public bool On(string name)
        {
            if (name == _curMotion)
                return _curFrame == 0;
            else
                return false;
        }
        public bool Off(string name)
        {
            if (name == _curMotion)
                return (_curFrame == GetTweening()._duration && _isPlay);
            else
                return false;
        }

    }

    public class AnimateVec2
    {
        bool _isPlay = false;
        string _curMotion = "";
        public float _curFrame = 0;
        Dictionary<string, MotionVec2> _motions = new Dictionary<string, MotionVec2>();

        public void Start(string name)
        {
            _curMotion = name;
            _curFrame = 0;
        }
        public Dictionary<string, MotionVec2> GetAll()
        {
            return _motions;
        }
        public void SetMotionVec2(string name, Func<float, float, float, float, float> easing, Vector2 start, Vector2 goal, float duration)
        {
            if (_motions.ContainsKey(name))
            {
                _motions[name] = new MotionVec2(name, easing, start, goal, duration);
            }
        }
        public void SetMotionVec2(string name, Func<float, float, float, float, float> easing, TweeningVec2 tweening)
        {
            if (_motions.ContainsKey(name))
            {
                _motions[name] = new MotionVec2(name, easing, tweening._start, tweening._goal, tweening._duration);
            }
        }
        public void Add(string name)
        {
            _motions.Add(name, new MotionVec2(name));
        }
        public void Add(string name, Func<float, float, float, float, float> easing, Vector2 start, Vector2 goal, float duration)
        {
            _motions.Add(name, new MotionVec2(name, easing, start, goal, duration));
        }
        public void Add(string name, Func<float, float, float, float, float> easing, TweeningVec2 tweening)
        {
            _motions.Add(name, new MotionVec2(name, easing, tweening._start, tweening._goal, tweening._duration));
        }
        public MotionVec2 Of(string name)
        {
            return _motions[name];
        }
        private TweeningVec2 GetTweening()
        {
            if (_motions.ContainsKey(_curMotion))
                return _motions[_curMotion]._tweening;
            else
                return new TweeningVec2();
        }

        public string StringCurMotion()
        {
            return _curMotion;
        }
        public bool IsPlay()
        {
            return _isPlay;
        }
        public void NextFrame(float step = 1f)
        {
            _isPlay = false;
            if (_curFrame < GetTweening()._duration)
            {
                _isPlay = true;
                _curFrame += step;
            }
        }
        public Vector2 Value()
        {

            if (_motions.ContainsKey(_curMotion))
                return Easing.GetValue(_motions[_curMotion]._easing, _curFrame, GetTweening());
            else
                return Vector2.Zero;
        }

        public void Transit(ref Vector2 value) // Copy reference to another var !
        {
            value = Value();
        }
        //public float Value(int curFrame)
        //{
        //    return Easing.GetValue(_sequences[_curSequence]._easing, curFrame, GetTransition());
        //}

        // Event
        [Obsolete]
        public bool OnBegin(string name)
        {
            if (name == _curMotion)
                return _curFrame == 0;
            else
                return false;
        }
        [Obsolete]
        public bool OnEnd(string name)
        {
            if (name == _curMotion)
                return (_curFrame == GetTweening()._duration && _isPlay);
            else
                return false;
        }
        public bool On(string name)
        {
            if (name == _curMotion)
                return _curFrame == 0;
            else
                return false;
        }
        public bool Off(string name)
        {
            if (name == _curMotion)
                return (_curFrame == GetTweening()._duration && _isPlay);
            else
                return false;
        }

    }

    public class Motion2D
    {
        public string Name = "";
        public bool IsPlay = false;
        public bool OnFinish = false;
        public float CurFrame = 0;
        public TweeningVec2 Tweening;

        public Func<float, float, float, float, float> Easing;

        public Motion2D(string name)
        {
            Name = name;
            Easing = Mugen.Animation.Easing.Linear;
            Tweening._start = Vector2.Zero;
            Tweening._goal = Vector2.One;
            Tweening._duration = 1f;
        }

        public Motion2D(string name, Func<float, float, float, float, float> easing, Vector2 start, Vector2 end, float duration)
        {
            Name = name;
            Easing = easing;
            Tweening._start = start;
            Tweening._goal = end;
            Tweening._duration = duration;
        }
    }

    public class Animate2D
    {
        Dictionary<string, Motion2D> _motion2Ds = new();
        public Animate2D() 
        { 

        }
        public void Start(string name)
        {
            if (_motion2Ds.ContainsKey(name))
            {
                _motion2Ds[name].OnFinish = false;
                _motion2Ds[name].CurFrame = 0;
                _motion2Ds[name].IsPlay = true;
            }
        }
        public Dictionary<string, Motion2D> GetAll()
        {
            return _motion2Ds;
        }
        public void SetMotion(string name, Func<float, float, float, float, float> easing, Vector2 start, Vector2 goal, float duration)
        {
            if (_motion2Ds.ContainsKey(name))
            {
                _motion2Ds[name] = new Motion2D(name, easing, start, goal, duration);
            }
        }
        public void SetMotion(string name, Func<float, float, float, float, float> easing, TweeningVec2 tweening)
        {
            if (_motion2Ds.ContainsKey(name))
            {
                _motion2Ds[name] = new Motion2D(name, easing, tweening._start, tweening._goal, tweening._duration);
            }
        }
        public void Add(string name)
        {
            _motion2Ds.Add(name, new Motion2D(name));
        }
        public void Add(string name, Func<float, float, float, float, float> easing, Vector2 start, Vector2 goal, float duration)
        {
            _motion2Ds.Add(name, new Motion2D(name, easing, start, goal, duration));
        }
        public void Add(string name, Func<float, float, float, float, float> easing, TweeningVec2 tweening)
        {
            _motion2Ds.Add(name, new Motion2D(name, easing, tweening._start, tweening._goal, tweening._duration));
        }
        public Motion2D Get(string name)
        {
            return _motion2Ds[name];
        }
        public bool IsPlay(string name)
        {
            if (_motion2Ds.ContainsKey(name))
            {
                return _motion2Ds[name].IsPlay;
            }
            return false;
        }
        public bool OnFinish(string name)
        {
            if (_motion2Ds.ContainsKey(name))
            {
                return _motion2Ds[name].OnFinish;
            }
            return false;
        }
        public void Update(string name, float step = 1f)
        {
            if (_motion2Ds.ContainsKey(name))
            {
                var motion2D = _motion2Ds[name];

                    motion2D.OnFinish = false;

                if (motion2D.IsPlay)
                    motion2D.CurFrame += step;

                if (motion2D.CurFrame >= motion2D.Tweening._duration)
                {
                    motion2D.OnFinish = true;
                    motion2D.CurFrame = 0f;
                    motion2D.IsPlay = false;
                }
            }
        }
        public void Update(float step = 1f)
        {
            foreach (var motion2D in _motion2Ds)
            {
                motion2D.Value.OnFinish = false;
                
                if (motion2D.Value.IsPlay)
                    motion2D.Value.CurFrame += step;

                if (motion2D.Value.CurFrame >= motion2D.Value.Tweening._duration)
                {
                    motion2D.Value.OnFinish = true;
                    motion2D.Value.CurFrame = 0f;
                    motion2D.Value.IsPlay = false;
                }

            }
        }
        public Vector2 Value(string name)
        {

            if (_motion2Ds.ContainsKey(name))
                return Easing.GetValue(_motion2Ds[name].Easing, _motion2Ds[name].CurFrame, _motion2Ds[name].Tweening);
            else
                return Vector2.Zero;
        }
        public void Transit(string name, ref Vector2 value) // Copy reference to another var !
        {
            value = Value(name);
        }
    }
}
