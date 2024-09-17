using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;
using CoreData.UniFlow.Common;

public class ItemAvatar : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI _tmpLevel;

    [SerializeField]
    private Image _imgAvatar;

    [SerializeField]
    private Image _imgBgNormal;

    [SerializeField]
    private Sprite[] _sprGradeNormal;

    [SerializeField]
    private Image _imgBgSelect;


    [SerializeField]
    private Image _imgBgDisable;

    public VassalDataInGame VassalData;
    private Action<VassalDataInGame>_onSelectVassalInfo;
    public void InitData(int index,Sprite avatar , VassalDataInGame vassalData, Action<VassalDataInGame> onSelectVassalInfo)
    {
        VassalData = vassalData;
        _tmpLevel.text = vassalData.Data.Level.ToString();
        _onSelectVassalInfo = onSelectVassalInfo;
        if (avatar != null) _imgAvatar.sprite = avatar;
        SetActiveVassal(false);
    }

    public void OnClickSelectVassal()
    {
        SetActiveVassal(true);
        _onSelectVassalInfo?.Invoke(VassalData);
    }

    public void OnReset()
    {
        SetActiveVassal(false);
    }

    public void SetActiveVassal(bool isActive)
    {
        if (_imgBgNormal != null) _imgBgNormal.gameObject.SetActive(!isActive);
        if (_imgBgSelect != null) _imgBgSelect.gameObject.SetActive(isActive);
        int vassal = MasterDataStore.Instance.BaseVassalTemplates.Find(t => t.Key == VassalData.Data.idVassalTemplate).Grade;
        _imgBgNormal.sprite = _sprGradeNormal[0];
        if (vassal == 4)
            _imgBgNormal.sprite = _sprGradeNormal[1];
    }
}
