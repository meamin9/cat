using System;
using System.Collections.Concurrent;

class MailBox<T>
{
    private ConcurrentQueue<T> _queue = new ConcurrentQueue<T>();

    void Post(T mail) {
        _queue.Enqueue(mail);
    }

    void Poll(Action<T> action) {
        // 先复制出来，再action，避免每次dequeue时加锁解锁
        T mail;
        while (_queue.TryDequeue(out mail)) {
            action(mail);
        }
    }
}
