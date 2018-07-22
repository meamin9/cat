using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NSUnityUtil;

public class AppConfig : Singleton<AppConfig>
{
    public bool _resMode;

    public void Initialize() {
        _resMode = true;
    }

    public bool IsResMode {
        get { return _resMode; }
    }

}
