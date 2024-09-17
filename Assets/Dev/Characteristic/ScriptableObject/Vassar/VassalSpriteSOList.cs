using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "VassalSpriteList", menuName = "Uniflow/Character/VassalSpriteList")]
public class VassalSpriteSOList : ScriptableObject
{
    [SerializeField]
    private List<VassalSprite> _vassalSpriteList;
    private Dictionary<int, VassalSprite> _vassalDict = new();
    private Dictionary<int, VassalSprite> VassalDict
    {
        get
        {
            if (_vassalDict.Count != _vassalSpriteList.Count)
            {
                _vassalDict.Clear();
                for (int i = 0; i < _vassalSpriteList.Count; i++)
                {
                    var vassal = _vassalSpriteList[i];
                    _vassalDict[vassal.vassalId] = vassal;
                }
            }

            return _vassalDict;
        }
    }

    public VassalSprite GetVassalSprites(int vassalId)
    {
        VassalDict.TryGetValue(vassalId, out var vassal);
        if (vassal == null)
        {
            Debug.LogWarning($"VassalId [{vassalId}] does not have any Sprite assets");
        }

        return vassal;
    }

    public Sprite GetPortrailSprite(int vassalId)
    {
        var vassal = GetVassalSprites(vassalId);
        if (vassal == null)
        {
            return null;
        }

        return vassal.portrailIcon;
    }

    public Sprite GetSmallIcon(int vassalId)
    {
        var vassal = GetVassalSprites(vassalId);
        if (vassal == null)
        {
            return null;
        }

        return vassal.smallIcon;
    }

    public Sprite GetLargeIcon(int vassalId)
    {
        var vassal = GetVassalSprites(vassalId);
        if (vassal == null)
        {
            return null;
        }

        return vassal.largeIcon;
    }

    public Sprite GetHexaIcon(int vassalId)
    {
        var vassal = GetVassalSprites(vassalId);
        if (vassal == null)
        {
            return null;
        }

        return vassal.HexaIcon;
    }
}

[System.Serializable]
public class VassalSprite
{
    public int vassalId;
    public Sprite portrailIcon;
    public Sprite smallIcon;
    public Sprite largeIcon;
    public Sprite HexaIcon;
}
