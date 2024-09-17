using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LandGeneralInfoMenu : MonoBehaviour
{
    [Header("Reference - Read")]
    [SerializeField]
    private UserProfile _profile;
    [SerializeField]
    private LandStaticInfoList _landStaticInfos;
    [SerializeField]
    private LandDynamicInfoList _landDynamicInfos;

    [Header("Configs")]
    [SerializeField]
    private Image _landFlag;
    [SerializeField]
    private Image _kingdomFlag;
    [SerializeField]
    private TMP_Text _landName;
    [SerializeField]
    private TMP_Text _kingdomName;
    [SerializeField]
    private TMP_Text _townNumberText;
    [SerializeField]
    private TMP_Text _prosperityText;
    [SerializeField]
    private TMP_Text _populationText;
    [SerializeField]
    private TMP_Text _reputationText;
    [SerializeField]
    private TMP_Text _religionText;
    [SerializeField]
    private TMP_Text _cultureText;

    public void SetUpContent()
    {
        var landStaticInfo = _landStaticInfos.GetLand(_profile.landId);
        if (landStaticInfo == null)
        {
            return;
        }
        _landName.text = landStaticInfo.landName;
        _religionText.text = landStaticInfo.religion.ToString();
        _cultureText.text = landStaticInfo.culture.ToString();

        var landDynamicInfo = _landDynamicInfos.GetLand(_profile.landId);
        if (landDynamicInfo == null)
        {
            return;
        }
        _kingdomName.text = landDynamicInfo.kingdom.ToString();
        _townNumberText.text = $"{landDynamicInfo.members.Count}/{landStaticInfo.maxTown}";
        _prosperityText.text = landDynamicInfo.prosperity.ToString();
        _populationText.text = landDynamicInfo.population.ToString();
        _reputationText.text = landDynamicInfo.reputation.ToString();


    }
}
