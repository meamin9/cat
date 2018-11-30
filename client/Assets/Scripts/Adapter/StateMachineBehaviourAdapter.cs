using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Automata.Adapter
{
    public class StateMachineBehaviourAdapter : StateMachineBehaviour
    {
        public string Name;
        [HideInInspector]
        public int Id { get; private set; }

        public StateMachineBehaviourAdapter()
        {
            Id = Animator.StringToHash(Name);
        }

        public event System.Action<Animator, AnimatorStateInfo, int> onStateEnter;
        // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            onStateEnter?.Invoke(animator, stateInfo, layerIndex);
        }

        public event System.Action<Animator, AnimatorStateInfo, int> onStateUpdate;
        // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
        override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            onStateUpdate?.Invoke(animator, stateInfo, layerIndex);
        }


        public event System.Action<Animator, AnimatorStateInfo, int> onStateExit;
        // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
        override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            onStateExit?.Invoke(animator, stateInfo, layerIndex);
        }

        public event System.Action<Animator, AnimatorStateInfo, int> onStateMove;
        // OnStateMove is called right after Animator.OnAnimatorMove(). Code that processes and affects root motion should be implemented here
        override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            onStateMove?.Invoke(animator, stateInfo, layerIndex);
        }

        public event System.Action<Animator, AnimatorStateInfo, int> onStateIK;
        // OnStateIK is called right after Animator.OnAnimatorIK(). Code that sets up animation IK (inverse kinematics) should be implemented here.
        override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            onStateIK?.Invoke(animator, stateInfo, layerIndex);
        }
    }

}

