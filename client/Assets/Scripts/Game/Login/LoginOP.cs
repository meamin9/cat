using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

static class LoginOP {
    #region proto
    public static void RegisterProto() {
        Net.Instance.RegisterProto<proto.SCRoleList>(scRoleList);
        Net.Instance.RegisterProto<proto.SCRoleCreate>(scRoleCreate);
    }

    static void scRoleList(proto.SCRoleList msg, Cellnet.Session ses) {
        var roles = msg.Roles;
        LoginM.Instance.SetRoleBaseList(roles);
        //test
        if (roles.Count == 0) {
            CreateRole("Test01", JobType.None, GenderType.Male);
            return;
        }
        var role = roles[0];
        LoginRole(role.Id);
    }

    static void scRoleCreate(proto.SCRoleCreate msg, Cellnet.Session ses) {
        var role = msg.Role;
        LoginM.Instance.AddRoleBase(role);
        // test
        LoginRole(role.Id);
    }
    #endregion

    #region 帐号创建
    public static bool CheckAccountKeyValid(string name, string pwd) {
        var len = name.Length;
        if (len < data.Constants.MinAccountLength || len > data.Constants.MaxAccountLength) {
			NoticeOP.ShowText(Lang.Instance.Notice(NoticeKey.NoticeLoginWrongNameLength));
            return false;
        }
        len = pwd.Length;
        if (len < data.Constants.MinAccountLength || len > data.Constants.MaxAccountLength) {
			NoticeOP.ShowText(Lang.Instance.Notice(NoticeKey.NoticeLoginWrongPwdLength));
            return false;
        }
        return true;
    }

    // AccountLogin 如果成功返回proto.SCRoleList
    public static void AccountLogin(string name, string pwd) {
        CheckAccountKeyValid(name, pwd);
        var msg = new proto.CSAccountLogin();
        msg.Id = name;
        msg.Pwd = StringOP.ComputeMD5(pwd);
        Net.Instance.Send<proto.CSAccountLogin>(msg); 
    }

    // AccountCreate 如果成功返回proto.SCRoleList
    public static void AccountCreate(string name, string pwd) {
        CheckAccountKeyValid(name, pwd);
        var msg = new proto.CSAccountCreate();
        msg.Id = name;
        msg.Pwd = StringOP.ComputeMD5(pwd);
        Net.Instance.Send<proto.CSAccountCreate>(msg); 
    }
    #endregion

    public static void CreateRole(string name, JobType job, GenderType gender) {
        name = name.Trim();
        if (name.Length < data.Constants.MinAccountLength 
            || name.Length > data.Constants.MaxAccountLength) {
            NoticeOP.ShowText(Lang.Instance.Notice(NoticeKey.NoticeNameWrongLength));
            return;
        }
        var msg = new proto.CSRoleCreate();
        msg.Name = name;
        msg.Gender = (proto.GenderType)gender;
        msg.Job = (proto.JobType)job;
        Net.Instance.Send<proto.CSRoleCreate>(msg);
    }

    public static void DeleteRole(Int64 roleid) {
        var msg = new proto.CSRoleDelete();
        msg.Id = roleid;
        Net.Instance.AddResponse<proto.SCError>(Net.Instance.Send<proto.CSRoleDelete>(msg), (m, ses) => {
            if (m.Error == 0) {
                LoginM.Instance.RemoveRoleBase(roleid);
            }
        });
    }

    // 登录角色都是成功的
    public static void LoginRole(Int64 roleid) {
        var msg = new proto.CSRoleLogin();
        msg.Id = roleid;
        Net.Instance.Send<proto.CSRoleLogin>(msg);
    }
}