using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MilitaryItem : MonoBehaviour
{
    public int Index;
    Action OnCLickDetail;
    MilitaryData militaryData;
    MasterDataStore masterDataStore;

    [SerializeField]
    private Animator _model;

    private CharacterAnimation _characterAnimation;
    public void InitData(int index,MilitaryData data, Action onClickDetail)
    {
        Index = index;
        militaryData = data;
        OnCLickDetail = onClickDetail;
        _characterAnimation = new CharacterAnimation();
        _characterAnimation.InitAnimator(_model);
        Action(StatusAction.Idle, (isComplete) => { });
    }

    public void Action(StatusAction action, Action<bool> onComplete = null)
    {
        _characterAnimation.StartAnimation(action, (isComplete) => { onComplete?.Invoke(isComplete); });
    }

    public void Rotation(RotationAnimation rotationAnimation)
    {
        Vector3 rotation = new Vector3(0f, -220f, 0);
        switch (rotationAnimation)
        {
            case RotationAnimation.Top:
                rotation = new Vector3(0f, -220f, 0);
                break;
            case RotationAnimation.Bottom:
                rotation = new Vector3(0f, -220f, 0);
                break;
            case RotationAnimation.Left:
                rotation = new Vector3(0f, -100f, 0);
                break;
            case RotationAnimation.Right:
                rotation = new Vector3(0f, 100f, 0);
                break;
            case RotationAnimation.Normal:
                rotation = new Vector3(0f, -220f, 0);
                break;
        }

        this.transform.eulerAngles = rotation;
    }

    public void OnClickOpenJob()
    {
        
    }

    public void OnReset()
    {
      
    }
}

public enum RotationAnimation
{
    Top = 1,
    Bottom,
    Left,
    Right,
    Normal
}
