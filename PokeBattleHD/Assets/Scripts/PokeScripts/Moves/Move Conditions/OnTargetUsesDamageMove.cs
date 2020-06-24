using System.Collections.Generic;
using UnityEngine;
using Moves.Effect;

namespace Moves.Condition
{
    /// <summary> Passes if the target has a queued move with a DealDamage component. </summary>
    [AddComponentMenu("Pokemon Moves/Move Conditions/On Target Uses Damage Move")]
    public class OnTargetUsesDamageMove : MoveCondition
    {
        public override RoutinesHandler CheckOn => EventManager.instance.BeforePriorityCheck;

        public override EventPriorityLevel Priority => EventPriorityLevel.Level0;

        protected override bool Evalulate()
        {
            /* Get List Of DealDamage Effects On The Target Pokemon's Move */
            List<DealDamage> effects = move.User.CurrentTarget.CurrentMove.GetEffects<DealDamage>();

            /* Return Whether This List Contains More Than One Instance */
            return effects.Count > 0;
        }
    }
}




