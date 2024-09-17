using System;
using UnityEngine;

public class PlayerHammer : CharacterStateBase
{
    #region System methods

    public override void ActionHammer(Animator animator, Action action)
    {
        base.ActionHammer(animator, action);
        animator.SetBool("IsHammer", true);
    }

    #endregion
}
