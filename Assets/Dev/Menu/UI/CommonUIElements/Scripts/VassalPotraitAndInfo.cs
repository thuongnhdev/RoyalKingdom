using CoreData.UniFlow;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class VassalPotraitAndInfo : BaseCustomCellView
{
    [Header("Reference - Read")]
    [SerializeField]
    private VassalSOList _vassals;
    [SerializeField]
    private VassalStatsSOList _vassalStats;
    [SerializeField]
    private VassalExpAndLevel _vassalExpAndLevel;
    [SerializeField]
    private VassalSpriteSOList _vassalAssets;
    [SerializeField]
    private UserVassalSOList _userVassals;

    [Header("Config")]
    [SerializeField]
    private TMP_Text _name;
    [SerializeField]
    private TMP_Text _description;
    [SerializeField]
    private Image _portrait;
    [SerializeField]
    private GameObject[] _relatedObjects;
    [SerializeField]
    private Image[] _statIcons;
    [SerializeField]
    private Slider[] _expSliders;
    [SerializeField]
    private TMP_Text[] _statNameTexts;
    [SerializeField]
    private TMP_Text[] _statValueTexts;
    [SerializeField]
    private TMP_Text[] _expTexts;

    public override void SetUp(object data)
    {
        int vassalId = (int)data;
        _portrait.overrideSprite = _vassalAssets.GetLargeIcon(vassalId);

        var vassal = _userVassals.GetVassal(vassalId);
        if (vassal == null)
        {
            SetActive(false);
            return;
        }

        SetActive(true);
        _name.text = _vassals.GetFullName(vassalId);
        _description.text = _vassals.GetDescription(vassalId);

        int[] shownStats = vassal.GetGreatestStatsId(3);
        int[] stats = vassal.stats;
        for (int i = 0; i < shownStats.Length; i++)
        {
            int showStatId = shownStats[i];
            _statNameTexts[i].text = _vassalStats.GetStatName(showStatId);
            _statIcons[i].overrideSprite = _vassalStats.GetStatIcon(showStatId);

            (int level, int exp) statInfo = _userVassals.GetVassalStatLevelAndExp(vassalId, shownStats[i]);
            _statValueTexts[i].text = statInfo.level.ToString();

            int expForNextLvl = _vassalExpAndLevel.GetExpRequiredForLevel(statInfo.level + 1);
            _expTexts[i].text = $"{statInfo.exp:#,###}/{expForNextLvl:#,###}";
            int expForCurrentLevel = _vassalExpAndLevel.GetExpRequiredForLevel(statInfo.level);
            int expDistance = expForNextLvl - expForCurrentLevel;
            int currentExpDistance = statInfo.exp - expForCurrentLevel;
            _expSliders[i].value = currentExpDistance / expDistance;
        }
    }

    public void SetActive(bool active)
    {
        _name.enabled = active;
        _description.enabled = active;
        _portrait.enabled = active;
        for (int i = 0; i < _statIcons.Length; i++)
        {
            _statIcons[i].enabled = active;
            _statNameTexts[i].enabled = active;
            _statValueTexts[i].enabled = active;
            _expTexts[i].enabled = active;
            _expSliders[i].gameObject.SetActive(active);
        }
        for (int i = 0; i < _relatedObjects.Length; i++)
        {
            _relatedObjects[i].SetActive(active);
        }
    }
}
