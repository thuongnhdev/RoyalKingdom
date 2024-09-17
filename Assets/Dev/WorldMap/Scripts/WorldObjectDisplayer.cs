using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldObjectDisplayer : MonoBehaviour
{
    [SerializeField]
    private IntegerVariable _zoomLevel;

    [Header("Configs")]
    [SerializeField]
    private int _showfromLevel;
    [SerializeField]
    private bool _disableObjectWhenHide = false;

    private Renderer[] _renderers;

    private void Awake()
    {
        _renderers = GetComponentsInChildren<Renderer>();        
    }

    private void Start()
    {
        ShowOrHide(_zoomLevel.Value);
        _zoomLevel.OnValueChange += ShowOrHide;
    }

    private void OnDestroy()
    {
        _zoomLevel.OnValueChange -= ShowOrHide;
    }

    private void ShowOrHide(int zoomLevel)
    {
        bool show = _showfromLevel <= zoomLevel;
        if (_disableObjectWhenHide)
        {
            gameObject.SetActive(show);
            return;
        }

        for (int i = 0; i < _renderers.Length; i++)
        {
            _renderers[i].enabled = show;
        }
    }
}
