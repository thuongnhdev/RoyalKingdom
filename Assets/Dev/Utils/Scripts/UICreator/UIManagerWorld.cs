using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using CoreData.UniFlow.Common;

public class UIManagerWorld : MonoSingleton<UIManagerWorld>
{
    [SerializeField] private UICustomFlag m_UICustomFlag;

    [SerializeField] private UISymbolTicket m_UISymbolTicket;

    public UICustomFlag UICustomFlag { get => m_UICustomFlag; set => m_UICustomFlag = value; }
    public UISymbolTicket UISymbolTicket { get => m_UISymbolTicket; set => m_UISymbolTicket = value; }

    public void Start()
    {

        // GetTime 
        _ = APIManager.Instance.GetTime();
    }

    public void Init()
    {
        m_UICustomFlag.Init();
        m_UISymbolTicket.Init();
    }

    public void ShowUICustomFlag()
    {
        if (m_UICustomFlag != null)
        {
            m_UICustomFlag.Open();
        }
    }

    public void ShowUISymbolTicket(Action<bool> onComplete)
    {
        if (m_UISymbolTicket != null)
        {
            m_UISymbolTicket.SetData(new object[] { onComplete });
            m_UISymbolTicket.Open();
        }
    }

}
