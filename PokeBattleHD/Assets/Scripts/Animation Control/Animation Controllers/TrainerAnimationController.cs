using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrainerAnimationController : CharacterAnimationControl
{
    /* Public Properties */
    public AnimationClip ballThrowAnimation;
    public CameraAnimation returnAnimation;

    /// <summary> Triggers ball throw animation. </summary>
    public void ThrowBall()
    {
        SetAnimTrigger(AnimTrigger.ThrowBall);

    }

    /// <summary> Sets return animation. </summary>
    /// <param name="state"> True for returning, false for not returning. </param>
    public void Returning(bool state)
    {
        SetAnimBool(AnimBool.Returning, state);
    }

    /// <summary> Trigger end of battle reaction. </summary>
    /// <param name="win"> True if this trainer won, false otherwise. </param>
    public void BattleEnd(bool win)
    {
        SetAnimTrigger(win ? AnimTrigger.Win : AnimTrigger.Lose);
    }
}
