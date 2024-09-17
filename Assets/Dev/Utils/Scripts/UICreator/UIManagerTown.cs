using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using CoreData.UniFlow.Common;

public class UIManagerTown : MonoSingleton<UIManagerTown>
{
    [SerializeField] private UIGameplayController m_UIGameplayController;

    [SerializeField] private UIPopulation m_UIPopulation;

    [SerializeField] private UIPopulationDetail m_UIPopulationDetail;

    [SerializeField] private UIStatusBoard m_UIStatusBoard;

    [SerializeField] private UIWorkerHouse m_UIWorkerHouse;

    [SerializeField] private UIWorkerHouseEdit m_UIWorkerHouseEdit;

    [SerializeField] private UIVassarName m_UIVassarName;

    [SerializeField] private UIVassalChoice m_UIVassalChoice;

    [SerializeField] private UIVassalInfomation m_UIVassalInfomation;

    [SerializeField] private UIVassalJobDetail m_UIVassalJobDetail;

    [SerializeField] private UIVassalJobSelect m_UIVassalJobSelect;

    [SerializeField] private UINotification m_UINotification;

    [SerializeField] private UIMilitaryHouse m_UIMilitaryHouse;

    [SerializeField] private UIVassalJobChange m_UIVassalJobChange;

    [SerializeField] private UIStatusBoardDetail m_UIStatusBoardDetail;

    [SerializeField] private UIBand m_UIBand;

    [SerializeField] private UIBandItem m_UIBandItem;

    [SerializeField] private UITroop m_UITroop;

    [SerializeField] private UIVassalChoiceMilitary m_UIVassalChoiceMilitary;

    public UIGameplayController UIGameplay { get => m_UIGameplayController; set => m_UIGameplayController = value; }
    public UIPopulation UIPopulation { get => m_UIPopulation; set => m_UIPopulation = value; }
    public UIPopulationDetail UIPopulationDetail { get => m_UIPopulationDetail; set => m_UIPopulationDetail = value; }
    public UIStatusBoard UIStatusBoard { get => m_UIStatusBoard; set => m_UIStatusBoard = value; }
    public UIWorkerHouse UIWorkerHouse { get => m_UIWorkerHouse; set => m_UIWorkerHouse = value; }
    public UIWorkerHouseEdit UIWorkerHouseEdit { get => m_UIWorkerHouseEdit; set => m_UIWorkerHouseEdit = value; }
    public UIVassarName UIVassarName { get => m_UIVassarName; set => m_UIVassarName = value; }
    public UIVassalChoice UIVassalChoice { get => m_UIVassalChoice; set => m_UIVassalChoice = value; }
    public UINotification UINotification { get => m_UINotification; set => m_UINotification = value; }
    public UIMilitaryHouse UIMilitaryHouse { get => m_UIMilitaryHouse; set => m_UIMilitaryHouse = value; }
    public UIVassalJobChange UIVassalJobChange { get => m_UIVassalJobChange; set => m_UIVassalJobChange = value; }
    public UIVassalInfomation UIVassalInfomation { get => m_UIVassalInfomation; set => m_UIVassalInfomation = value; }
    public UIVassalJobSelect UIVassalJobSelect { get => m_UIVassalJobSelect; set => m_UIVassalJobSelect = value; }
    public UIVassalJobDetail UIVassalJobDetail { get => m_UIVassalJobDetail; set => m_UIVassalJobDetail = value; }
    public UIStatusBoardDetail UIStatusBoardDetail { get => m_UIStatusBoardDetail; set => m_UIStatusBoardDetail = value; }
    public UIBand UIBand { get => m_UIBand; set => m_UIBand = value; }
    public UITroop UITroop { get => m_UITroop; set => m_UITroop = value; }
    public UIBandItem UIBandItem { get => m_UIBandItem; set => m_UIBandItem = value; }
    public UIVassalChoiceMilitary UIVassalChoiceMilitary { get => m_UIVassalChoiceMilitary; set => m_UIVassalChoiceMilitary = value; }
    public void Init()
    {
        m_UIGameplayController.Init();
        m_UIPopulation.Init();
        m_UIVassalChoice.Init();
    }

    public void ShowUIGamePlay()
    {
        if (m_UIGameplayController != null)
        {
            m_UIGameplayController.Open();
        }
    }

    public void ShowGameCompleted()
    {
        m_UIGameplayController.Close();

    }
    public void ShowUIPopulation(Action onComplete ,UIPopulation.TypePanel typePanel)
    {
        if (m_UIPopulation != null && !m_UIPopulation.isActiveAndEnabled)
        {
            m_UIPopulation.SetData(new object[] { onComplete, typePanel });
            m_UIPopulation.Open();
        }
    }

