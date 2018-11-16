using System.Collections.Generic;
using UnityEngine;
using Automata.Base;

namespace Automata.Game
{
    public class Table<T> where T: class
    {
        public static List<T> List { get; private set; }
        public static void LoadArray(AsyncInfo req)
        {
            List = LitJson.JsonMapper.ToObject<List<T>>((req.Asset as TextAsset).text);
        }

        public static Dictionary<int, T> Dict { get; private set; }
        public static void LoadDict(AsyncInfo req)
        {
            var dict = LitJson.JsonMapper.ToObject<Dictionary<string, T>>((req.Asset as TextAsset).text);
            Dict = new Dictionary<int, T>();
            foreach (var it in dict)
            {
                Dict.Add(int.Parse(it.Key), it.Value);
            }
        }

        public static T Find(int index)
        {
            if (List != null)
            {
                return index < List.Count ? List[index] : null;
            }
            if (Dict != null)
            {
                T item = null;
                Dict.TryGetValue(index, out item);
                return item;
            }
            return null;
        }
    }
}
