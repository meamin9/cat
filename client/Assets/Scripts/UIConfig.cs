namespace Game {
    public class UIConfig {
        static readonly UIConfig _instance = new UIConfig();
        public static UIConfig I {
            get { return _instance; }
        }
        Dictionary<int, UIDefine> _uiconfigs;
        private UIConfig() {
            _uiconfigs;
        }
    }
}
