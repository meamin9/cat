using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game {
    /**unity**/
    public static class RandomExt {
        public static void Init() {
            UnityEngine.Random.InitState((int)System.DateTime.Now.ToFileTime());
        }
    }
}
