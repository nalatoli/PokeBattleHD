using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ParticleSystemExtension
{
    /// <summary> Starts the particle system even if it hasn't been stopped. </summary>
    /// <param name="p"> Particle system to start. </param>
    public static void Restart(this ParticleSystem p)
    {
        p.Stop();
        p.Play();
    }
}
