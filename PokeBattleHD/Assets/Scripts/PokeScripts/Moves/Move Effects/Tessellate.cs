using System.Collections;
using UnityEngine;

namespace Moves.Effect
{
    /// <summary> Change the terrain's height. </summary>
    [AddComponentMenu("Pokemon Moves/Move Effects/Tessellate")]
    public class Tessellate : MoveEffect
    {
        [Tooltip("Texture to base tessellation off of.")]
        public Texture2D texture;

        [Tooltip("Radius of tessellation.")]
        public uint radius;

        [Tooltip("Multiplier to rise tessellation. Can be negative for reverse tessellation.")]
        public float riseMultiplier;

        [Tooltip("Multiplier of fall tessellation. Can be negative for reverse tessellation.")]
        public float fallMultiplier;

        [Tooltip("True if tessellating at target. False for tessellating at self.")]
        public bool atTarget;

        protected override void ApplyEffect()
        {
            /* Tesselate */
            EnvironmentManager.TessellateTerrain(
                atTarget ? move.User.CurrentTarget.transform.position : move.User.transform.position,
                texture,
                (int)radius,
                riseMultiplier,
                fallMultiplier
                );
        }
    }
}

