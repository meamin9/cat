using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.AI;
using Base;

namespace Game
{

    public class Creature {
        public Avatar avatar;

    }

    public class Entity {
        private ActController mActCtrl;
        private AnimationController mAnimCtrl;

    }

    public class EntityObject
    {
        public GameObject gameObject { get; set; }
        //public MoveController MoveCtrl { get; set; }
        public AnimController AnimCtrl { get; set; }

        public string Name { get; set; }
        public string Id { get; set; }

        public Dictionary<string, object> Extra;

        //protected MonoBehaviourAdapter _adapter;

        public void OnAttach()
        {

            AnimCtrl = new AnimController();
            var anim = gameObject.GetComponent<Animator>();
            AnimCtrl.Init(anim);

            //MoveCtrl = new MoveController();
            //var navAgent = gameObject.AddComponent<NavMeshAgent>();
            //MoveCtrl.Init(navAgent, AnimCtrl, gameObject.transform);

            //var adapter = gameObject.AddComponent<MonoBehaviourAdapter>();
            //adapter.update += Update;
            //adapter.onAnimatorMove += MoveCtrl.OnAnimatorMove;
        }


        public void Update()
        {
            //MoveCtrl.Update();
            //if (Input.GetMouseButtonDown(0))
            //{
            //    RaycastHit hitInfo;
            //    Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            //    if (Physics.Raycast(ray.origin, ray.direction, out hitInfo))
            //        _navAgent.destination = hitInfo.point;
            //}
        }



        //private float _speed = 0;
        //private float _moveSpeed = 1f;
        //public void CheckSpeed()
        //{
        //    var spd = _speed;
        //    if (_anim.IsInTransition(0)) {
        //        var trans = _anim.GetAnimatorTransitionInfo(0);
        //        if (trans.fullPathHash == AnimNameHash.Idle2Run) {
        //            spd = Mathf.Lerp(0, _speed, trans.normalizedTime);
        //        }
        //        else if (trans.fullPathHash == AnimNameHash.Run2Idle) {
        //            spd = Mathf.Lerp(_speed, 0, trans.normalizedTime);
        //        }
        //    }
        //    _navAgent.speed = spd;
        //}

   

    }
}
