using System.IO;

namespace AM.Base
{
    /*
     * assetbundle结构
     */
    [System.Serializable]
    public class BundleEntry
    {
        public int Version;
        public long Size;
        public string[] Assets;
    }

    [System.Serializable]
    public class BundleInfo
    {
        public int VersionCode { get; private set; }
        public long Bytes { get; private set; }
        public bool InApp { get; set; }

        public string Name { get; private set; }

        public string[] Assets { get; private set; }

        public BundleInfo(string name, BundleEntry conf, bool isPatch)
        {
            Name = name;
            VersionCode = conf.Version;
            Bytes = conf.Size;
            Assets = conf.Assets;
            InApp = !isPatch;
        }

        public BundleInfo(string name, int verCode, int bytes)
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
                return InApp ? Path.Combine(AssetMgr.DefaultDir, Name) 
                    : Path.Combine(AssetMgr.PatchDir, Name + VersionCode);
            }
        }

        public override string ToString()
        {
            return string.Format("{0}|{1}|{2}", Name, VersionCode, InApp);
        }
    }
}
