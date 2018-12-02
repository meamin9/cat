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
        public Animator _anim;

        protected MonoBehaviourAdapter _adapter;

        public void OnAttach()
        {
            _navAgent = gameObject.AddComponent<NavMeshAgent>();
            _navAgent.updatePosition = false;

            _adapter = gameObject.AddComponent<MonoBehaviourAdapter>();
            _adapter.update += Update;
            _adapter.onAnimatorMove += OnAnimatorMove;

            _anim = gameObject.GetComponent<Animator>();
        }

        void OnAnimatorMove()
        {
            var vec3 = _navAgent.nextPosition - gameObject.transform.position;
            gameObject.transform.LookAt(_navAgent.nextPosition);
            gameObject.transform.position = _navAgent.nextPosition;
        }

        protected void Update()
        {
            //if (Input.GetMouseButtonDown(0))
            //{
            //    RaycastHit hitInfo;
            //    Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            //    if (Physics.Raycast(ray.origin, ray.direction, out hitInfo))
            //        _navAgent.destination = hitInfo.point;
            //}

            Vector3 worldDeltaPosition = _navAgent.nextPosition - gameObject.transform.position;

            // Map 'worldDeltaPosition' to local space
            float dx = Vector3.Dot(gameObject.transform.right, worldDeltaPosition);
            float dy = Vector3.Dot(gameObject.transform.up, worldDeltaPosition);
            float dz = Vector3.Dot(gameObject.transform.forward, worldDeltaPosition);
            var deltaPosition = new Vector3(dx, dy, dz);

            //// Low-pass filter the deltaMove
            //float smooth = Mathf.Min(1.0f, Time.deltaTime / 0.15f);
            //_smoothDeltaPosition = Vector3.Lerp(_smoothDeltaPosition, deltaPosition, smooth);

            //// Update velocity if time advances
            //var velocity = Vector3.zero;
            //if (Time.deltaTime > 1e-5f)
            //    velocity = _smoothDeltaPosition / Time.deltaTime;

            //bool shouldMove = _navAgent.remainingDistance > _navAgent.radius;

            var spd = _moveSpeed;
            if (_anim.IsInTransition(0))
            {
                var trans = _anim.GetAnimatorTransitionInfo(0);
                if (trans.fullPathHash == AnimNameHash.Idle2Run)
                {
                    spd = Mathf.Lerp(0, _moveSpeed, trans.normalizedTime);
                }
                else if (trans.fullPathHash == AnimNameHash.Run2Idle)
                {
                    spd = Mathf.Lerp(_moveSpeed, 0, trans.normalizedTime);
                }
            }
            _navAgent.speed = spd;


            if (deltaPosition.magnitude < 1e-5f)
            {
                SetTransId(AnimTransId.Idle);
            }
            else
            {
                SetTransId(AnimTransId.Run);
            }



        }

        public void Play(int id)
        {
            var nameHash = _anim.GetCurrentAnimatorStateInfo(0).fullPathHash;
            if (id == nameHash)
            {
                return;
            }

        }

        public int TransId { get; private set; }
        public void SetTransId(int id)
        {
            if (TransId == id)
            {
                return;
            }
            TransId = id;
            _anim.SetInteger("EAvatarAnim", id);
        }

        private float _speed = 0;
        private float _moveSpeed = 1f;
        public void CheckSpeed()
        {
            var spd = _speed;
            if (_anim.IsInTransition(0))
            {
                var trans = _anim.GetAnimatorTransitionInfo(0);
                if (trans.fullPathHash == AnimNameHash.Idle2Run)
                {
                    spd = Mathf.Lerp(0, _speed, trans.normalizedTime);
                }
                else if (trans.fullPathHash == AnimNameHash.Run2Idle)
                {
                    spd = Mathf.Lerp(_speed, 0, trans.normalizedTime);
                }
            }
            _navAgent.speed = spd;
        }

        public void Move(Vector3 offset)
        {
            _navAgent.Move(offset);
        }

        public void MoveTo(Vector3 pos)
        {
            _navAgent.destination = pos;
        }


    }
}
