using DG.Tweening;
using System;
using UnityEngine;

public partial interface ICharacterStateBase
{
    void ActionJump(Animator animator , Action action);

    void ActionRun(Animator animator , Action action);

    void ActionAttack(Animator animator, Action action);

    void ActionWalk(Animator animator , Action action);

    void ActionIdle(Animator animator, Action action);

    void ActionIdle01(Animator animator, Action action);

    void ActionIdle02(Animator animator, Action action);

    void ActionDie(Animator animator, Action action);

    void ActionHammer(Animator animator, Action action);

    void ActionFarmer(Animator animator, Action action);

    void ActionMine(Animator animator, Action action);
    
    void ActionBuild(Animator animator, Action action);

    void ActionCarriBox(Animator animator, Action action);

    void ActionCarriBoxHigh(Animator animator, Action action);

    void ActionFinish(Animator animator, Action action);

    void ActionManager_00(Animator animator, Action action);

    void ActionManager_01(Animator animator, Action action);

    void ActionWork_00(Animator animator, Action action);

    void ActionWork_01(Animator animator, Action action);

    void ActionTouch(Animator animator, Action action);

    void ActionIdle_99(Animator animator, Action action);

    void ActionReset(Animator animator);

}

public class CharacterStateBase : MonoBehaviour, ICharacterStateBase
{
    public virtual void ActionJump(Animator animator , Action action)
    {
        ActionReset(animator);
        if (animator != null) animator.SetTrigger("IsJumps");
    }

    public virtual void ActionRun(Animator animator , Action action) {
        ActionReset(animator);
        if (animator != null) animator.SetTrigger("IsRun");
    }

    public virtual void ActionAttack(Animator animator, Action action)
    {
        ActionReset(animator);
        if (animator != null) animator.SetTrigger("IsAttack");
    }

    public virtual void ActionWalk(Animator animator , Action action) {
        ActionReset(animator);
        if (animator != null) animator.SetTrigger("IsWalking");
    }

    public virtual void ActionIdle(Animator animator, Action action) {
        ActionReset(animator);
        if (animator != null) animator.SetTrigger("IsIdle");
    }

    public virtual void ActionDie(Animator animator, Action action) {
        ActionReset(animator);
        if (animator != null) animator.SetTrigger("IsDie");
    }

    public virtual void ActionHammer(Animator animator, Action action) {
        ActionReset(animator);
        if (animator != null) animator.SetTrigger("IsHammer");
    }

    public virtual void ActionFarmer(Animator animator, Action action) {
        ActionReset(animator);
        if (animator != null) animator.SetTrigger("IsFarming");
    }

    public virtual void ActionBuild(Animator animator, Action action) {
        ActionReset(animator);
        if (animator != null) animator.SetTrigger("IsBuilding");
    }

    public virtual void ActionMine(Animator animator, Action action) {
        ActionReset(animator);
        if (animator != null) animator.SetTrigger("IsMining");
    }

    public virtual void ActionCarriBox(Animator animator, Action action) {
        ActionReset(animator);
        if (animator != null) animator.SetTrigger("IsCarriBox");
    }

    public virtual void ActionCarriBoxHigh(Animator animator, Action action)
    {
        ActionReset(animator);
        if (animator != null) animator.SetTrigger("IsCarriBoxHigh");
    }

    public virtual void ActionFinish(Animator animator, Action action)
    {
        ActionReset(animator);
        if (animator != null) animator.SetTrigger("IsFinish");
    }

    public virtual void ActionManager_00(Animator animator, Action action)
    {
        ActionReset(animator);
        if (animator != null) animator.SetTrigger("IsManager_00");
    }

    public virtual void ActionManager_01(Animator animator, Action action)
    {
        ActionReset(animator);
        if (animator != null) animator.SetTrigger("IsManager_01");
    }

    public virtual void ActionWork_00(Animator animator, Action action)
    {
        ActionReset(animator);
        if (animator != null) animator.SetTrigger("IsWork00");
    }

    public virtual void ActionWork_01(Animator animator, Action action)
    {
        ActionReset(animator);
        if (animator != null) animator.SetTrigger("IsWork01");
    }

    public virtual void ActionIdle01(Animator animator, Action action)
    {
        ActionReset(animator);
        if (animator != null) animator.SetTrigger("IsIdle01");
    }

    public virtual void ActionIdle02(Animator animator, Action action)
    {
        ActionReset(animator);
        if (animator != null) animator.SetTrigger("IsIdle02");
    }

    public virtual void ActionTouch(Animator animator, Action action)
    {
        ActionReset(animator);
        if (animator != null) animator.SetTrigger("IsTouch");
    }

    public virtual void ActionIdle_99(Animator animator, Action action)
    {
        ActionReset(animator);
        if (animator != null) animator.SetTrigger("IsIdle99");
    }
    public virtual void ActionReset(Animator animator)
    {
        if (animator == null) return;
        animator.ResetTrigger("IsFising");
        animator.ResetTrigger("IsCooking");
        animator.ResetTrigger("IsMining");
        animator.ResetTrigger("IsBuilding");
        animator.ResetTrigger("IsFarming");
        animator.ResetTrigger("IsHammer");
        animator.ResetTrigger("IsDie");
        animator.ResetTrigger("IsIdle");
        animator.ResetTrigger("IsIdle01");
        animator.ResetTrigger("IsIdle02");
        animator.ResetTrigger("IsWalking");
        animator.ResetTrigger("IsRun");
        animator.ResetTrigger("IsJumps");
        animator.ResetTrigger("IsCarriBox");
        animator.ResetTrigger("IsCarriBoxHigh");
        animator.ResetTrigger("IsTouch");
        animator.ResetTrigger("IsIdle99");
    }
}
