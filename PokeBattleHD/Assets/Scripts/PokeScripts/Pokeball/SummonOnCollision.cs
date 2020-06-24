using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SummonOnCollision : MonoBehaviour
{
    public Pokeball parentPokeball;
    bool instancedAPoke;
    private ParticleSystem part;
    private List<ParticleCollisionEvent> collisions;

    private void Start()
    {
        instancedAPoke = false;
        part = GetComponent<ParticleSystem>();
        collisions = new List<ParticleCollisionEvent>();
    }

    private void OnParticleCollision(GameObject other)
    {
        /* Return If A Pokemon Has Already been Instanced */
        if (instancedAPoke)
            return;

        /* Get Collision Data */
        part.GetCollisionEvents(other, collisions);

        /* Call Callback Function */
        StartCoroutine(parentPokeball.OnSummonContact(collisions[0].intersection));

        /* Record That Pokemon Has Been Instanced */
        instancedAPoke = true;

    }
}
