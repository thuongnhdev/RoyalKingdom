using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "InternalAccounts", menuName ="Uniflow/User/InternalAccounts")]
public class InternalAccountList : ScriptableObject
{
    [SerializeField]
    private List<InternalAccount> _internalAccounts;

    public string GetUserName(string deviceName)
    {
        for (int i = 0; i < _internalAccounts.Count; i++)
        {
            if (_internalAccounts[i].deviceName == deviceName)
            {
                return _internalAccounts[i].name;
            }
        }

        return "";
    }
}

[System.Serializable]
public class InternalAccount
{
    public string name;
    public string deviceName;
}
