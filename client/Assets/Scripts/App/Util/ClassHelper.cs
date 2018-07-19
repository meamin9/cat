using System;

namespace Util
{
    class Singleton<T> where T : class, new()
    {
        static T _instance;
        public static T Instance {
            get {
                if (_instance == null) {
                    _instance = new T();
                }
                return _instance;
            }
        }
        public static void Destroy() {
            _instance = null;
        }
    }

    interface IService
    {
        void Init();
        void Start();
        void Stop();
    }

    class SingleService<T> : IService where T : class, new()
    {
        public virtual void Init() { }
        public virtual void Start() { }
        public virtual void Stop() { }

        static T _instance;
        public static T Instance {
            get {
                if (_instance == null) {
                    _instance = new T();
                }
                return _instance;
            }
        }
        public static void Destroy() {
            _instance = null;
        }
    }
}


