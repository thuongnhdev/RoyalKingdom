using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Assets.Dev.Tutorial.Scripts;
using TMPro;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine.UI;
using WorldMapStrategyKit;

public class LandContextMenu : MonoBehaviour
{
    [Header("Reference - Read")]
    [SerializeField]
    private KingdomList _kingdoms;
    [SerializeField]
    private LandStaticInfoList _landsStaticInfos;
    [SerializeField]
    private LandDynamicInfoList _landsDynamicInfos;
    [SerializeField]
    private LandAndKingdomService _landAndKingdom;
    [SerializeField]
    private WorldPathFindingService _pathFinding;
    [SerializeField]
    private UserProfile _userProfile;
    [SerializeField]
    private CityList _cities;
    [SerializeField]
    private ItemList _items;
    [SerializeField]
    private UserProfile _passport;
    [SerializeField]
    private IntegerVariable _zoomLevel;
    [SerializeField]
    private IntegerVariable _selectedLandGeoId;
    [SerializeField]
    private IntegerVariable _selectedCity;

    [Header("Events out")]
    [SerializeField]
    private GameEvent _onUserBoughtLand;
    [SerializeField]
    private GameEvent _onUserJoinedLand;

    [SerializeField]
    private GameEvent _onAttackLand;

    [SerializeField] 
    private GameEvent _onAttackTutorial;
    
    [Header("Configs")]
    [SerializeField]
    private GameObject _newUserGroup;
    [SerializeField]
    private Button _buyLandButton;
    [SerializeField]
    private Button _joinLandButton;
    [SerializeField]
    private TMP_Text _landInfoText;
    [SerializeField]
    private GameObject _oldUserGroup;
    [SerializeField]
    private TMP_Text _kingdomNameTextForOld;
    [SerializeField]
    private TMP_Text _landNameTextForOld;
    [SerializeField]
    private TMP_Dropdown _cityDropdown;
    [SerializeField]
    private TMP_Text _kingdomNameTextForNew;
    [SerializeField]
    private TMP_Text _landNameTextForNew;

    [SerializeField]
    private Button _attackButton;
    [SerializeField]
    private Button _moveButton;

    [SerializeField] private TutorialNpc _tutorialNpc;

    private int _displayLand = 0;
    bool _isDirty = false;

    void SetDirty(int notUsed)
    {
        _isDirty = true;
    }
    //private WMSK _map;

    public void SetUpContent(int notUsed)
    {
        if (_selectedLandGeoId.Value == 0)
        {
            return;
        }

        if (_passport.landId <= 0)
        {
            SetUpContentForNewUser();
            return;
        }

        _newUserGroup.SetActive(false);
        _oldUserGroup.SetActive(true);

        var land = _landsStaticInfos.GetLand(_selectedLandGeoId.Value);
        var mother = _landAndKingdom.motherLand(land.id);
        var motherLand = _landsStaticInfos.GetLand(mother);

        _landNameTextForOld.text = land.landName;
        _kingdomNameTextForOld.text = motherLand.landName;//_kingdoms.GetKingdomName(_landsDynamicInfos.GetKingdom(land.id));
        _kingdomNameTextForOld.ToHeavy();

        SetUpCityDropdown();
        bool attackable = _landAndKingdom.isEnemyLand(land.id) && _landAndKingdom.isLandCapital(land.cities[_cityDropdown.value]);
        var userLand = _landsStaticInfos.GetLand(_userProfile.landId);
        var path = _pathFinding.FindPath(userLand.cities[0], land.cities[_cityDropdown.value], _userProfile.id);
        var moveable = path != null && path.Count >= 2;
        _attackButton.gameObject.SetActive(attackable);
        _attackButton.interactable = moveable;
        _moveButton.gameObject.SetActive(!attackable);
        _moveButton.interactable = moveable;

        if (TutorialWorldmap.Instance.GetTutorialData().TutorialTracker.IsNeedTutorial() &&
            TutorialWorldmap.Instance.GetTutorialData().GetStepTutorial() == TutorialData.StepTutorial.TutorialBatleBraditCity)
        {
            _tutorialNpc.CloseNpcClickCity();
            _attackButton.gameObject.SetActive(true);
            _attackButton.interactable = true;
            _moveButton.gameObject.SetActive(false);
            _moveButton.interactable = true;
            _tutorialNpc.OpenNpcAttack(_attackButton.GetComponent<RectTransform>());
        }
    }

