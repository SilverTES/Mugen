using Microsoft.Xna.Framework;

namespace Mugen.Animation
{
    public class Motion
    {
        string _name = "";

        public Tweening _tweening;

        public Func<float, float, float, float, float> _easing;

        public Motion(string name, Func<float, float, float, float, float> easing, float start, float end, float duration)
        {
            _name = name;
            _easing = easing;
            _tweening._start = start;
            _tweening._end = end;
            _tweening._duration = duration;
        }

        List<Frame> _frames = new List<Frame>();
    }

    public class MotionVec2
    {
        string _name = "";

        public TweeningVec2 _tweening;

        public Func<float, float, float, float, float> _easing;

        public MotionVec2(string name, Func<float, float, float, float, float> easing, Vector2 start, Vector2 end, float duration)
        {
            _name = name;
            _easing = easing;
            _tweening._start = start;
            _tweening._end = end;
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
        public void Add(string name, Func<float, float, float, float, float> easing, float start, float goal, float duration)
        {
            _motions.Add(name, new Motion(name, easing, start, goal, duration));
        }
        public void Add(string name, Func<float, float, float, float, float> easing, Tweening tweening)
        {
            _motions.Add(name, new Motion(name, easing, tweening._start, tweening._end, tweening._duration));
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
        public void Add(string name, Func<float, float, float, float, float> easing, Vector2 start, Vector2 goal, float duration)
        {
            _motions.Add(name, new MotionVec2(name, easing, start, goal, duration));
        }
        public void Add(string name, Func<float, float, float, float, float> easing, TweeningVec2 tweening)
        {
            _motions.Add(name, new MotionVec2(name, easing, tweening._start, tweening._end, tweening._duration));
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
}
