using System;
using System.Collections.Generic;

/// <summary>
/// UIConfig用来处理ui与prefab的n-1关系，优先获取uiconfig中的配置会覆盖调prefab上的默认配置
/// 因为大多数都是一一对应关系，所以保留了prefab上的默认配置，方便直接在prefab上修改
/// </summary>
public class UIConfig {
    static readonly UIConfig _instance = new UIConfig();
    public static UIConfig I {
        get { return _instance; }
    }

    Dictionary<string, UIDefine> _uiconfigs;
    private UIConfig() {
    }

	public UIDefine Find(string uiname) {
		UIDefine def;
		_uiconfigs.TryGetValue (uiname, out def);
		return def;
	}
}

