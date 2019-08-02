using Log = UnityEngine.Debug;

namespace AM.Base
{
    public struct AppVersion
    {
        public int Main { get; private set; }
        public int Sub { get; private set; }
        public int Fix { get; private set; }
        public string Pre { get; private set; }
        public override string ToString()
        {
            var name = string.Format("{0}.{1}.{2}", Main, Sub, Fix);
            if (!string.IsNullOrEmpty(Pre))
            {
                name += "-" + Pre;
            }
            return name;
        }
        public AppVersion(string versionStr)
        {
            var strs = versionStr.Trim().Split('.');
            if (strs.Length != 3)
            {
                Log.LogErrorFormat("Invalid version {0}", versionStr);
            }
            Main = int.Parse(strs[0]);
            Sub = int.Parse(strs[1]);

            var index = strs[2].IndexOf('-');
            if (index == -1)
            {
                Fix = int.Parse(strs[2]);
                Pre = null;
            }
            else
            {
                Fix = int.Parse(strs[2].Substring(0, index));
                Pre = strs[2].Substring(index);
            }
        }
        public static AppVersion Parse(string versionStr)
        {
            return new AppVersion(versionStr);
        }

        public static bool operator <(AppVersion lhs, AppVersion rhs)
        {
            return lhs.Main < rhs.Main ||
                (lhs.Main == rhs.Main && (lhs.Sub < rhs.Sub ||
                lhs.Sub == rhs.Sub && (lhs.Fix < rhs.Fix)));
        }

        public static bool operator >(AppVersion lhs, AppVersion rhs)
        {
            return lhs.Main > rhs.Main ||
                (lhs.Main == rhs.Main && (lhs.Sub > rhs.Sub ||
                lhs.Sub == rhs.Sub && (lhs.Fix > rhs.Fix)));
        }
    }

}
