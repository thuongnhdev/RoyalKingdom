using System;
using UnityEngine;

public class PlayerCarrying : CharacterStateBase
{
    #region System methods

    public override void ActionMine(Animator animator, Action action)
    {
        base.ActionMine(animator , action);
        animator.SetBool("IsMining", true);
    }

    #endregion
}
