using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Moves.Effect;

namespace Moves.Condition
{
    /// <summary> Modifies damage done on condition pass. </summary>
    [CreateAssetMenu(fileName = "Modify Damage Influencer", menuName = "Pokemon Moves/Move Influencers/Modify Damage")]
    public class ModifyDamage : MoveModifier
    {
        [Tooltip("Modifier amount. 1 means no modifcation.")]
        public int mod = 2;

        /// <summary> Determines if modification has already occured. </summary>
        private bool modified;

        /// <summary> List of all DealDamage effects on this move. </summary>
        private List<DealDamage> effects;

        protected override void Initialize()
        {
            /* Get All Deal Damage Effects On This Move */
            effects = move.GetEffects<DealDamage>();

            /* Initialzie Modification Recorder */
            modified = false;

        }

        public override IEnumerator Perform(bool success)
        {
            /* If Condition Is Met and Damage Is NOT Already Modified */
            if (success == true && modified == false)
            {
                /* Mark Modified As True */
                modified = true;

                /* Modifiy All Damage Effects */
                foreach (DealDamage effect in effects)
                    if (effect.type == DamageType.Regular) 
                        effect.power *= mod;
                    
                /* End Routine Before Next Check */
                yield break;

            }

            /* Otherwise, If Condition NOT Met and Damage Has Been Modified */
            if (success == false && modified == true)
            {
                /* Mark Modified As Untrue */
                modified = false;

                /* Restore All Damage Effects */
                foreach (DealDamage effect in effects)
                    if (effect.type == DamageType.Regular)
                        effect.power /= mod;
                
            }

            /* End Routine */
            yield break;

        }
    }
}