    public void JoinLand()
    {
        _onUserJoinedLand.Raise(_passport.id, _selectedLandGeoId.Value);
    }

    public void BuyLand()
    {
        _onUserBoughtLand.Raise(_passport.id, _selectedLandGeoId.Value);
    }

    public void OnAttackTutorial()
    {
        if (TutorialData.Instance.GetStepTutorial() == TutorialData.StepTutorial.TutorialTroopMoveAttack)
        {
            _cityDropdown.value = 0;
            _selectedLandGeoId.Value = 35;
            var land = _landsStaticInfos.GetLand(_selectedLandGeoId.Value);
            _selectedCity.Value = land.cities[_cityDropdown.value];
        }
        else if(TutorialData.Instance.GetStepTutorial() == TutorialData.StepTutorial.TutorialBatleBraditCity)
        {
            _cityDropdown.value = 0;
            _selectedLandGeoId.Value = 36;
            var land = _landsStaticInfos.GetLand(_selectedLandGeoId.Value);
            _selectedCity.Value = land.cities[_cityDropdown.value];
        }
        _onAttackTutorial.Raise();
    }

    public void Attack() {
        var land = _landsStaticInfos.GetLand(_selectedLandGeoId.Value);
        _selectedCity.Value = land.cities[_cityDropdown.value];
        if (TutorialData.Instance.TutorialTracker.IsNeedTutorial())
        {
            _tutorialNpc.CloseNpcAttack();
            _onAttackTutorial.Raise();
        }
        else
        {
            _onAttackLand.Raise();
        }
    
    }
    

    private void SetUpContentForNewUser()
    {
        _newUserGroup.SetActive(true);
        _oldUserGroup.SetActive(false);

        int id = _selectedLandGeoId.Value;
        var land = _landsStaticInfos.GetLand(id);
        bool landAvaiable = land != null && land.purchasable;

        _joinLandButton.interactable = landAvaiable;
        _buyLandButton.interactable = landAvaiable;

        //var landIndex = _map.GetProvinceIndex(landGeoId);
        _landNameTextForNew.text = land.landName;
        _landNameTextForNew.ToHeavy();
        _kingdomNameTextForNew.text = "No Kingdom";
        _kingdomNameTextForNew.ToHeavy();
        if (!landAvaiable)
        {
            _landInfoText.text = "This land is not available at this time!";
            return;
        }

        var sb = new StringBuilder();
        sb.AppendLine(land.maxCity.ToString());
        sb.AppendLine(_items.GetItemName(land.regionalProduct));

        int landId = land.id; //_landsStaticInfos.GetLandIdByGeoId(landGeoId);
        var landDynamicInfo = _landsDynamicInfos.GetLand(landId);
        if (landDynamicInfo == null)
        {
            sb.Append("There is no member in this land currently!");
            _landInfoText.text = sb.ToString();
            return;
        }
        _kingdomNameTextForNew.text = _kingdoms.GetKingdomName(landDynamicInfo.kingdom);
        _kingdomNameTextForNew.ToHeavy();

        sb.AppendLine($"{landDynamicInfo.members.Count} / {land.maxTown}");
        string owner = landDynamicInfo.owner == 0 ? "No owner" : landDynamicInfo.owner.ToString();
        sb.Append(owner);

        _landInfoText.text = sb.ToString();

        _buyLandButton.interactable = landDynamicInfo.owner == 0;
    }

    private void SetUpCityDropdown()
    {
        _cityDropdown.ClearOptions();
        var land = _landsStaticInfos.GetLand(_selectedLandGeoId.Value);
        List<int> cities = land.cities;
        if (cities.Count == 0)
        {
            return;
        }

        List<string> cityNames = new(3);
        int select = 0;
        for (int i = 0; i < cities.Count; i++)
        {
            cityNames.Add(_cities.GetCityName(cities[i]));
            if (_selectedCity.Value == cities[i])
                select = i;
        }
        _cityDropdown.AddOptions(cityNames);
        _cityDropdown.value = select;
    }


    private void OnEnable()
    {
        //_map = WMSK.instance;
        _selectedLandGeoId.OnValueChange += SetDirty;
        _selectedCity.OnValueChange += SetDirty;
    }

    private void OnDisable()
    {
        _selectedLandGeoId.OnValueChange -= SetDirty;
        _selectedCity.OnValueChange += SetDirty;
    }
    
}
