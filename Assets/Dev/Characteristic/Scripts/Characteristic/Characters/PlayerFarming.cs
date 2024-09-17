using System;
using UnityEngine;

public class PlayerFarming : CharacterStateBase
{
    #region System methods

    public override void ActionFarmer(Animator animator, Action action)
    {
        base.ActionFarmer(animator , action);
        animator.SetBool("IsFarming", true);
    }

    #endregion
}
