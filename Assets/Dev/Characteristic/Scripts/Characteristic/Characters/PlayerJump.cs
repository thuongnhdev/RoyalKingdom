using System;
using UnityEngine;

public class PlayerJump : CharacterStateBase
{
    #region System methods
    public override void ActionJump(Animator animator, Action action)
    {
        animator.SetBool("IsJumps", true);
        base.ActionJump(animator, action);
    }

    #endregion
}
