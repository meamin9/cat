using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

class Error
{
    public string Info = "";
    public StackTrace Trace;
    public Error(string err) {
        Info = err;
        Trace = new StackTrace(1, true);
    }

    public override string ToString() {
        var build = new StringBuilder();
        build.AppendFormat("Error Found:{0}\n", Info);
        for (var i = 0; i < Trace.FrameCount; ++i) {
            var sf = Trace.GetFrame(i);
            build.AppendFormat("\t...{0}.{1}:{2}", sf.GetFileName(), sf.GetMethod(),
                sf.GetFileLineNumber());
        }
        return build.ToString();
    }
}

class Log {
    public static void Errorf(string fmt, params object[] args) {
    }
    public static void Error(params object[] args) {
    }

    public static void Error(Error err) {

    }
}

