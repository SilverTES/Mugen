using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mugen.Event
{
    public class TimerEvent
    {
        bool[] _active;
        float[] _timers;
        float[] _tics;
        bool[] _on;

        float[] _factorTimes;

        public TimerEvent(int nbTimer)
        {
            _active = new bool[nbTimer];
            _timers = new float[nbTimer];
            _tics = new float[nbTimer];
            _on = new bool[nbTimer];

            _factorTimes = new float[nbTimer];

            for (int i = 0; i < nbTimer; i++)
                _factorTimes[i] = 1f;
        }

        public static float TimeToFrame(float hours, float minutes, float seconds)
        {
            return hours * 216000 + minutes * 3600 + seconds * 60;
        }

        public static float Time(float hours, float minutes, float seconds)
        {
            return 1f / (float)TimeToFrame(hours, minutes, seconds);
        }

        public void SetTimer(int idTimer, float tic)
        {
            _tics[idTimer] = tic;
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
                }
                else
                {
                    _on[i] = false;
                }

            }
        }

    }
}
