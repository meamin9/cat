using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AM.Game
{
    public class SceneObject
    {

        public SceneConf Conf { get; private set;}

        public static SceneObject CreateScene(int sceneId)
        {
            if (!SceneConf.All.TryGetValue(sceneId, out SceneConf conf)) {
                Log.Error($"not Found Scene: {sceneId}");
                return null;
            }
            return new SceneObject(conf);
        }

        protected SceneObject(SceneConf conf)
        {
            Conf = conf;
        }

        public bool CanSwitchTo(SceneObject conf)
        {
            return true;
        }

        public void OnEnter()
        {
            return;
        }

        public void OnLeave()
        {
            return;

        }
    }
}
