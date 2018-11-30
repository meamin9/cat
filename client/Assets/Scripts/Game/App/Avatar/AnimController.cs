using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Automata.Game
{
    public class AnimNameHash
    {
        public static int Run = Animator.StringToHash("Base Layer.Run");
        public static int Walk = Animator.StringToHash("Base Layer.Walk");
        public static int Jump = Animator.StringToHash("Base Layer.Jump");
        public static int Stand = Animator.StringToHash("Base Layer.Idle");
        public static int Idle1 = Animator.StringToHash("Base Layer.Idle1");
        public static int Idle2 = Animator.StringToHash("Base Layer.Idle2");
        public static int Idle3 = Animator.StringToHash("Base Layer.Idle3");
        public static int Idle4 = Animator.StringToHash("Base Layer.Idle4");

        public static int Idle2Run = Animator.StringToHash("Base Layer.Idle -> Base Layer.Run");
        public static int Run2Idle = Animator.StringToHash("Base Layer.Run -> Base Layer.Idle");

        //public static int NameHash(string )
    }
    public class AnimTransId
    {
        public const int Idle = 0;
        public const int Walk = 100;
        public const int Run = 200;
    }
    public enum EAnimTransId
    {
        Stand = 0,
        Idle1,
        Idle2,
        Idle3,
        Idle4,
        IdleEnd,
        Walk = 100,
        Run = 200,
        Jump = 300,
    }

    public enum ERoleAnimState
    {
        None,
        Idel,
        Walk,
        Run,
        Jump,

    }
    public class AnimController
    {
        public Animator _anim;
        public EAnimTransId TransId { get; private set; }
        public ERoleAnimState AnimState { get; private set; }

        //public float 
        public Timer _idleTimer;

        public void OnAttach(Animator anim)
        {
            _anim = anim;

        }
        //public int Current
        //{
        //    get { return _anim.GetAnimatorTransitionInfo.GetCurrentAnimatorStateInfo(0).shortNameHash; }
        //}

        public void SetTransId(EAnimTransId index)
        {
            if (TransId == index)
            {
                return;
            }
            TransId = index;
            _anim.SetInteger("EAvatarAnim", (int)TransId);
        }

        public bool SetAnimState(ERoleAnimState state)
        {
            if (AnimState == state)
            {
                return false;
            }
            AnimState = state;
            if (state != ERoleAnimState.Idel)
            {
                _idleTimer?.Cancel();
                _idleTimer = null;
            }
            else
            {
                _idleTimer = TimeMgr.Instance.Schduler(20, OnChangeIdle);
            }
            return true;
        }

        public void OnChangeIdle(Timer timer)
        {
            var id = Random.Range((int)EAnimTransId.Idle1, (int)EAnimTransId.IdleEnd);
            SetTransId((EAnimTransId)id);
            Debug.Log(timer);
        }

        public void Idle() {
            if (SetAnimState(ERoleAnimState.Idel))
            {
                SetTransId(EAnimTransId.Stand);
            }
        }

        public void Stand()
        {
            _anim.CrossFadeInFixedTime(AnimNameHash.Stand, 0.5f);
        }

        public void Walk()
        {
            if (AnimState == ERoleAnimState.Idel)
            {
                _anim.CrossFadeInFixedTime(AnimNameHash.Walk, 0.5f);
            }
            if (SetAnimState(ERoleAnimState.Walk))
            {
                SetTransId(EAnimTransId.Walk);
            }
        }
        public void Run() { }
        public void Jump() { }


    }
}
