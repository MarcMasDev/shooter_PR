using System;
using UnityEngine;

namespace UOC
{

    public static class UOCUtils
    {
        
        public static T FindObjectOfType<T>() where T : UnityEngine.Object
        {
            T result = null;

#if UNITY_6000_0_OR_NEWER
            result = UnityEngine.Object.FindAnyObjectByType<T>();
#else
            result = UnityEngine.Object.FindObjectOfType<T>();            
#endif

            return result;
        }

        public static UnityEngine.Object FindObjectOfType(Type t)
        {
            UnityEngine.Object result = null;

#if UNITY_6000_0_OR_NEWER
            result = UnityEngine.Object.FindAnyObjectByType(t);
#else
            result = UnityEngine.Object.FindObjectOfType(t);
#endif

            return result;
        }

        public static T[] FindObjectsOfType<T>() where T : UnityEngine.Object
        {
            T[] result = null;

#if UNITY_6000_0_OR_NEWER
            result = UnityEngine.Object.FindObjectsByType<T>(FindObjectsSortMode.None);
#else
            result = UnityEngine.Object.FindObjectsOfType<T>();
#endif

            return result;
        }

        public static UnityEngine.Object[] FindObjectsOfType(Type t) 
        {
            UnityEngine.Object[] result = null;

#if UNITY_6000_0_OR_NEWER
            result = UnityEngine.Object.FindObjectsByType(t, FindObjectsSortMode.None);
#else
            result = UnityEngine.Object.FindObjectsOfType(t);
#endif

            return result;
        }

    }

}


