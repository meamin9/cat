using System;

using UnityEngine;

namespace AM.Base
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
