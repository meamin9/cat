using System.Text;
using System.Diagnostics;
using UnityEngine;

/// <summary>
/// Base提供patch和game的共有支持
/// </summary>
namespace Automata.Base
{
    public class Error
    {
        public string Info = "";
        public StackTrace Trace;
        public Error(string err)
        {
            Info = err;
            Trace = new StackTrace(1, true);
        }

        public override string ToString()
        {
            var build = new StringBuilder();
            build.AppendFormat("Error Found:{0}\n", Info);
            for (var i = 0; i < Trace.FrameCount; ++i)
            {
                var sf = Trace.GetFrame(i);
                build.AppendFormat("\t...{0}.{1}:{2}", sf.GetFileName(), sf.GetMethod(),
                    sf.GetFileLineNumber());
            }
            return build.ToString();
        }
    }

    public class Log
    {

        public static void Info(params object[] args)
        {
            UnityEngine.Debug.Log(args);
        }

        public static void Infof(string fmt, params object[] args)
        {
            UnityEngine.Debug.LogFormat(fmt, args);
        }

        public static void Warn(params object[] args)
        {
            UnityEngine.Debug.Log(args);
        }

        public static void Warnf(string fmt, params object[] args)
        {
            UnityEngine.Debug.LogFormat(fmt, args);
        }

        public static void Errorf(string fmt, params object[] args)
        {
            UnityEngine.Debug.LogErrorFormat(fmt, args);
        }

        public static void Error(params object[] args)
        {
            UnityEngine.Debug.LogError(args);
        }

        public static void Error(Error err)
        {

        }

        public static void Fatalf(string fmt, params object[] args)
        {
            Errorf(fmt, args);

        }
        public static void Fatal(params object[] args)
        {
            Error(args);
        }

        public static void FatalAsset(bool assert, string fmt, params object[] args)
        {
            if (!assert)
            {
                Fatalf(fmt, args);
            }
        }

        public static void FatalAsset(bool assert, params object[] args)
        {
            if (!assert)
            {
                Fatal(args);
            }
        }
    }
}


