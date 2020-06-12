using System;

namespace Game
{
    public class Skill
    {
        public int id;
        public SkillTable table;
        public int level;
        public float cdOvertTime;

        public bool IsCd() {
            return GameTime.time < cdOverTime;
        }
    }

}