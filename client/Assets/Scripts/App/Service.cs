using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

class Singleton<T> where T: class, new()
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

interface IService
{
    void Init();
    void Start();
    void Stop();
}


class ServiceMgr : Singleton<ServiceMgr>, IService
{
    List<IService> _serviceList = new List<IService>();

    public void VisitService(Action<IService> func)
    {
        foreach (var s in _serviceList) {
            func(s);
        }
    }
    public void Init() {

    }

    public void Start() { }
    public void Stop() { }


}
