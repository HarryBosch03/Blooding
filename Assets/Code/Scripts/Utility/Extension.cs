using UnityEngine;

namespace Blooding.Runtime.Utility
{
    public static class Extension
    {
        public static T Find<T>(this Transform transform, string path)
        {
            var find = transform.Find(path);
            return find ? find.GetComponent<T>() : default;
        }
    }
}