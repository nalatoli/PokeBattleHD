using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UIElements;

[RequireComponent(typeof(Pokemon))]
public class HitHandler : MonoBehaviour
{
    #region Properties

    /// <summary> Delay after revovery to start moving again. </summary>
    [Tooltip("Delay after revovery to start moving again.")]
    public float delayAfterRecovery = 1.5f;

    /// <summary> Determines if object is reacting to a hit. </summary>
    public bool IsReactingToHit { get; private set; }

    /// <summary> Determines if ragdoll is automatically recovered after getting hit. </summary>
    public bool AutomaticRecovery { get; set; }

    /// <summary> Pokemon tied to this hit handling system. </summary>
    private Pokemon pokemon;

    /// <summary> Navmesh agent tied to this hit handling system. </summary>
    private NavMeshAgent agent;

    /// <summary> Zone assigned to this pokemon. </summary>
    private Collider zone;

    /// <summary> Offset of zone so pokemon can rotate pokemon. </summary>
    private Transform wayPoint;

    #endregion

    private void Start()
    {
        /* Get Component*/
        pokemon = GetComponent<Pokemon>();
        agent = GetComponent<NavMeshAgent>();
        zone = transform.parent.GetComponent<Collider>();
        wayPoint = zone.transform.GetChild(0);
        agent.enabled = false;

        /* Set Default Properties */
        AutomaticRecovery = true;
        IsReactingToHit = false;
    }

    /// <summary> Applies a force to this ragdoll's rigidbody. The ragdoll must be enabled prior to calling this function. </summary>
    /// <param name="force"> The force to apply to the ragdoll. </param>
    public void ApplyForce(Vector3 force)
    {
        /* If NOT Reacting To A Hit, React To This Hit */
        if (!IsReactingToHit)
            StartCoroutine(ReactToHit(force));
    }

    /// <summary> Animates hit reaction. </summary>
    /// <param name="force"> Force applied during hit. </param>
    private IEnumerator ReactToHit(Vector3 force)
    {
        /* Set That This Object Is Reacting To A Hit */
        IsReactingToHit = true;

        /* Disable Nav AI So It Doesn't Interrupt Physics */
        agent.enabled = false;

        /* Wait One Frame So Physics Can Update With Disabled AI */
        yield return new WaitForFixedUpdate();

        /* Add force to This Rigid Body */
        pokemon.Rigidbody.AddForce(force, ForceMode.Acceleration);

        /* Enable Hurt Animation */
        pokemon.Anim.SetHurt(force);

        /* Wait Until Rigid Body Is Slowed Or For One Second */
        yield return new WaitUntilOrFor(() => pokemon.Rigidbody.velocity.magnitude < 0.4f, 1f);

        /* If Automatically Recovering */
        if (AutomaticRecovery)
        {
            /* Determine If Pokemon Is Out Of Its Designated Zone */
            bool outOfZone = Vector3.Distance(zone.transform.position, transform.position) > zone.bounds.extents.x;

            /* Disable Hurt Animation */
            pokemon.Anim.ClearHurt(outOfZone);

            /* If Pokemon Is Out Of Its Zone */
            if(outOfZone)
            {
                /* Wait For Delay */
                yield return new WaitForSeconds(delayAfterRecovery);

                /* Enable Nav AI and Use It To Go Pokemon To Waypoint Location */
                agent.enabled = true;
                agent.SetDestination(wayPoint.position);

                /* Wait Until Character Goes To Waypoint */
                yield return new WaitUntil(() => Vector3.Distance(transform.position, wayPoint.position) < 0.4f);

                /* Go Pokemon To Zone Location */
                agent.SetDestination(zone.transform.position);

                /* Wait Until Character Goes Back To Original Position */
                yield return new WaitUntil(() => Vector3.Distance(transform.position, zone.transform.position) < 0.4f);

                /* Disable Nav AI and Stop Movement Animation */
                agent.enabled = false;

                /* Disable Movement Animation */
                pokemon.Anim.EndMove();

            }
        }

        /* Set Hit Reaction To False */
        IsReactingToHit = false;

    }

}
