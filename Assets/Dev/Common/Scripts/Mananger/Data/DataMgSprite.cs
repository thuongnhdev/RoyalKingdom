using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataMgSprite : MonoBehaviour
{
    public static DataMgSprite Instance = null;
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
