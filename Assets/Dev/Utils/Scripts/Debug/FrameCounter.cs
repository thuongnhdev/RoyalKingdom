using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FrameCounter : MonoBehaviour
{
    [SerializeField]
    private bool _isMaster = false;
    private int _frameCount = 0;
    private GameObject[] _trackedObjects;

    private void OnEnable()
    {
        _frameCount = 0;
        Logger.Log("On Enable!, frame count = 0");

        if (!_isMaster)
        {
            return;
        }

        _trackedObjects = FindObjectsOfType<GameObject>();
        for (int i = 0; i < _trackedObjects.Length; i++)
        {
            var obj = _trackedObjects[i];
            if (!obj.activeSelf)
            { 
                continue;
            }
            obj.GetOrAddComponent<FrameCounter>();
        }
    }

    private void Start()
    {
        Logger.Log($"On Start of {gameObject.name}");
    }

    private void LateUpdate()
    {
        _frameCount++;
        Logger.Log($"Frame {_frameCount}");
    }
}
