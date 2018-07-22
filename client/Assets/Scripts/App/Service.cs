using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NSUnityUtil;

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
