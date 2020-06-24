using System.Collections;
using UnityEngine;

namespace Moves.Condition
{
    /// <summary>
    /// MoveEffect modifier thats gets attached to a MoveCondition.
    /// These modifiers perform depending on the result of its condition.
    /// </summary>
    public abstract class MoveModifier : ScriptableObject
    {
        /// <summary> The move that this condition result is tied to. </summary>
        protected Move move;

        /// <summary> Performs result of result depedning on sucess. </summary>
        /// <param name="success"> True for condition met, false if otherwise. </param>
        public abstract IEnumerator Perform(bool success);

        /// <summary> Initialize this effect influencer. </summary>
        protected abstract void Initialize();

        /// <summary> Initializes effect influencer. </summary>
        /// <param name="move"> The move that this effect influencer is tied to. </param>
        public void Initialize(Move move)
        {
            /* Assign Move */
            this.move = move;

            /* Perform Further Initialization */
            Initialize();

        }

    }
}

