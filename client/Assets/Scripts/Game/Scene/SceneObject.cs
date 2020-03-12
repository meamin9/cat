using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AM.Game
{
    public class SceneObject
    {

        public SceneEntry Conf { get; private set;}

        public static SceneObject CreateScene(int sceneId)
        {
            if (!Entry<SceneEntry>.All.TryGetValue(sceneId, out SceneEntry conf)) {
                Log.Error($"not Found Scene: {sceneId}");
                return null;
            }
            return new SceneObject(conf);
        }

        protected SceneObject(SceneEntry conf)
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
