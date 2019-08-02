using UnityEngine;
using UnityEngine.AI;

namespace AM.Game
{
    public class MoveController
    {
        public NavMeshAgent NavAgent { get; set; }
        public AnimController AnimCtrl { get; set; }

        public Transform Target { get; set; }

        public float Speed {
            get { return NavAgent.speed; }
            set { NavAgent.speed = value; }
        }

        public Vector3 _steeringPos;
        private Quaternion _steeringDir;

        public void Init(NavMeshAgent agent, AnimController anim, Transform target)
        {
            NavAgent = agent;
            AnimCtrl = anim;
            Target = target;

            NavAgent.updatePosition = false;
            NavAgent.updateRotation = false;
        }

        public void Move(Vector3 offset)
        {
            NavAgent.Move(offset);
        }

        public void MoveTo(Vector3 pos)
        {
            NavAgent.destination = pos;
        }


        public void Update()
        {
            Vector3 worldDeltaPosition = NavAgent.nextPosition - Target.position;

            if (worldDeltaPosition.sqrMagnitude > 0f) {
                AnimCtrl.Run();
            }
            else {
                AnimCtrl.Stand();
            }
        }




        public void OnAnimatorMove()
        {
            // 转向
            if (_steeringPos != NavAgent.steeringTarget) {
                _steeringPos = NavAgent.steeringTarget;
                _steeringDir = Quaternion.LookRotation(_steeringPos - Target.position);
            }
            if (Target.position == NavAgent.nextPosition) {
                Target.rotation = _steeringDir;
            }
            else if (_steeringDir != Target.rotation) {
                Target.rotation = Quaternion.Slerp(Target.rotation, _steeringDir, 0.15f);
            }
            // 移动
            Target.position = NavAgent.nextPosition;
        }
    }

}
