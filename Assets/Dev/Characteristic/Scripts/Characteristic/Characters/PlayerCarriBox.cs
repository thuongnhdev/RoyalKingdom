using System;
using UnityEngine;

public class PlayerCarriBox : CharacterStateBase
{
    #region System methods

    public override void ActionCarriBox(Animator animator, Action action)
    {
        base.ActionCarriBox(animator, action);
        animator.SetBool("IsCarriBox", true);
    }

    #endregion
}
