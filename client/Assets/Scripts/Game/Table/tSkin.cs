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
    /// <summary>
    /// 服装配置
    /// </summary>
    public class tSkin
    {
        public int id;
        public string name;
        /// <summary>
        /// 限制穿戴的角色id
        /// </summary>
        public int roleId;
        public string skeleton;
        public string skin;
        public string animDir;
    }
}
