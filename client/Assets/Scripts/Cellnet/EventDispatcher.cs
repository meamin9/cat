using System;
using System.Collections.Generic;
using System.Threading;

namespace Cellnet
{
    public class EventDispatcher
    {
        Dictionary<uint, Action<object>> _msgCallbacks = new Dictionary<uint, Action<object>>();
        Dictionary<UInt16, Action<SessionEvent>> _seriesCallbacks = new Dictionary<UInt16, Action<SessionEvent>>();

        EventQueue _queue = new EventQueue();

        Thread _thread;
        bool _running;
        object _runningGuard = new object();
        AutoResetEvent _exitSignal = new AutoResetEvent(false);

        public EventQueue Queue
        {
            get{

                return _queue;
            }
        }

        public EventDispatcher( )
        {
            _queue.OnEvent += Invoke;
        }        

		public void AddSeriesCb(UInt16 series, Action<SessionEvent> cb) {
            if (series == 0) {
                UnityEngine.Debug.LogError("series为0，不能注册回调");
                return;
            }
            if (_seriesCallbacks.ContainsKey(series)) {
                UnityEngine.Debug.LogError("注册序列号回调覆盖");
            }
            _seriesCallbacks.Add(series, cb);
		}

        public void Add(uint msgid, Action<object> callback)
        {
            Action<object> callbacks;
            if (_msgCallbacks.TryGetValue(msgid, out callbacks))
            {
                callbacks += callback;
                _msgCallbacks[msgid] = callbacks;
            }
            else
            {
                callbacks += callback;

                _msgCallbacks.Add(msgid, callbacks);
            }
        }

        public void Remove(uint msgid, Action<object> callback)
        {
            Action<object> callbacks;
            if (_msgCallbacks.TryGetValue(msgid, out callbacks))
            {
                callbacks -= callback;
                _msgCallbacks[msgid] = callbacks;
            }
        }

        public void Invoke( object msg)
        {
            

            if (msg is SessionEvent )
            {
                var ev = (SessionEvent)msg;
                Action<object> callbacks;
				UnityEngine.Debug.Log("ccc: " + ev.Series + " " + _seriesCallbacks.Count);
                if (ev.Series > 0) {
                    Action<SessionEvent> cb;
                    if (_seriesCallbacks.TryGetValue(ev.Series, out cb)) {
                        _seriesCallbacks.Remove(ev.Series);
                        cb.Invoke(ev);
                    }
                }
                if (!_msgCallbacks.TryGetValue(ev.ID, out callbacks))
                {
                    return;
                }

                callbacks.Invoke(msg);
            }
            else if ( msg is Action )
            {
                var call = (Action)msg;
                call();
            }


        
        }



        public bool Running
        {
            get
            {
                lock (_runningGuard)
                {
                    return _running;
                }
            }

            set
            {
                lock (_runningGuard)
                {
                    _running = value;
                }
            }
        }

        public void Start( )
        {
            Running = true;
            _thread = new Thread(EventLoop);
            _thread.Name = "EventLoop";
            _thread.IsBackground = true;
            _thread.Start();
        }

        public void Stop( )
        {
            Running = false;
        }


        public void Wait( )
        {
            _exitSignal.WaitOne();
        }

        void EventLoop()
        {
            while( Running )
            {
                _queue.WaitEvent();

                _queue.Polling();                
            }

            _exitSignal.Set();
        }
    }

}
