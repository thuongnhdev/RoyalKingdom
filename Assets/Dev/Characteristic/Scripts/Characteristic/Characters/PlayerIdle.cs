using System;
using UnityEngine;

public class PlayerIdle : CharacterStateBase
{
    #region System methods

    public override void ActionIdle(Animator animator, Action action)
    {
        base.ActionIdle(animator, action);
        animator.SetBool("IsWalking", false);
    }

    #endregion
}
