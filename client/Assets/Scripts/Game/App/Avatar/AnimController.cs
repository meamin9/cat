using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Automata.Game
{
    public enum EAvatarAnim
    {
        Idle = 0,
        Idle1,
        Idle2,
        Idle3,
        Idle4,
        Walk = 100,
        Run = 200,
        Jump = 300,
    }

    public enum EFaceAnim
    {

    }
    public class AnimController
    {
        public Animator _anim;

        public void OnAttach(Animator anim)
        {
            _anim = anim;

        }

        public void Play(EAvatarAnim index)
        {
            _anim.SetInteger(nameof(EAvatarAnim), (int)index);
        }


    }
}
