using AM.Base;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace AM.Game
{
    public class Setting<T> : ScriptableObject where T: ScriptableObject
    {
        public static T Instance { get; private set; }
        public static void Load(AsyncInfo req)
        {
            Instance = req.Asset as T;
        }
    }

    [CreateAssetMenu(fileName = nameof(AppSetting), menuName = "ScriptableObjects/" + nameof(AppSetting), order = 2)]
    public class AppSetting : Setting<AppSetting>
    {
        public const string AssetPath = nameof(AppSetting) + Names.SETTING_EXT;


        [Header("Camera Follow")]
        public float FOV = 34;
        public Vector3 RelativePosition = new Vector3(0, 5, -7);
        public Vector3 Rotation = new Vector3(30, 0, 0);
    }
}
