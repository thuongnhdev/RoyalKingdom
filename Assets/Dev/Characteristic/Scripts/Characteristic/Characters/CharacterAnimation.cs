using CoreData.UniFlow.Common;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterAnimation : MonoBehaviour, ICharacterStateBase
{
    private Animator _animator;
    private ICharacterStateBase _iCharacterStateBase;

    public void InitAnimator(Animator animator)
    {
        _animator = animator;
    }
  
    public void StartAnimation(StatusAction statusAction, Action<bool> onComplete = null)
    {
        switch (statusAction)
        {
            case StatusAction.Idle:
                ActionIdle(_animator, () => {
                    onComplete?.Invoke(true);
                });
                break;
            case StatusAction.Walk:
                ActionWalk(_animator, () => {
                    onComplete?.Invoke(true);
                });
                break;
            case StatusAction.Farm:
                ActionFarmer(_animator, () => {
                    onComplete?.Invoke(true);
                });
                break;
            case StatusAction.Building:
                ActionBuild(_animator, () => {
                    onComplete?.Invoke(true);
                });
                break;
            case StatusAction.CarriBox:
                ActionCarriBox(_animator, () => {
                    onComplete?.Invoke(false);
                });
                break;
            case StatusAction.CarriBoxHigh:
                ActionCarriBoxHigh(_animator, () => {
                    onComplete?.Invoke(false);
                });
                break;
            case StatusAction.Gatherer:
                ActionHammer(_animator, () => {
                    onComplete?.Invoke(false);
                });
                break;
            case StatusAction.Attack:
                ActionAttack(_animator, () => {
                    onComplete?.Invoke(false);
                });
                break;
            case StatusAction.Run:
                ActionRun(_animator, () => {
                    onComplete?.Invoke(true);
                });
                break;
            case StatusAction.Die:
                ActionDie(_animator, () => {
                    onComplete?.Invoke(true);
                });
                break;
            case StatusAction.Manager00:
                ActionManager_00(_animator, () => {
                    onComplete?.Invoke(true);
                });
                break;
            case StatusAction.Manager01:
                ActionManager_01(_animator, () => {
                    onComplete?.Invoke(true);
                });
                break;
            case StatusAction.Work00:
                ActionWork_00(_animator, () => {
                    onComplete?.Invoke(true);
                });
                break;
            case StatusAction.Work01:
                ActionWork_01(_animator, () => {
                    onComplete?.Invoke(true);
                });
                break;
            case StatusAction.Finish:
                ActionFinish(_animator, () => {
                    onComplete?.Invoke(true);
                });
                break;
            case StatusAction.Idle01:
                ActionIdle01(_animator, () => {
                    onComplete?.Invoke(true);
                });
                break;
            case StatusAction.Idle02:
                ActionIdle02(_animator, () => {
                    onComplete?.Invoke(true);
                });
                break;
            case StatusAction.IsTouch:
                ActionTouch(_animator, () => {
                    onComplete?.Invoke(true);
                });
                break;
            case StatusAction.IsIdle99:
                ActionIdle_99(_animator, () => {
                    onComplete?.Invoke(true);
                });
                break;
        }
    }

    public ICharacterStateBase GetICharacterStateBase()
    {
        if (_iCharacterStateBase == null)
            _iCharacterStateBase = new CharacterStateBase();
        return _iCharacterStateBase;
    }

    public void ActionReset(Animator animator)
    {
        GetICharacterStateBase().ActionReset(animator);
    }
    public void ActionJump(Animator animator, Action action)
    {
        action?.Invoke();
        GetICharacterStateBase().ActionJump(animator, () => { });
    }

    public void ActionRun(Animator animator, Action action)
    {
        action?.Invoke();
        GetICharacterStateBase().ActionRun(animator, () => { });
    }

    public void ActionAttack(Animator animator, Action action)
    {
        action?.Invoke();
        GetICharacterStateBase().ActionAttack(animator, () => { });
    }

    public void ActionWalk(Animator animator, Action action)
    {
        action?.Invoke();
        GetICharacterStateBase().ActionWalk(animator, () => { });
    }

    public void ActionIdle(Animator animator, Action action)
    {
        action?.Invoke();
        GetICharacterStateBase().ActionIdle(animator, () => { });
    }

    public void ActionDie(Animator animator, Action action)
    {
        action?.Invoke();
        GetICharacterStateBase().ActionDie(animator, () => { });
    }

    public void ActionHammer(Animator animator, Action action)
    {
        action?.Invoke();
        GetICharacterStateBase().ActionHammer(animator, () => { });
    }

    public void ActionFarmer(Animator animator, Action action)
    {
        action?.Invoke();
        GetICharacterStateBase().ActionFarmer(animator, () => { });
    }

    public void ActionMine(Animator animator, Action action)
    {
        action?.Invoke();
        GetICharacterStateBase().ActionMine(animator, () => { });
    }

    public void ActionCarriBox(Animator animator, Action action)
    {
        action?.Invoke();
        GetICharacterStateBase().ActionCarriBox(animator, () => { });
    }
    public void ActionCarriBoxHigh(Animator animator, Action action)
    {
        action?.Invoke();
        GetICharacterStateBase().ActionCarriBoxHigh(animator, () => { });
    }


    public void ActionBuild(Animator animator, Action action)
    {
        action?.Invoke();
        GetICharacterStateBase().ActionBuild(animator, () => { });
    }

    public void ActionManager_00(Animator animator, Action action)
    {
        action?.Invoke();
        GetICharacterStateBase().ActionManager_00(animator, () => { });
    }

    public void ActionManager_01(Animator animator, Action action)
    {
        action?.Invoke();
        GetICharacterStateBase().ActionManager_01(animator, () => { });
    }

    public void ActionWork_00(Animator animator, Action action)
    {
        action?.Invoke();
        GetICharacterStateBase().ActionWork_00(animator, () => { });
    }

    public void ActionWork_01(Animator animator, Action action)
    {
        action?.Invoke();
        GetICharacterStateBase().ActionWork_01(animator, () => { });
    }

    public void ActionFinish(Animator animator, Action action)
    {
        action?.Invoke();
        GetICharacterStateBase().ActionFinish(animator, () => { });
    }

    public void ActionIdle01(Animator animator, Action action)
    {
        action?.Invoke();
        GetICharacterStateBase().ActionIdle01(animator, () => { });
    }

    public void ActionIdle02(Animator animator, Action action)
    {
        action?.Invoke();
        GetICharacterStateBase().ActionIdle02(animator, () => { });
    }

    public void ActionTouch(Animator animator, Action action)
    {
        action?.Invoke();
        GetICharacterStateBase().ActionTouch(animator, () => { });
    }

    public void ActionIdle_99(Animator animator, Action action)
    {
        action?.Invoke();
        GetICharacterStateBase().ActionIdle_99(animator, () => { });
    }

}
