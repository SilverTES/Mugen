
namespace Mugen.Event
{
    [Obsolete]
    public class TimerEvent
    {
        bool[] _active;
        float[] _timers;
        float[] _tics;
        bool[] _on;
        bool[] _repeat;

        float[] _factorTimes;

        public TimerEvent(int nbTimer)
        {
            _active = new bool[nbTimer];
            _timers = new float[nbTimer];
            _tics = new float[nbTimer];
            _on = new bool[nbTimer];
            _repeat = new bool[nbTimer];

            _factorTimes = new float[nbTimer];

            for (int i = 0; i < nbTimer; i++)
                _factorTimes[i] = 1f;
        }

        private static float TimeToFrame(float hours, float minutes, float seconds)
        {
            return hours * 216000 + minutes * 3600 + seconds * 60;
        }

        public static float Time(float hours, float minutes, float seconds)
        {
            return 1f / (float)TimeToFrame(hours, minutes, seconds);
        }

        public void SetTimer(int idTimer, float tic, bool repeat = false)
        {
            _tics[idTimer] = tic;
            _repeat[idTimer] = repeat;
        }
        public void SetTimeFactor(int idTimer, float timeFactor = 1f)
        {
            _factorTimes[idTimer] = timeFactor;
        }
        public float GetTimer(int idTimer)
        {
            return _timers[idTimer];
        }
        public void StartTimer(int idTimer)
        {
            _active[idTimer] = true;
            _timers[idTimer] = 1f;
        }
        public void PauseTimer(int idTimer)
        {
            _active[idTimer] = false;
        }
        public void ResumeTimer(int idTimer)
        {
            _active[idTimer] = true;
        }
        public void StopTimer(int idTimer)
        {
            _active[idTimer] = false;
            _timers[idTimer] = 1f;
        }
        public void SetTimerOn(int idTimer)
        {
            _on[idTimer] = true;
            _timers[idTimer] = 1f;
        }
        public void SetTimerOff(int idTimer)
        {
            _on[idTimer] = false;
            _timers[idTimer] = 1f;
        }
        public bool OnTimer(int idTimer)
        {
            return _on[idTimer];
        }

        public void Update(float deltaTime = 1f)
        {
            for (int i = 0; i < _timers.Length; i++)
            {
                if (_active[i])
                    _timers[i] -= _tics[i] * _factorTimes[i] * deltaTime;

                if (_timers[i] <= 0f)
                {
                    _on[i] = true;
                    _timers[i] = 1f;

                    if (_repeat[i])
                        StartTimer(i);
                }
                else
                {
                    _on[i] = false;
                }

            }
        }

    }
    public class Timer
    {
        private static float TimeToFrame(float hours, float minutes, float seconds)
        {
            return hours * 216000 + minutes * 3600 + seconds * 60;
        }

        public static float Time(float hours, float minutes, float seconds)
        {
            return 1f / (float)TimeToFrame(hours, minutes, seconds);
        }
    }
    public class Timer<T> where T : Enum
    {
        bool[] _active;
        float[] _timers;
        float[] _tics;
        bool[] _on;
        Action[] _onTimer;
        bool[] _repeat;

        float[] _factorTimes;

        public Timer()
        {
            var enums = Enum.GetValues(typeof(T));

            int nbTimer = enums.Length;

            _active = new bool[nbTimer];
            _timers = new float[nbTimer];
            _tics = new float[nbTimer];
            _on = new bool[nbTimer];
            _onTimer = new Action[nbTimer];
            _repeat = new bool[nbTimer];

            _factorTimes = new float[nbTimer];

            for (int i = 0; i < nbTimer; i++)
            {
                _active[i] = false;
                _timers[i] = 1f;
                _on[i] = false;
                _repeat[i] = false;
                _factorTimes[i] = 1f;
            }
        }
        public bool IsActive(T IdTimer)
        {
            int idTimer = Convert.ToInt32(IdTimer);
            return _active[idTimer];
        }
        public void SetOnActive(T IdTimer, bool active = true)
        {
            int idTimer = Convert.ToInt32(IdTimer);
            SetOn(IdTimer);
            _active[idTimer] = active;
        }
        public void On(T IdTimer, Action action)
        {
            int idTimer = Convert.ToInt32(IdTimer);
            _onTimer[idTimer] += action;
        }
        public void Set(T IdTimer, float tic, bool repeat = false)
        {
            int idTimer = Convert.ToInt32(IdTimer);

            _tics[idTimer] = tic;
            _repeat[idTimer] = repeat;
        }
        public void SetTimeFactor(T IdTimer, float timeFactor = 1f)
        {
            int idTimer = Convert.ToInt32(IdTimer);
            _factorTimes[idTimer] = timeFactor;
        }
        public float Get(T IdTimer)
        {
            int idTimer = Convert.ToInt32(IdTimer);
            return _timers[idTimer];
        }
        public void Start(T IdTimer)
        {
            int idTimer = Convert.ToInt32(IdTimer);
            _active[idTimer] = true;
            _timers[idTimer] = 1f;
        }
        public void Pause(T IdTimer)
        {
            int idTimer = Convert.ToInt32(IdTimer);
            _active[idTimer] = false;
        }
        public void Resume(T IdTimer)
        {
            int idTimer = Convert.ToInt32(IdTimer);
            _active[idTimer] = true;
        }
        public void Stop(T IdTimer)
        {
            int idTimer = Convert.ToInt32(IdTimer);
            _active[idTimer] = false;
            _timers[idTimer] = 1f;
        }
        public void SetOn(T IdTimer)
        {
            int idTimer = Convert.ToInt32(IdTimer);
            _on[idTimer] = true;
            _timers[idTimer] = 1f;
        }
        public void SetOff(T IdTimer)
        {
            int idTimer = Convert.ToInt32(IdTimer);
            _on[idTimer] = false;
            _timers[idTimer] = 1f;
        }
        public bool On(T IdTimer)
        {
            int idTimer = Convert.ToInt32(IdTimer);
            return _on[idTimer];
        }

        public void Update(float deltaTime = 1f)
        {
            for (int i = 0; i < _timers.Length; i++)
            {
                if (_active[i])
                    _timers[i] -= _tics[i] * _factorTimes[i] * deltaTime;

                if (_timers[i] <= 0f)
                {
                    _on[i] = true;
                    _onTimer[i]?.Invoke();
                    _timers[i] = 1f;

                    if (_repeat[i])
                    {
                        _active[i] = true;
                        _timers[i] = 1f;
                    }
                }
                else
                {
                    _on[i] = false;
                }

            }
        }

    }

}
