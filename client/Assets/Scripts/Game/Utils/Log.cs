using System.Text;
using System.Diagnostics;


namespace Game
{
    //public class Error
    //{
    //    public string Info = "";
    //    public StackTrace Trace;
    //    public Error(string err)
    //    {
    //        Info = err;
    //        Trace = new StackTrace(1, true);
    //    }

    //    public override string ToString()
    //    {
    //        var build = new StringBuilder();
    //        build.AppendFormat("Error Found:{0}\n", Info);
    //        for (var i = 0; i < Trace.FrameCount; ++i)
    //        {
    //            var sf = Trace.GetFrame(i);
    //            build.AppendFormat("\t...{0}.{1}:{2}", sf.GetFileName(), sf.GetMethod(),
    //                sf.GetFileLineNumber());
    //        }
    //        return build.ToString();
    //    }
    //}

    public class Log
    {
        public static void Info(string msg)
        {
            UnityEngine.Debug.Log(msg);
        }

        public static void InfoIf(bool cond, string msg)
        {
            if (cond) {
                Info(msg);
            }
        }

        public static void Warn(string msg)
        {
            UnityEngine.Debug.Log(msg);
        }

        public static void WarnIf(bool cond, string msg)
        {
            if (cond) {
                Warn(msg);
            }
        }

        public static void Error(string msg)
        {
            UnityEngine.Debug.LogError(msg);
        }

        public static void ErrorIf(bool cond, string msg)
        {
            if (cond) {
                Error(msg);
            }
        }

        public static void Fatal(string msg)
        {
            Error(msg);
        }

        public static void FatalIf(bool cond, string msg)
        {
            if (cond)
            {
                Fatal(msg);
            }
        }
    }
}


