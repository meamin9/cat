using System;
using System.Collections.Generic;
using System.Threading;

class MailBox<T>
{
    private List<T> _queue = new List<T>();

    public void Post(T mail) {
        Monitor.Enter(_queue);
        try {
            _queue.Add(mail);
            Monitor.Pulse(_queue);
        }
        finally {
            Monitor.Exit(_queue);
        }
    }

    public void Peek(ref List<T> list) {
        Monitor.Enter(_queue);
        try {
            if (_queue.Count == 0) {
                Monitor.Wait(_queue);
            }
            list.AddRange(_queue);
            _queue.Clear();
        }
        finally {
            Monitor.Exit(_queue);
        }
        
    }

    public void Poll(Action<T> action) {
        // 先复制出来，再action，避免每次dequeue时加锁解锁
        //T mail;
        //while (_queue.TryDequeue(out mail)) {
        //    action(mail);
        //}
    }
}
