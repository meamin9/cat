using System;

namespace Util
{
    class Singleton<T> where T : class, new()
    {
        static T _instance;
        T Instance {
            get {
                if (_instance == null) {
                    _instance = new T();
                }
                return _instance;
            }
        }
        void Destroy() {
            _instance = null;
        }
    }
}
