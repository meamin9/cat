using System;
using System.Collections.Generic;
using System.Linq;
using Automata.Base;
using Automata.Adapter;
using System.Threading.Tasks;

namespace Automata.Game
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
