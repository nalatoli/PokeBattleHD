using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HPChange : StatusEffectBehavior
{
    [Tooltip("Determines whether to change the HP by a percentage or a flat amount.")]
    public bool percentage = true;

    [Tooltip("Percent change in HP."), ConditionalHide("percentage", false, -1f, 1f)]
    public float percent = -0.0625f;

    [Tooltip("Flat change in HP."), ConditionalHide("percentage", true)]
    public float flatAmount = -10;

    public override RoutinesHandler ApplyOn => EventManager.instance.TurnEnd;

    public override EventPriorityLevel Priority => EventPriorityLevel.Level2;

    protected override bool Apply()
    {
        /* Change Applicant's HP, Play Particles, and Return Success */
        applicant.Hp += (int)(percentage ? (applicant.Hp * percent) : flatAmount);
        RestartParticles();
        return true;
    }
}
