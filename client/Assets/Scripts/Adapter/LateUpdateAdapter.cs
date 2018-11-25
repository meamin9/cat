using System;

using UnityEngine;

namespace Automata.Adapter
{
    public class LateUpdateAdapter : MonoBehaviour
    {
        public event Action lateUpdate;
        private void LateUpdate()
        {
            lateUpdate?.Invoke();
        }
    }
}
