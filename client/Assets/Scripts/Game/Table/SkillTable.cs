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
    public class SkillTable
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
