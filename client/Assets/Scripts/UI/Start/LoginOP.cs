using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

class LoginOP {
    #region proto
    public static void RegisterProto() {
        Net.Instance.RegisterProto<proto.SCRoleList>(scRoleList);
    }

    static void scRoleList(proto.SCRoleList roles, Cellnet.Session ses) {

    }
    #endregion
    static readonly int minKeyLength = 6;
    static readonly int maxNameLength = 32;
    static readonly int maxPwdLength = 32;
    public static bool CheckAccountKeyValid(string name, string pwd) {
        var len = name.Length;
        if (len < minKeyLength || len > maxNameLength) {
            NoticeOP.ShowText(Lang.Instance.Notice(Lang.Key.AccountNameLengthInvalid));
            return false;
        }
        len = pwd.Length;
        if (len < minKeyLength || len > maxPwdLength) {
            NoticeOP.ShowText(Lang.Instance.Notice(Lang.Key.AccountPwdLengthInvalid));
            return false;
        }
        return true;
    }

    public static void AccountLogin(string name, string pwd) {
        CheckAccountKeyValid(name, pwd);
        var msg = new proto.CSAccountLogin();
        msg.Id = name;
        msg.Pwd = StringOP.ComputeMD5(pwd);
        Net.Instance.Send<proto.CSAccountLogin>(msg);
    }

    public static void AccountCreate(string name, string pwd) {
        CheckAccountKeyValid(name, pwd);
        var msg = new proto.CSAccountCreate();
        msg.Id = name;
        msg.Pwd = StringOP.ComputeMD5(pwd);
        Net.Instance.Send<proto.CSAccountCreate>(msg);
    }
}