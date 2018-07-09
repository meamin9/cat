using System;
using System.Collections.Generic;

class LoginM {
    static readonly LoginM _instance = new LoginM();
    public static LoginM Instance { get { return _instance; } }

    string _name;
    string _password;
    List<proto.RoleBase> _rolebases;
    public event Action EventRoleChanges;

    public string AccountName { 
        get { return _name; }
        set { _name = value; }
    }
    
    public string Password {
        get { return _password; }
        set { _password = value; }
    }

    public List<proto.RoleBase> RoleBases {
        get { return _rolebases; }
    }

    public void SetRoleBaseList(List<proto.RoleBase> rbs) {
        _rolebases = rbs;
        triggerEventRoleChanges();
    }

    public void AddRoleBase(proto.RoleBase rb) {
        _rolebases.Add(rb);
        triggerEventRoleChanges();
    }

    public void RemoveRoleBase(Int64 roleid) {
        _rolebases.RemoveAll((role) => {
            return role.Id == roleid;
        });
        triggerEventRoleChanges();
    }

    void triggerEventRoleChanges() {
        // TODO: 测试在Invoke中修改事件是否会正常
        if (EventRoleChanges != null) {
            EventRoleChanges.Invoke();
        }
    }
}
