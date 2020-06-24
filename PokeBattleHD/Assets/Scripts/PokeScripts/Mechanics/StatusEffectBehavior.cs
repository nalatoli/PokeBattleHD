using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[RequireComponent(typeof(StatusEffect))]
public abstract class StatusEffectBehavior : MonoBehaviour
{
    /// <summary> When to apply this effect. </summary>
    public abstract RoutinesHandler ApplyOn { get; }

    /// <summary> Priority to place condition on. </summary>
    public abstract EventPriorityLevel Priority { get; }

    /// <summary> Applies effect on pokemon applicant. </summary>
    /// <returns> True if effect was applied, false otherwise. </returns>
    protected abstract bool Apply();

    [Tooltip("Text that is printed in dialogue when effect is applied."), TextArea]
    public string announcment = "";

    /// <summary> Determines whether this effect was applied this turn. </summary>
    public bool Applied { get; private set; }

    /// <summary> Pokemon this aliment effect applied to. </summary>
    protected Pokemon applicant;

    /// <summary> Particle system component of status effect. </summary>
    protected ParticleSystem part;

    protected virtual void Awake()
    {
        /* Get Applicant Parent and This Particle System */
        applicant = GetComponentInParent<Pokemon>();
        part = GetComponent<ParticleSystem>();

        /* Assign The Applicant's Skinned Mesh Renderer On The Shape Module Of This Particle System */
        ParticleSystem.ShapeModule shape = part.shape;
        shape.skinnedMeshRenderer = applicant.SMRenderer;

        /* Play Particles */
        RestartParticles();

    }

    /// <summary> Attempt to apply this effect. </summary>
    public IEnumerator Proc()
    {
        /* Get Whether This Effect Has Applied */
        Applied = Apply();

        /* If Effect Was Successfully Applied */
        if(Applied)
        {
            /* Announce The Effect */
            RestartParticles();
            yield return BattleManager.Announce(applicant, announcment, false);

        }        
    }

    protected void OnEnable()
    {
        /* Subscribe Application Effect */
        ApplyOn.Subscribe(Proc, Priority);
    }

    /// <summary> Plays status effect particle system and triggers hurt effect. </summary>
    public void RestartParticles()
    {
        part.Restart();
        applicant.Hits.ApplyForce(Vector3.down * 4);
    }

    protected void OnDisable()
    {
        /* Unsubscribe Application Effect */
        ApplyOn.Unsubscribe(Proc);
    }
}
