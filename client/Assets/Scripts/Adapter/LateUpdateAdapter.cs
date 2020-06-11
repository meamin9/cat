using System;

using UnityEngine;

namespace Base
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
