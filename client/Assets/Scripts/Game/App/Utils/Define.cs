using System;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Automata.Game
{



    interface IService
    {
        void Init();
        void RoleInit();
        void RoleUnint();
        void Uninit();
    }

    public class SingleService<T> : IService where T : class, new()
    {
        public virtual void Init() { }
        public virtual void RoleInit() { }
        public virtual void RoleUnint() { }
        public virtual void Uninit() {
            ReleaseInstance();
        }

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


}