    public void ShowUIPopulation(Action onComplete = null)
    {
        if (m_UIPopulation != null && !m_UIPopulation.isActiveAndEnabled)
        {
            m_UIPopulation.SetData(new object[] {onComplete });
            m_UIPopulation.Open();
        }
    }

    public void ShowUIPopulationDetail()
    {
        if (m_UIPopulationDetail != null)
        {
            m_UIPopulationDetail.Open();
        }
    }

    public void ShowUIStatusBoard()
    {
        if (m_UIStatusBoard != null)
        {
            m_UIStatusBoard.Open();
        }
    }

    public void ShowUIWorkerHouse()
    {
        if (m_UIWorkerHouse != null)
        {
            m_UIWorkerHouse.Open();
        }
    }
    
    public void ShowUIWorkerHouseEdit()
    {
        if (m_UIWorkerHouseEdit != null)
        {
            m_UIWorkerHouseEdit.Open();
        }
    }

    public void ShowUIMilitaryHouse()
    {
        if (m_UIMilitaryHouse != null)
        {
            m_UIMilitaryHouse.Open();
        }
    }

    public void ShowUIVassarName()
    {
        if (m_UIVassarName != null)
        {
            m_UIVassarName.Open();
        }
    }

    public void ShowUIVassarChoice(int buildingObjId)
    {
        if (m_UIVassalChoice != null)
        {
            m_UIVassalChoice.SetData(new object[] { buildingObjId });
            m_UIVassalChoice.Open();
        }
    }

    public void ShowUIVassarChoiceMilitary(int idTroop,List<TroopData> troopDatas, int index, Action<List<TroopData>> onSelectComplete)
    {
        if (m_UIVassalChoiceMilitary != null)
        {
            m_UIVassalChoiceMilitary.SetData(new object[] { idTroop, troopDatas, index , onSelectComplete });
            m_UIVassalChoiceMilitary.Open();
        }
    }

    public void ShowUIVassarInformation()
    {
        if (m_UIVassalInfomation != null)
        {
            m_UIVassalInfomation.Open();
        }
    }

    public void ShowUIVassarJobDetail(VassalDataInGame vassal)
    {
        if (m_UIVassalJobDetail != null)
        {
            m_UIVassalJobDetail.SetData(new object[] {vassal});
            m_UIVassalJobDetail.Open();
        }
    }

    public void ShowUIVassarSelectJob(int index , JobData jobData, VassalDataInGame vassal,Action<UIVassalJobDetail.TypeSelectJob , VassalDataInGame, JobData> onComplete)
    {
        if (m_UIVassalJobSelect != null)
        {
            m_UIVassalJobSelect.SetData(new object[] { index, jobData, vassal, onComplete });
            m_UIVassalJobSelect.Open();
        }
    }

    public void ShowUIVassalJobChange(int idJobChange, JobData jobData, VassalDataInGame vassal, Action<UIVassalJobDetail.TypeSelectJob, VassalDataInGame, JobData> onComplete)
    {
        if (m_UIVassalJobChange != null)
        {
            m_UIVassalJobChange.SetData(new object[] { idJobChange, jobData, vassal, onComplete });
            m_UIVassalJobChange.Open();
        }
    }

    public void ShowUINotification()
    {
        if (m_UINotification != null)
        {
            m_UINotification.Open();
        }
    }

    public void ShowUIStatusBoardDetail(BaseDataEnumGame baseDataEnumGame,Action<TaskData> onPlace)
    {
        if (m_UIStatusBoardDetail != null)
        {
            m_UIStatusBoardDetail.SetData(new object[] { baseDataEnumGame ,onPlace});
            m_UIStatusBoardDetail.Open();
        }
    }

    public void ShowUIBand(TroopData troopData, Action<TroopData> onComplete)
    {
        if (m_UIBand != null)
        {
            m_UIBand.SetData(new object[] { troopData, onComplete });
            m_UIBand.Open();
        }
    }

    public void ShowUITroop(Action onComplete)
    {
        if (m_UITroop != null)
        {
            m_UITroop.SetData(new object[] { onComplete });
            m_UITroop.Open();
        }
    }

    public void ShowUIBandItem(Action onComplete)
    {
        if (m_UIBandItem != null)
        {
            m_UIBandItem.SetData(new object[] { onComplete });
            m_UIBandItem.Open();
        }
    }

}
