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
    public class tRole {
        public int id;
        public string name;
        public int[] skinIds;

        public int[] actIds;
        public string animWalk;

        //public int[][] actCombos;


        //private SkinTable[] mSkins;
        //public SkinTable[] skins {
        //    get {
        //        if (mSkins == null) {
        //            mSkins = new SkinTable[skinIds.Length];
        //            for(var i = 0; i < skinIds.Length; ++i) {
        //                mSkins[i] = Table<SkinTable>.Find(skinIds[i]);
        //            }
        //        }
        //        return mSkins;
        //    }
        //}

    }
}
