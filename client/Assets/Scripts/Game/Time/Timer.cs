using System;

namespace AM.Game
{
    public class Timer : IComparable<Timer>
    {
        private float interval;
        private System.Action<Timer> cb;
        private int times;
        private float next;

        private Timer(float interval, System.Action<Timer> cb, int times, float firstInterval = -1)
        {
            if (interval < 0.01f) {
                interval = 0.01f;
            }
            this.interval = interval;
            this.next = TimeExt.GameTime + firstInterval >= 0 ? firstInterval : interval;
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


        #region timer manager
        private static MinHeap<Timer> _timers = new MinHeap<Timer>();

        static public Timer DelayCall(float interval, System.Action<Timer> cb)
        {
            return Schduler(interval, cb, 1);
        }

        /// <summary>
        /// 添加定时器
        /// </summary>
        /// <param name="interval"></param>
        /// <param name="cb"></param>
        /// <param name="times">-1表示循环</param>
        /// <param name="firstInterval"></param>
        /// <returns></returns>
        static public Timer Schduler(float interval, System.Action<Timer> cb, int times = -1, float firstInterval = 0.0f)
        {
            var timer = new Timer(interval, cb, times, firstInterval);
            _timers.Push(timer);
            return timer;
        }


        static public void Update()
        {
            var now = TimeExt.GameTime;
            for (var min = _timers.Min(); min != null; min = _timers.Min()) {
                if (min.times == 0) {
                    _timers.Pop();
                    continue;
                }
                if (now < min.next) {
                    break;
                }
                if (min.times > 0) {
                    --min.times;
                }
                min.cb?.Invoke(min);
                if (min.times != 0) {
                    min.next = now + min.interval;
                    _timers.Fix(0);
                }
                else {
                    _timers.Pop();
                }
            }
        }
        #endregion

    }
}
