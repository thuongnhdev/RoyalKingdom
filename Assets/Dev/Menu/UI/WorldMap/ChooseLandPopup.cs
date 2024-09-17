using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using WorldMapStrategyKit;

public class ChooseLandPopup : MonoBehaviour
{
    [Header("Reference - Read")]
    [SerializeField]
    private LandStaticInfoList _landsStaticInfos;
    [SerializeField]
    private LandDynamicInfoList _landDynamicInfos;
    [SerializeField]
    private ItemList _items;
    [SerializeField]
    private IntegerVariable _selectedLand;

    [Header("Configs")]
    [SerializeField]
    private TMP_Text _landInfoText;

    private WMSK map;

    public void SetUpContent()
    {
        var land = _landsStaticInfos.GetLand(_selectedLand.Value);
        if (land == null)
        {
            _landInfoText.text = "This land is not available at this time!";
            return;
        }
        var sb = new StringBuilder();
        var landIndex = map.GetProvinceIndex(_selectedLand.Value);
        sb.Append("LandName: ").AppendLine(map.GetProvince(landIndex).name);
        sb.Append("Max City: ").AppendLine(land.maxCity.ToString());
        sb.Append("Regional Product: ").AppendLine(_items.GetItemName(land.regionalProduct));

        _landInfoText.text = sb.ToString();
    }

    private void OnEnable()
    {
        map = WMSK.instance;
    }
}
