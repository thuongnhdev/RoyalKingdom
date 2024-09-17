using System;
using UnityEngine;

public class PlayerAxeWorking : CharacterStateBase
{
    #region System methods

    public override void ActionFinish(Animator animator, Action action)
    {
        base.ActionFinish(animator, action);
        animator.SetBool("IsFinish", true);
    }

    #endregion
}
