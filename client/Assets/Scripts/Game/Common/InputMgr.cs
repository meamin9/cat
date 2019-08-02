using System;
using System.Collections.Generic;
using System.Linq;
using AM.Base;
using System.Threading.Tasks;

namespace AM.Game
{
    public class InputMgr : Singleton<InputMgr>
    {
        public void Initialize()
        {
            MonoProxy.Instance.Adapter.update += Update;
        }

        public void Update()
        {

        }

        public void ShowJoystick()
        {

        }

    }
}
