using System;
using UnityEngine;
using Automata.Base;
using Automata.Adapter;

namespace Automata.Game
{
    public class Timer : IComparable<Timer>
    {
        public float interval;
        public DateTime next;
        public System.Action<Timer> cb;
        public int times;

        public Timer(float interval, System.Action<Timer> cb, int times)
        {
            if (interval < 0.01f)
            {
                interval = 0.01f;
            }
            this.interval = interval;
            this.next = TimeMgr.Instance.Now.AddSeconds(interval);
            this.cb = cb;
            this.times = times;
        }

        override public string ToString()
        {
            return String.Format("Timer:interval,times,next={0},{1},{2}", interval, times, next);
        }
        public int CompareTo(Timer other)
        {
            return next.CompareTo(other.next);
        }

        public void Cancel()
        {
            times = 0;
        }

    }

    public class TimeMgr : Singleton<TimeMgr>
    {
        private DateTime _now;
        private MinHeap<Timer> _timers = new MinHeap<Timer>();

        public void Initialize()
        {
            _now = DateTime.Now;
            MonoProxy.Instance.Adapter.update += Update;
        }

        public DateTime Now { get { return _now; } }

        #region timer
        public Timer DelayCall(float interval, System.Action<Timer> cb)
        {
            var timer = new Timer(interval, cb, 1);
            _timers.Push(timer);
            return timer;
        }

        public Timer Schduler(float interval, System.Action<Timer> cb, int times=-1, float firstInterval=0.0f)
        {
            var timer = new Timer(interval, cb, times)
            {
                next = Now.AddSeconds(firstInterval)
            };
            _timers.Push(timer);
            return timer;
        }

        #endregion

        public void Update()
        {
            _now = _now.AddSeconds(Time.deltaTime);
            for (var min = _timers.Min(); min != null; min = _timers.Min())
            {
                if (min.times == 0)
                {
                    _timers.Pop();
                    continue;
                }
                if (_now < min.next)
                {
                    break;
                }
                if (min.times > 0)
                {
                    --min.times;
                }
                min.cb?.Invoke(min);
                if (min.times != 0)
                {
                    min.next = _now.AddSeconds(min.interval);
                    _timers.Fix(0);
                }
                else
                {
                    _timers.Pop();
                }
            }

        }
    }
}
