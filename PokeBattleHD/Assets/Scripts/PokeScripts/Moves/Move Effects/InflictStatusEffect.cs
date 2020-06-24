using UnityEngine;

namespace Moves.Effect
{
    /// <summary> Attempts to inflict a status condition. </summary>
    [AddComponentMenu("Pokemon Moves/Move Effects/Inflict Status Effect")]
    public class InflictStatusEffect : MoveEffect
    {
        [Tooltip("Aliment to inflict.")]
        public StatusEffect statusEffect;

        [Tooltip("Chance that aliment is inflicted."), Range(0, 1)]
        public float chance = 0.3f;

        protected override void ApplyEffect()
        {
            /* Get Target */
            Pokemon target = move.User.CurrentTarget;

            /* Return If Move Doesn't Affect The Target */
            if (move.Type.EffectivnessMod(target) == 0)
                return;

            /* Return If RNG Doesn't Bless */
            if (Random.value > chance)
                return;

            /* If Status Effect Is Added, Announce Status Effect Affliction */
            if (move.User.CurrentTarget.StatusEffects.Add(statusEffect))
                DialogueManager.instance.StartCoroutine(DialogueManager.Print(statusEffect.name + " inflicted!" + '\n', false));

        }
    }
}
