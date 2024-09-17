using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomDataManager : MonoBehaviour
{    
    public static CustomDataManager Instance;

    [SerializeField]
    private List<ScriptableObject> Datas;

    void Awake()
    {
        Instance = this;
    }
    void OnDestroy()
    { 
        Instance = null;
    }

    private T GetCustomData<T>()
    {
        T data = default(T);

        for (int i = 0; i < Datas.Count; i++)
        {
            if (Datas[i] == null)
                continue;

            if (Datas[i].GetType() == typeof(T))
            {
                data = (T)Convert.ChangeType(Datas[i], typeof(T));
            }
        }
        return data;
    }

    public AnimationCurve GetAniCurveData(int key)
    {
        SO_AniCurveData dataInfo = GetCustomData<SO_AniCurveData>();

        if (dataInfo != null)
        {
            if (dataInfo.AniCurveDataList_Int.ContainsKey(key))
            {
                return dataInfo.AniCurveDataList_Int[key];
            }
            else
            {
                Debug.LogWarning("GetAniCurveDataInt invalid key!!");
                return null;
            }            
        }
        else
        {
            Debug.LogWarning("GetAniCurveDataInt invalid ScriptableObject!!");
            return null;
        }
        
    }
    public AnimationCurve GetAniCurveData(string key)
    {
        SO_AniCurveData dataInfo = GetCustomData<SO_AniCurveData>();

        if (dataInfo != null)
        {
            if (dataInfo.AniCurveDataList_Str.ContainsKey(key))
            {
                return dataInfo.AniCurveDataList_Str[key];
            }
            else
            {
                Debug.LogWarning("GetAniCurveDataInt invalid key!!");
                return null;
            }            
        }
        else
        {
            Debug.LogWarning("GetAniCurveDataInt invalid ScriptableObject!!");
            return null;
        }
    }
}
