using Base;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Game
{
    public class tAct {
        public int id;
        public int type;
        public string anim;
        public string icon;

        public string name;
        /// <summary>
        /// [rangeType, range args]
        /// </summary>
        public int[] range;

        public int[] next;

        public float dmgRate;

    }

    public class tSkill
    {
        public int id;
        public string name;
        public string desc;
        public string animName;
        public float cd;
        /// <summary>
        /// [rangeType, range args]
        /// </summary>
        public int[] range;
        public float dmgRate;
    }

    public class BuffTable {
        public int id;
        public int time;
        //public int 
    }
}
