using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Automata.Base
{
    [System.Serializable]
    public class BundleConf
    {
        public int Version;
        public int Size;
        public string[] Assets;
    }

    [System.Serializable]
    public class BundleEntry
    {
        public int VersionCode { get; private set; }
        public int Bytes { get; private set; }
        public bool InApp { get; set; }

        public string Name { get; private set; }

        public string[] Assets { get; private set; }

        public BundleEntry(string name, BundleConf conf, bool isPatch)
        {
            Name = name;
            VersionCode = conf.Version;
            Bytes = conf.Size;
            Assets = conf.Assets;
            InApp = isPatch;
        }

        public BundleEntry(string name, int verCode, int bytes)
        {
            //Name = name;
            VersionCode = verCode;
            Bytes = bytes;
            InApp = false;
        }

        public string Url
        {
            get
            {
                return InApp ? Path.Combine(Application.streamingAssetsPath, Name) 
                    : Path.Combine(AssetMgr.PatchDir, Name + VersionCode);
            }
        }
        public string AppUrl
        {
            get { return Path.Combine(Application.streamingAssetsPath, Name); }
        }

        public string WWWUrl
        {
            get { return string.Format("{0}/{1}", VersionCode, Name); }
        }

        public override string ToString()
        {
            return string.Format("{0}|{1}|{2}", Name, VersionCode, InApp);
        }
    }
}
