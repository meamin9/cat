using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

class NoticeOP {
    public static void RegisterProto(string content) {
        Net.Instance.RegisterProto<proto.SCNotice>((msg, ses) => {
            ShowText(Lang.Instance.Notice((Lang.Key)(msg.Index)));
        });
    }

    // 显示一条弹窗文字提示
    public static void ShowText(string content) {
    }
}