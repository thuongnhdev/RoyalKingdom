using System;
using UnityEngine;

public class PlayerDie : CharacterStateBase
{
    #region System methods

    public override void ActionDie(Animator animator, Action action)
    {
        base.ActionDie(animator, action);
        animator.SetBool("IsDie", true);
    }

    #endregion
}
