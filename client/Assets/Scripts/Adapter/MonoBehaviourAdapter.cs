using UnityEngine;
using System;

namespace Automata.Adapter
{



    public class MonoBehaviourAdapter : MonoBehaviour
    {
        public event Action awake;
        private void Awake()
        {
            awake?.Invoke();
        }

        public event Action start;
        private void Start()
        {
            start?.Invoke();
        }

        public event Action onEnable;
        private void OnEnable()
        {
            onEnable?.Invoke();
        }

        public event Action onDisable;
        private void OnDisable()
        {
            onDisable?.Invoke();
        }

        public event Action update;
        private void Update()
        {
            update?.Invoke();
        }

        public event Action fixedUpdate;
        private void FixedUpdate()
        {
            fixedUpdate?.Invoke();
        }

        public event Action lateUpdate;
        private void LateUpdate()
        {
            lateUpdate?.Invoke();
        }

        public event Action onDestory;
        private void OnDestroy()
        {
            onDestory?.Invoke();
        }
    }

}
