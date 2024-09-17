using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GameObjectExtensions
{
    public static void SetLayerRecursively(this GameObject target, int layer)
    {
        target.layer = layer;

        int childCount = target.transform.childCount;
        if (childCount == 0)
        {
            return;
        }

        var trans = target.transform;
        for (int i = 0; i < childCount; i++)
        {
            SetLayerRecursively(trans.GetChild(i).gameObject, layer);
        }
    }

    public static T GetOrAddComponent<T>(this GameObject target) where T : Component
    {
        var comp = target.GetComponent<T>();
        if (comp == null)
        {
            comp = target.AddComponent<T>();
        }

        return comp;
    }

    public static void SetMaterialRecursively(this GameObject target, Material mat)
    {
        var renderer = target.GetComponent<Renderer>();
        if (renderer == null)
        {
            return;
        }

        renderer.material = mat;

        int childCount = target.transform.childCount;
        if (childCount == 0)
        {
            return;
        }

        var trans = target.transform;
        for (int i = 0; i < childCount - 1; i++)
        {
            SetMaterialRecursively(trans.GetChild(i).gameObject, mat);
        }
    }
}
