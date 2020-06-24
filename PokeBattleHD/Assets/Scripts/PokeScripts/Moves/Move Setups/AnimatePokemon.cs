using System.Collections;
using UnityEngine;

namespace Moves.Setup
{
    /// <summary> Animates this move's user using this move. </summary>
    [AddComponentMenu("Pokemon Moves/Move Setups/Animate Pokemon")]
    public class AnimatePokemon : MoveSetup
    {
        /// <summary> The animation clip of the pokemon using this move. </summary>
        public AnimationClip Clip { get; private set; }

        protected override void Awake()
        {
            /* Perform Base Awake Functionality */
            base.Awake();

            /* Foreach Move In The Pokemon's Animation Map */
            foreach (PokeAnimation moveAnimation in move.User.Anim.animationMap)
            {
                /* If The Pokemon's Move From The Animation Map Is This Move */
                if (moveAnimation.move.Name == move.Name)
                {
                    /* Return The Pokemon Move's Animation */
                    Clip = moveAnimation.clip;
                    return;
                }
            }

            /* Else, Warn User and Return Null */
            Debug.LogWarning(move.Name + " was unable to find a suitable animation in " + move.User.Name);

        }

        protected override IEnumerator OnBehaviorEnable()
        {
            /* Play The Pokemon's Move Animation */
            move.User.Anim.PlayAnimation(Clip);

            /* Wait Until The Move Animation Starts */
            yield return new WaitForSeconds(2);
        }
    }
}
