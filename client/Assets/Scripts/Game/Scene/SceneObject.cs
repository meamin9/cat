using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game
{
    public class SceneObject
    {

        public tScene table { get; private set;}

        public static SceneObject CreateScene(int sceneId)
        {
            var table = Table<tScene>.Find(sceneId);
            Log.ErrorIf(table == null, $"not Found Scene: {sceneId}");
            return new SceneObject(table);
        }

        protected SceneObject(tScene conf)
        {
            table = conf;
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
