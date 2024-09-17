using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LandInfoUiContent : MonoBehaviour
{
    [Header("Reference - Read")]
    [SerializeField]
    private UserProfile _userProfile;
    [SerializeField]
    private LandStaticInfoList _landsStaticInfos;
    [SerializeField]
    private LandDynamicInfoList _landsDynamicInfos;
    [SerializeField]
    private KingdomList _kingdoms;

    [Header("Configs")]
    [SerializeField]
    private TMP_Text _landNameText;
    [SerializeField]
    private TMP_Text _kingdomName;
    [SerializeField]
    private TMP_Text _landlordName;
    [SerializeField]
    private TMP_Text _memberText;
    [SerializeField]
    private TMP_Text _popText;
    [SerializeField]
    private TMP_Text _prosperityText;

    public void SetUpContent()
    {
        var staticInfos = _landsStaticInfos.GetLand(_userProfile.landId);
        if (staticInfos == null)
        {
            return;
        }

        var dynamicInfos = _landsDynamicInfos.GetLand(_userProfile.landId);

        _landNameText.text = staticInfos.landName;
        _kingdomName.text = _kingdoms.GetKingdomName(dynamicInfos.kingdom);
        _landlordName.text = dynamicInfos.ownerName;
        _memberText.text = $"{dynamicInfos.members.Count}/{staticInfos.maxTown}";
        _popText.text = dynamicInfos.population.ToString();
        _prosperityText.text = dynamicInfos.prosperity.ToString();
    }
}
