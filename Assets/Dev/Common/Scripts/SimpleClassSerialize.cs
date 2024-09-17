using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class TextInfo
{
    public Text TxObj;
    public E_GameTextDataType TxType;
    public string Key;
}

[System.Serializable]
public class MoveCamInfo
{
    public Vector3 Position;
    public Transform LookAtTarget;    

    private int Type;

    public void SetInfoType(int type)
    {
        Type = type;
    }
    public int GetInfoType()
    {
        return Type;
    }
}