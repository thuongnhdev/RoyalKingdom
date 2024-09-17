using System;
using UnityEngine;

public class PlayerWalk : CharacterStateBase
{
    #region System methods

    public override void ActionWalk(Animator animator, Action action)
    {
        base.ActionWalk(animator, action);
        animator.SetBool("IsWalking", true);
    }

    #endregion
}
