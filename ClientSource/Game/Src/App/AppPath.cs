using System;
using System.Collections.Generic;

namespace Automata.Game
{
    public static class AppPath
    {
        public static string ViewPath(string name)
        {
            return string.Format("View/{0}.prefab", name);
        }
    }
}
