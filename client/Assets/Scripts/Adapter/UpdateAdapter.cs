using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Base
{
    public class UpdateAdapter : MonoBehaviour
    {
        public Action update;
        private void Update()
        {
            update?.Invoke();
        }
    }
}
