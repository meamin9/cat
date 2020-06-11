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

        public static void Info(params object[] args)
        {
            UnityEngine.Debug.Log(args);
        }

        public static void Info(string fmt, params object[] args)
        {
            UnityEngine.Debug.LogFormat(fmt, args);
        }

        public static void InfoIf(bool cond, params object[] args)
        {
            if (cond) {
                Info(args);
            }
        }

        public static void Warn(params object[] args)
        {
            UnityEngine.Debug.Log(args);
        }

        public static void Warn(string fmt, params object[] args)
        {
            UnityEngine.Debug.LogFormat(fmt, args);
        }

        public static void WarnIf(bool cond, params object[] args)
        {
            if (cond) {
                Warn(args);
            }
        }

        public static void Error(string fmt, params object[] args)
        {
            UnityEngine.Debug.LogErrorFormat(fmt, args);
        }

        public static void Error(params object[] args)
        {
            UnityEngine.Debug.LogError(args);
        }

        public static void ErrorIf(bool cond, params object[] args)
        {
            if (cond) {
                Error(args);
            }
        }

        public static void Fatal(string fmt, params object[] args)
        {
            Error(fmt, args);

        }
        public static void Fatal(params object[] args)
        {
            Error(args);
        }

        public static void FatalIf(bool cond, params object[] args)
        {
            if (cond)
            {
                Fatal(args);
            }
        }
    }
}


