using System;
using UnityEngine;

public class PlayerRun : CharacterStateBase
{
    #region System methods

    public override void ActionRun(Animator animator, Action action)
    {
        base.ActionRun(animator, action);
        animator.SetBool("IsRun", true);
    }

    #endregion
}
