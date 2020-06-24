using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReturnOnCollision : MonoBehaviour
{
    public Pokeball parentPokeball;
    bool returnedAPoke;

    private void Start()
    {
        returnedAPoke = false;
    }

    private void OnParticleCollision(GameObject other)
    {
        /* Return If A Pokemon Has Already been Instanced */
        if (returnedAPoke)
            return;

        /* Call Callback Function */
        StartCoroutine(parentPokeball.OnReturnContact(other.GetComponentInParent<Pokemon>()));

        /* Record That Pokemon Has Been Instanced */
        returnedAPoke = true;

    }
}
