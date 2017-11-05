using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

// 
class Lang {
    public enum Key {
        AccountNameLengthInvalid, // 帐号名长度不合法
        AccountPwdLengthInvalid, // 帐号密码长度不合法
    }

    static readonly Lang _instance = new Lang();
    public static Lang Instance { get { return _instance; } }

    // 改成数组实现
    Dictionary<Key, string> _lang;

    Lang() {
        ChangeLang();
    }

    public void ChangeLang() {
        _lang = new Dictionary<Key, string>();
    }

    public string Notice(Key key) {
        string s = "默认文字提示";
        _lang.TryGetValue(key, out s);
        return s;
    }
}