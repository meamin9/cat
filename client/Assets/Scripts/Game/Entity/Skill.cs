using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class Skill
    {
        public int id;
        public tSkill table;
        public int level;
        public float cdOverTime;

        public bool IsCd() {
            return GameTime.time < cdOverTime;
        }

        public void Play() {
            cdOverTime += table.cd;

        }
    }
    

}