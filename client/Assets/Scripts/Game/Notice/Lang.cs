using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

// 
class Lang {

    static readonly Lang _instance = new Lang();
    public static Lang Instance { get { return _instance; } }

    // 改成数组实现
    Dictionary<NoticeKey, string> _lang;

    Lang() {
        ChangeLang();
    }

    public void ChangeLang() {
        _lang = new Dictionary<NoticeKey, string>();
    }

    public string Notice(NoticeKey key) {
        string s = "默认文字提示";
        _lang.TryGetValue(key, out s);
        return s;
    }
}