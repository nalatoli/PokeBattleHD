using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterAnimationControl : ScriptedAnimationController
{
    /// <summary> Override controller that allows custom clips to be played. </summary>
    protected AnimatorOverrideController overrideController;

    protected override void Awake()
    {
        base.Awake();
        overrideController = Animator.runtimeAnimatorController as AnimatorOverrideController;
    }

    /// <summary> Plays 'clip', overriding current clip. </summary>
    /// <param name="clip"> Animation clip to be played. </param>
    public void PlayAnimation(AnimationClip clip)
    {
        /* Warn User If NO State/Clip Exsists for This Animator */
        if (overrideController["Action"] == null)
        {
            Debug.LogError("No clip to override on state 'Action' for " + clip);
            return;
        }

        /* Override Empty "Action" Animation Clip and Trigger Animation */
        overrideController["Action"] = clip;
        SetAnimTrigger(AnimTrigger.DoAction);

    }




}
