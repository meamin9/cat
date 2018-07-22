using System;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace NSUnityUtil
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

    public class SingleMonoBehaviour<T> : MonoBehaviour where T : class
    {
        static T _instance;
        public static T Instance {
            get {
                if (_instance == null) {
                    var typ = typeof(T);
                    var obj = new GameObject(typ.Name);
                    _instance = obj.AddComponent(typ) as T;
                    //SceneManager.CreateScene("#dddd");
                    DontDestroyScene.Move(obj);
                }
                return _instance;
            }
        }
        public static void ReleaseInstance() {
            _instance = null;
        }
    }

    public class DontDestroyScene {
        private static System.Nullable<Scene> _dontDestroyScene = null;
        public static Scene SceneInstance {
            get {
                if (!_dontDestroyScene.HasValue) {
                    _dontDestroyScene = SceneManager.CreateScene("#DontDestroyScene");
                }
                return _dontDestroyScene.Value;
            }
        }
        public static void Move(GameObject obj) {
            SceneManager.MoveGameObjectToScene(obj, SceneInstance);
        }
        
    }
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


