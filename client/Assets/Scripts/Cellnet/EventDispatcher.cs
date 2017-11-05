using System;
using System.Collections.Generic;
using System.Threading;

namespace Cellnet
{
    public struct SeriesCallback {
        public UInt16 Series;
        public Action<SessionEvent> Cb;
		public SeriesCallback(UInt16 sid, Action<SessionEvent> cb) {
			Series = sid;
			Cb = cb;
		}
    }
    public class EventDispatcher
    {
        Dictionary<uint, Action<object>> _msgCallbacks = new Dictionary<uint, Action<object>>();
        Queue<SeriesCallback> _seriesCallbacks = new Queue<SeriesCallback>();

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

		public void AddSid(UInt16 sid, Action<SessionEvent> cb) {
			if (sid == 0) {
				UnityEngine.Debug.LogError("sid cannot be 0");
				return;
			}
			_seriesCallbacks.Enqueue(new SeriesCallback(sid, cb));
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
                if (ev.Series > 0 && _seriesCallbacks.Count > 0) {
                    var handle = _seriesCallbacks.Peek();
					UnityEngine.Debug.Log("series:" + handle.Series + " " + ev.Series);
                    if (handle.Series == ev.Series) {
						_seriesCallbacks.Dequeue();
                        handle.Cb.Invoke(ev);
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
