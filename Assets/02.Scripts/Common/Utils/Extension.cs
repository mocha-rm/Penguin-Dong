using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Linq;
using UnityEngine;


public static class Extension
{
    public static T FindChild<T>(this GameObject go, string name = null, bool recursive = false) where T : UnityEngine.Object
    {
        return Util.FindChild<T>(go, name, recursive);
    }

    public static T GetOrAddComponent<T>(this GameObject go) where T : Component
    {
        return Util.GetOrAddComponent<T>(go);
    }
    public static T GetHierachyPath<T>(this GameObject go, string path) where T : UnityEngine.Component
    {
        return GetHierachyPath(go.transform, path)?.GetComponent<T>();
    }

    public static Transform GetHierachyPath(this GameObject go, string path)
    {
        return GetHierachyPath(go.transform, path);
    }


    public static Transform GetHierachyPath(this Transform transform, string path)
    {
        if (path == "") return transform;

        Transform rv = transform;
        var splitPaths = path.Replace('\\', '/').Split('/');
        foreach (var curPath in splitPaths)
        {
            rv = rv?.Find(curPath);
        }

        if (rv?.name == splitPaths.Last())
        {
            return rv;
        }
        return null;
    }

    public static bool IsAnimEnd(this Animator anim, string AnimName, int layerIdx = 0)
    {
        var state = anim.GetCurrentAnimatorStateInfo(layerIdx);
        return state.IsName(AnimName) == false || state.normalizedTime >= 1.0f;
    }
}



