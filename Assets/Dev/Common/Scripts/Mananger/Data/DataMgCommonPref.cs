using UnityEngine;

public class DataMgCommonPref : MonoBehaviour
{
    public static DataMgCommonPref Instance = null;
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
