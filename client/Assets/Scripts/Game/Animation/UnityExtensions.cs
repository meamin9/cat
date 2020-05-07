using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.Playables;

public static class UnityExtensions {

    public static void DestoryAllInput(this Playable owner) {
        var count = owner.GetInputCount();
        if (count == 0) {
            return;
        }
        for(var i = 0; i < count; ++i) {
            owner.GetInput(i).Destroy();
        }
        owner.SetInputCount(0);

    }
}
