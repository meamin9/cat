using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;

namespace AM.Game
{
    #region 动画常量
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
        public const int Stand = 0;
        public const int Walk = 100;
        public const int Run = 200;
    }

    public class AnimLayer
    {
        public const int Default = 0;
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
    #endregion
    public class AnimController
    {

        PlayableGraph mGraph;
        PlayableOutput mOutput;

        AnimationClip[] mClips;
        

        public void Create(GameObject go) {
            var animator = go.GetComponent<Animator>();
            if (animator == null) {
                animator = go.AddComponent<Animator>();
            }
            mGraph = new PlayableGraph();
            mOutput = AnimationPlayableOutput.Create(mGraph, "Animation", animator);
        }

        public void PlayAnimation(AnimationClip clip) {
            var clipPlayable = AnimationClipPlayable.Create(mGraph, clip);
            mOutput.SetSourcePlayable(clipPlayable);
            mGraph.Play();
        }



        public Animator _anim;
        public int TransId { get; private set; }
        public ERoleAnimState AnimState { get; private set; }

        //public float 
        public Timer _idleTimer;

        public void Init(Animator anim)
        {
            _anim = anim;

        }

        public void SetTransId(int index)
        {
            if (TransId == index)
            {
                return;
            }
            TransId = index;
            _anim.SetInteger("EAvatarAnim", TransId);
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
                _idleTimer = Timer.Schduler(20, OnChangeIdle);
            }
            return true;
        }

        public void OnChangeIdle(Timer timer)
        {
            var id = Random.Range((int)EAnimTransId.Idle1, (int)EAnimTransId.IdleEnd);
            SetTransId(id);
            Debug.Log(timer);
        }

        public void Idle() {
            if (SetAnimState(ERoleAnimState.Idel))
            {
                //SetTransId(EAnimTransId.Stand);
            }
        }

        public void Stand()
        {
            SetTransId(AnimTransId.Stand);
            //_anim.CrossFadeInFixedTime(AnimNameHash.Stand, 0.5f);
        }

        public void Walk()
        {
            if (AnimState == ERoleAnimState.Idel)
            {
                _anim.CrossFadeInFixedTime(AnimNameHash.Walk, 0.5f);
            }
            if (SetAnimState(ERoleAnimState.Walk))
            {
                //SetTransId(EAnimTransId.Walk);
            }
        }
        public void Run() {
            SetTransId(AnimTransId.Run);
        }

        public void Jump() { }


    }
}
