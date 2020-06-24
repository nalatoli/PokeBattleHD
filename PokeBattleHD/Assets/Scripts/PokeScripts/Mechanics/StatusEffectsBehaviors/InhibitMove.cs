using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InhibitMove : StatusEffectBehavior
{
    [Tooltip("Chance the pokemon's move will be inhibited."), Range(0, 1)]
    public float chance = 0.3f;

    public override RoutinesHandler ApplyOn => applicant.Events.BeforeMoveUse;

    public override EventPriorityLevel Priority => EventPriorityLevel.Level2;

    protected override bool Apply()
    {
        /* Return Failure If Applicant's Current Move Is Already Null or RNG Doesn't Bless */
        if (applicant.CurrentMove == null || Random.value > chance)
            return false;
        
        /* Otherwise, Nullify The Pokemon's Current Move and Return Success */
        applicant.CurrentMove = null;
        return true;

    }
}
