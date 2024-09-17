using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class LandInfoInWorldPopup : MonoBehaviour
{
    [Header("Reference - Read")]
    [SerializeField]
    private UserProfile _userProfile;
    [SerializeField]
    private KingdomList _kingdoms;
    [SerializeField]
    private LandStaticInfoList _landsStaticInfos;
    [SerializeField]
    private LandDynamicInfoList _landsDynamicInfos;
    [SerializeField]
    private ItemList _items;
    [SerializeField]
    private IntegerVariable _selectedLandGeoId;

    [Header("Configs")]
    [SerializeField]
    private TwoTextFieldsScrollItem _landName;
    [SerializeField]
    private TwoTextFieldsScrollItem _kingdomName;
    [SerializeField]
    private TwoTextFieldsScrollItem _regionalProduct;
    [SerializeField]
    private TwoTextFieldsScrollItem _terrain;
    [SerializeField]
    private TwoTextFieldsScrollItem _religion;
    [SerializeField]
    private TwoTextFieldsScrollItem _culture;
    [SerializeField]
    private TwoTextFieldsScrollItem _owner;
    [SerializeField]
    private TwoTextFieldsScrollItem _purchasable;
    [SerializeField]
    private TwoTextFieldsScrollItem _population;
    [SerializeField]
    private TwoTextFieldsScrollItem _prosperity;
    [SerializeField]
    private TwoTextFieldsScrollItem _reputation;
    [SerializeField]
    private TwoTextFieldsScrollItem _taxRate;
    [SerializeField]
    private TwoTextFieldsScrollItem _memberCount;

    public void SetUp()
    {
        int landId = _userProfile.landId; // For reusing this popup in Land Scene
        var landStatic = _landsStaticInfos.GetLand(landId);

        if (landStatic == null)
        {
            return;
        }
        var landDynamic = _landsDynamicInfos.GetLand(landId);

        _landName.SetUp(landStatic.landName);
        _kingdomName.SetUp(_kingdoms.GetKingdomName(landDynamic.kingdom));
        _regionalProduct.SetUp(_items.GetItemName(landStatic.regionalProduct));
        _terrain.SetUp(landStatic.terrainId.ToString());
        _religion.SetUp(landStatic.religion.ToString());
        _culture.SetUp(landStatic.culture.ToString());
        _owner.SetUp(landDynamic.ownerName);
        _purchasable.SetUp(landStatic.purchasable ? "Yes" : "No");
        _population.SetUp(landDynamic.population.ToString());
        _prosperity.SetUp(landDynamic.prosperity.ToString());
        _reputation.SetUp(landDynamic.reputation.ToString());
        _taxRate.SetUp(landDynamic.taxRate.ToString());
        _memberCount.SetUp(landDynamic.members.Count.ToString());
    }
}
