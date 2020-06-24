using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Moves.Condition
{
    public class OnStatusEffectInhibition : MoveCondition
    {
        public override RoutinesHandler CheckOn => move.User.Events.BeforeMoveUse;

        public override EventPriorityLevel Priority => EventPriorityLevel.Level1;

        protected override bool Evalulate()
        {
            throw new System.NotImplementedException();
        }
    }
}

