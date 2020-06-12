using UnityEngine;
using UnityEngine.AI;

namespace Game
{
    public class MoveController
    {
        private NavMeshAgent mNavAgent;
        private Transform mTarget;

        //朝向缓动
        private Vector3 mSteeringPos;
        private Quaternion mSteeringDir;
        private Quaternion mSteeringBeginDir;
        private const float cDirChangeTime = 0.3f;
        private float mDirChangeTime;
        //

        private bool mIsMoving;

        public MoveController(Transform target) {
            mNavAgent = target.GetComponent<NavMeshAgent>();
            if (mNavAgent == null) {
                mNavAgent = target.gameObject.AddComponent<NavMeshAgent>();
                Log.Info($"Add NavMeshAgent To {target.name}");
            }
            mTarget = target;
            mNavAgent.updatePosition = false;
            mNavAgent.updateRotation = false;
        }

        public bool IsMoving {
            get => mIsMoving;
            set {
                mIsMoving = value;
                if (value) {
                    Log.Info("IsMoving");
                } else {
                    Log.Info("Stop Moving");
                }
            }
        }
        public float Speed {
            get { return mNavAgent.speed; }
            set { mNavAgent.speed = value; }
        }

        public void Move(Vector3 offset)
        {
            mNavAgent.Move(offset);
            IsMoving = true;
        }

        public void MoveTo(Vector3 pos)
        {
            mNavAgent.destination = pos;
            IsMoving = true;
        }

        public void StopMove() {
            mNavAgent.ResetPath();
            IsMoving = false;
        }

        public void Update()
        {
            if (!IsMoving) {
                return;
            }
            if (mTarget.position == mNavAgent.nextPosition) {
                mTarget.rotation = mSteeringDir;
                IsMoving = false;
                return;
            }
            // 转向
            if (mSteeringPos != mNavAgent.steeringTarget) {
                mSteeringPos = mNavAgent.steeringTarget;
                mSteeringDir = Quaternion.LookRotation(mSteeringPos - mTarget.position);
                mSteeringBeginDir = mTarget.rotation;
                mDirChangeTime = 0f;
            }
            if (mDirChangeTime < cDirChangeTime) {
                mDirChangeTime += Time.deltaTime;
                var r = Mathf.Clamp01(mDirChangeTime / cDirChangeTime);
                mTarget.rotation = Quaternion.Slerp(mSteeringBeginDir, mSteeringDir, r);
            }
            mTarget.position = mNavAgent.nextPosition;
        }
    }

}
