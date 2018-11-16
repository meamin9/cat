using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.AI;
using Automata.Adapter;

namespace Automata.Game
{
    //public interface I

    public class Movement
    {
        protected NavMeshAgent _navAgent;
        public void Move(Vector3 offset)
        {
            _navAgent.Move(offset);
        }
    }

    public class Entity
    {
        public GameObject gameObject { get; set; }
        protected NavMeshAgent _navAgent;
        protected MonoBehaviourAdapter _adapter;
        public void OnAttach()
        {
            _navAgent = gameObject.AddComponent<NavMeshAgent>();
            _adapter = gameObject.AddComponent<MonoBehaviourAdapter>();
            _adapter.update += Update;
        }

        protected void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                RaycastHit hitInfo;
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray.origin, ray.direction, out hitInfo))
                    _navAgent.destination = hitInfo.point;
            }

        }

        public void MoveTo(Vector3 pos)
        {
            _navAgent.destination = pos;
        }


    }
}
