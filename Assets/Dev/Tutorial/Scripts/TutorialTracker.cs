using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TutorialTracker : MonoBehaviour
{
    public const string ACTIVE_TUTORIAL = "ActiveTutorial";

    public UnityEvent _onNeedStartTutorial;
    [Header("Inspec")]
    [SerializeField]
    private string _tutorialFlag;

    public static bool NeedTutorial => PlayerPrefs.GetInt(ACTIVE_TUTORIAL, 1) == 1;

    public void SetTutorialFlag(bool active)
    {
        PlayerPrefs.SetInt(ACTIVE_TUTORIAL, active ? 1 : 0);
        PlayerPrefs.Save();
        SetInspecFlag();
    }

    public bool IsNeedTutorial()
    {
        return PlayerPrefs.GetInt(ACTIVE_TUTORIAL, 1) == 1;
    }

    public void ResetTutorialFlag()
    {
        PlayerPrefs.SetInt(ACTIVE_TUTORIAL, 1);
        PlayerPrefs.Save();
        SetInspecFlag();
    }

    public void CheckAndStartTutorialIfNeeded()
    {
        if (!IsNeedTutorial())
        {
            return;
        }

        _onNeedStartTutorial.Invoke();
    }

    public void CheckAndStartTutorialIfNeeded(Action startTutorialAction)
    {
        if (!IsNeedTutorial())
        {
            return;
        }

        startTutorialAction?.Invoke();
    }

    private void SetInspecFlag()
    {
        _tutorialFlag = IsNeedTutorial() ? "Active Tutorial" : "Inactive Tutorial";
    }
    private void OnValidate()
    {
        SetInspecFlag();
    }
}
