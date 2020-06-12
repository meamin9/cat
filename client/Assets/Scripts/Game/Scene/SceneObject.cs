using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game
{
    public class SceneObject
    {

        public SceneTable Conf { get; private set;}

        public static SceneObject CreateScene(int sceneId)
        {
            if (!Table<SceneTable>.all.TryGetValue(sceneId, out SceneTable conf)) {
                Log.Error($"not Found Scene: {sceneId}");
                return null;
            }
            return new SceneObject(conf);
        }

        protected SceneObject(SceneTable conf)
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
