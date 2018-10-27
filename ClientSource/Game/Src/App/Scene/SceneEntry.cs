using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Automata.Base;
using UnityEngine;

namespace Automata.Game
{
    public class SceneEntry
    {
        public const string JsonPath = "data/scene.json";
        public static Dictionary<int, SceneEntry> _allScenes;
        public static void Load(Object obj)
        {
            _allScenes = JsonUtility.FromJson<Dictionary<int, SceneEntry>>((obj as TextAsset).text);
        }

        public static SceneEntry Find(int id)
        {
            SceneEntry entry;
            _allScenes.TryGetValue(id, out entry);
            return entry;
        }
        
        public int Id;
        public int Type;
        public string Name;
        public string ResPath;
        public string NavRes;
    }

}
