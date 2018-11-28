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

        public void MoveTo(Vector3 pos) { }
    }

    public class Player : Entity
    {
        //public static Player

    }

    public class Entity
    {
        public GameObject gameObject { get; set; }
        protected NavMeshAgent _navAgent;
        protected MonoBehaviourAdapter _adapter;

        public AnimController _animControl;

        Vector3 _smoothDeltaPosition = Vector2.zero;
        Vector2 _velocity = Vector2.zero;

        public void OnAttach()
        {
            _navAgent = gameObject.AddComponent<NavMeshAgent>();
            _navAgent.updatePosition = false;

            _adapter = gameObject.AddComponent<MonoBehaviourAdapter>();
            _adapter.update += Update;

            _animControl = new AnimController();
            _animControl.OnAttach(gameObject.GetComponent<Animator>());
            _animControl.Play(EAvatarAnim.Idle2);
        }

        void OnGUI()
        {
            //for (var i = EAvatarAnim.Idle; i)
            //{
            //    if (GUILayout.Button(animation.name))
            //    {
            //        anim.CrossFade(animation.name, 0);
            //    }
            //}
        }

        protected void Update()
        {
            Vector3 worldDeltaPosition = _navAgent.nextPosition - gameObject.transform.position;

            // Map 'worldDeltaPosition' to local space
            float dx = Vector3.Dot(gameObject.transform.right, worldDeltaPosition);
            float dy = Vector3.Dot(gameObject.transform.up, worldDeltaPosition);
            float dz = Vector3.Dot(gameObject.transform.forward, worldDeltaPosition);
            Vector2 deltaPosition = new Vector3(dx, dy, dz);

            // Low-pass filter the deltaMove
            float smooth = Mathf.Min(1.0f, Time.deltaTime / 0.15f);
            _smoothDeltaPosition = Vector3.Lerp(_smoothDeltaPosition, deltaPosition, smooth);

            // Update velocity if time advances
            var velocity = Vector3.zero;
            if (Time.deltaTime > 1e-5f)
                velocity = _smoothDeltaPosition / Time.deltaTime;

            bool shouldMove = velocity.sqrMagnitude > 0.25f && _navAgent.remainingDistance > _navAgent.radius;

            //if (Input.GetMouseButtonDown(0))
            //{
            //    RaycastHit hitInfo;
            //    Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            //    if (Physics.Raycast(ray.origin, ray.direction, out hitInfo))
            //        _navAgent.destination = hitInfo.point;
            //}

            if (!shouldMove)
            {
                //Debug.Log(_navAgent.hasPath);
                //Debug.Log(_navAgent.pathPending);
                //Debug.Log(_navAgent.pathStatus);
                //Debug.Log(_navAgent.pathStatus);
                //Debug.Log("-----");

                _animControl.Play(EAvatarAnim.Idle);
                //_animControl.Play(EAvatarAnim.Idle);
            }
            else
            {
                Debug.Log("runing");
            }

        }

        public void Move(Vector3 offset)
        {
            _navAgent.Move(offset);
            _animControl.Play(EAvatarAnim.Walk);
            _animControl._anim.CrossFade("WALK00_F", 0.2f);
        }

        public void MoveTo(Vector3 pos)
        {
            _navAgent.destination = pos;
        }


    }
}
