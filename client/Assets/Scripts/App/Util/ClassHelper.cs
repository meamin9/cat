using System;
using UnityEngine;

namespace Util
{
    public class Singleton<T> where T : class, new()
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
        public static void ReleaseInstance() {
            _instance = null;
        }
    }

    interface IService
    {
        void Init();
        void Start();
        void Stop();
    }

    public class SingleService<T> : IService where T : class, new()
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
        public static void ReleaseInstance() {
            _instance = null;
        }
    }


    public class SingleMonoBehaviour<T> : MonoBehaviour where T : class
    {
        static T _instance;
        public static T Instance {
            get {
                if (_instance == null) {
                    var typ = typeof(T);
                    var obj = new GameObject(typ.Name);
                    _instance = obj.AddComponent(typ) as T;
                }
                return _instance;
            }
        }
        public static void ReleaseInstance() {
            _instance = null;
        }
    }
}


