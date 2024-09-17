using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using CoreData.UniFlow.Common;
using CoreData.UniFlow;

public class ItemBand : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI _tmpIndex;

    [SerializeField]
    private Image _imgIconBranch_1;

    [SerializeField]
    private Image _imgIconBranch_2;

    [SerializeField]
    private Image _imgIconBranch_3;

    [SerializeField]
    private TextMeshProUGUI _tmpNumberPawns;

    [SerializeField]
    private TextMeshProUGUI _tmpNumberMaxBranch_1;

    [SerializeField]
    private TextMeshProUGUI _tmpNumberMaxBranch_2;

    [SerializeField]
    private TextMeshProUGUI _tmpNumberMaxBranch_3;

    [SerializeField]
    private TextMeshProUGUI _tmpMoraleCount;

    [SerializeField]
    private Image _imgIconItem;

    [SerializeField]
    private Image _imgIconStatus;

    [SerializeField]
    private GameObject _imgHighlight;

    [SerializeField]
    private GameObject _imgSaled;

    [SerializeField]
    private Sprite _sprInfantry;

    [SerializeField]
    private Sprite _sprArcher;

    [SerializeField]
    private Sprite _sprCavalry;

    [SerializeField]
    private TextMeshProUGUI _tmpNumberStatus;

    private int _index;
    private UIBand _uiBand;
    private TroopData _troopData;
    private BaseBandDefaut _baseBandDefaut;
    private Action<bool, UIBand.TypeSelectBand, BaseBandDefaut> _onComplete;

    public void InitData(UIBand uiband, int index, TroopData troopData, BaseBandDefaut baseBandDefaut, Action<bool,UIBand.TypeSelectBand, BaseBandDefaut> onComplete)
    {
        _index = index;
        _uiBand = uiband;
        _troopData = troopData;
        _onComplete = onComplete;
        _baseBandDefaut = baseBandDefaut;
        _imgSaled.SetActive(false);
        _imgHighlight.SetActive(false);

        UpdateUI();

        // check band is selected
        UpdateBandSelect();
    }

    private void UpdateUI()
    {
        _tmpIndex.text = _baseBandDefaut.BandKey.ToString();
        _tmpNumberPawns.text = _baseBandDefaut.Pawns.ToString();
  
      
        if (_baseBandDefaut.Proficiency_1 > _baseBandDefaut.Proficiency_2 && _baseBandDefaut.Proficiency_1 > _baseBandDefaut.Proficiency_3)
        {
            _tmpNumberMaxBranch_1.text = _baseBandDefaut.Proficiency_1.ToString();
            _imgIconBranch_1.sprite = _sprInfantry;
            if(_baseBandDefaut.Proficiency_2 > _baseBandDefaut.Proficiency_3)
            {
                _tmpNumberMaxBranch_2.text = _baseBandDefaut.Proficiency_2.ToString();
                _tmpNumberMaxBranch_3.text = _baseBandDefaut.Proficiency_3.ToString();
                _imgIconBranch_2.sprite = _sprCavalry;
                _imgIconBranch_3.sprite = _sprArcher;
            }
            else
            {
                _tmpNumberMaxBranch_2.text = _baseBandDefaut.Proficiency_3.ToString();
                _tmpNumberMaxBranch_3.text = _baseBandDefaut.Proficiency_2.ToString();
                _imgIconBranch_2.sprite = _sprArcher;
                _imgIconBranch_3.sprite = _sprCavalry;
            }
        }
        else if(_baseBandDefaut.Proficiency_2 > _baseBandDefaut.Proficiency_1 && _baseBandDefaut.Proficiency_2 > _baseBandDefaut.Proficiency_3)
        {
            _tmpNumberMaxBranch_1.text = _baseBandDefaut.Proficiency_2.ToString();
            _imgIconBranch_1.sprite = _sprCavalry;
            _imgIconBranch_1.rectTransform.sizeDelta = new Vector2(52, 72);
            if (_baseBandDefaut.Proficiency_1 > _baseBandDefaut.Proficiency_3)
            {
                _tmpNumberMaxBranch_2.text = _baseBandDefaut.Proficiency_1.ToString();
                _tmpNumberMaxBranch_3.text = _baseBandDefaut.Proficiency_3.ToString();
                _imgIconBranch_2.sprite = _sprInfantry;
                _imgIconBranch_3.sprite = _sprArcher;
            }
            else
            {
                _tmpNumberMaxBranch_2.text = _baseBandDefaut.Proficiency_3.ToString();
                _tmpNumberMaxBranch_3.text = _baseBandDefaut.Proficiency_1.ToString();
                _imgIconBranch_2.sprite = _sprArcher;
                _imgIconBranch_3.sprite = _sprInfantry;
            }
        }
        else if(_baseBandDefaut.Proficiency_3 > _baseBandDefaut.Proficiency_1 && _baseBandDefaut.Proficiency_3 > _baseBandDefaut.Proficiency_2)
        {
            _tmpNumberMaxBranch_1.text = _baseBandDefaut.Proficiency_3.ToString();
            _imgIconBranch_1.sprite = _sprArcher;
            if (_baseBandDefaut.Proficiency_1 > _baseBandDefaut.Proficiency_2)
            {
                _tmpNumberMaxBranch_2.text = _baseBandDefaut.Proficiency_1.ToString();
                _tmpNumberMaxBranch_3.text = _baseBandDefaut.Proficiency_2.ToString();
                _imgIconBranch_2.sprite = _sprInfantry;
                _imgIconBranch_3.sprite = _sprCavalry;
            }
            else
            {
                _tmpNumberMaxBranch_2.text = _baseBandDefaut.Proficiency_2.ToString();
                _tmpNumberMaxBranch_3.text = _baseBandDefaut.Proficiency_1.ToString();
                _imgIconBranch_2.sprite = _sprCavalry;
                _imgIconBranch_3.sprite = _sprInfantry;
            }
        }

        _imgIconBranch_2.SetNativeSize();
        _imgIconBranch_3.SetNativeSize();

        _tmpMoraleCount.text = string.Format("{0}", _baseBandDefaut.MoraleCount.ToString());
    }

    private void UpdateBandSelect()
    {
        for (int i = 0; i < UIManagerTown.Instance.UITroop.TroopDatas.Count; i++)
        {
            var troop = UIManagerTown.Instance.UITroop.TroopDatas[i];
            var bandData = troop.Band.Find(t => t.BandKey == _baseBandDefaut.BandKey);
            if (troop.IdTroop == _troopData.IdTroop)
            {
                if (bandData != null)
                    _imgHighlight.SetActive(true);
            }
            else
            {
                if (bandData != null)
                    _imgSaled.SetActive(true);
            }
        }
    }

    public void OnClickOpenBandItem()
    {
        //UIManagerTown.Instance.ShowUIBandItem(() => { });
    }

    public void OnReset()
    {
        _imgHighlight.SetActive(false);
    }

    public void OnSelectItem()
    {
        var itemChoiced = _uiBand.ItemBandChoiced.Find(t => t.BandKey == _baseBandDefaut.BandKey);
        if (_uiBand.ItemBandChoiced.Count > 3 && itemChoiced == null) return;
        if (_imgHighlight.activeInHierarchy)
        {
            var itemBand = _troopData.Band.Find(t => t.BandKey == _baseBandDefaut.BandKey);
            if (itemBand != null)
            {
                var troop = UIManagerTown.Instance.UITroop.TroopDatas.Find(t => t.IdTroop == _troopData.IdTroop);
                if(troop == null)
                {
                    _imgSaled.SetActive(false);
                    _imgHighlight.SetActive(false);
                    _onComplete?.Invoke(false, UIBand.TypeSelectBand.NoSelect, _baseBandDefaut);
                }
                else
                {

                    _imgSaled.SetActive(false);
                    _imgHighlight.SetActive(false);
                    var bandData = troop.Band.Find(t => t.BandKey == _baseBandDefaut.BandKey);
                    if (bandData != null)
                        _onComplete?.Invoke(false, UIBand.TypeSelectBand.Remove, _baseBandDefaut);
                    else
                        _onComplete?.Invoke(false, UIBand.TypeSelectBand.NoSelect, _baseBandDefaut);
                }
            }
        }
        else
        {
            _imgHighlight.SetActive(true);
            _onComplete?.Invoke(true, UIBand.TypeSelectBand.Select, _baseBandDefaut);
        }
    }
}
