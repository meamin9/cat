using UnityEngine;
using UnityEngine.SceneManagement;

namespace Automata.Base
{
    public class Singleton<T> where T : class, new()
    {
        static T _instance;
        public static T Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new T();
                }
                return _instance;
            }
        }
        public static void ReleaseInstance()
        {
            _instance = null;
        }
    }


}
