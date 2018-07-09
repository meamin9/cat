using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

class Singleton<T> where T: class, new()
{
    static T _instance;
    T Instance
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
    void Destroy()
    {
        _instance = null;
    }
}

interface IService
{
    void Init();
    void Start();
    void Stop();
}


class ServiceMgr
{
    List<IService> _serviceList = new List<IService>();

    void VisitService(Action<IService> func)
    {
        foreach (var s in _serviceList) {
            func(s);
        }
    }

    void RegService(IService s)
    {
        _serviceList.Add(s);
    }

}
