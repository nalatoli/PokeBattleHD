using UnityEngine;

namespace Moves.Effect
{
    /// <summary> Applies a force to a pokemon's rigidbody. </summary>
    [AddComponentMenu("Pokemon Moves/Move Effects/Apply Force")]
    public class ApplyForce : MoveEffect
    {
        [Tooltip("Local force that will applied to collision target.")]
        public Vector3 force;

        protected override void ApplyEffect()
        {
            /* If This Move Doesn't Effect The Target, Return Without Applying Force */
            if (move.Type.EffectivnessMod(move.User.CurrentTarget) == 0)
                return;

            /* Apply Force */
            move.User.CurrentTarget.Hits.ApplyForce(move.User.CurrentTarget.transform.TransformDirection(force));

        }

    }
}
