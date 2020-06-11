using System;
using System.Collections.Generic;
using System.Threading;

namespace Game {
    public class LockQueue<T> {
        private List<T> _queue = new List<T>();

        public void Post(T mail) {
            Monitor.Enter(_queue);
            try {
                _queue.Add(mail);
                Monitor.Pulse(_queue);
            } finally {
                Monitor.Exit(_queue);
            }
        }

        // 拿出所有信息，如果当前没有，会阻塞直到有
        public void Peek(ref List<T> list) {
            Monitor.Enter(_queue);
            try {
                while (_queue.Count == 0) {
                    Monitor.Wait(_queue);
                }
                list.AddRange(_queue);
                _queue.Clear();
            } finally {
                Monitor.Exit(_queue);
            }

        }

        // 拿出所有的信息，不阻塞调用线程
        public void Poll(ref List<T> list) {
            if (Monitor.TryEnter(_queue)) {
                try {
                    list.AddRange(_queue);
                    _queue.Clear();
                } finally {
                    Monitor.Exit(_queue);
                }
            }
        }
    }
}
