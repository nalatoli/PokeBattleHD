using System.Collections;
using UnityEngine;

namespace Moves.Condition
{
    /// <summary>
    /// Condition to be evaluated on an event-basis, 
    /// where the condition result will be passed to 
    /// this condition's influencers.
    /// </summary>
    [RequireComponent(typeof(Move))]
    public abstract class MoveCondition : MonoBehaviour
    {
        /// <summary> Event to check condition on. </summary>
        public abstract RoutinesHandler CheckOn { get; }

        /// <summary> Priority to place condition on. </summary>
        public abstract EventPriorityLevel Priority { get; }

        /// <summary> Influncers to execute based on condition result. </summary>
        [Tooltip("Influncers to execute based on condition result.")]
        public MoveModifier[] mods;

        /// <summary> The move that this condition is tied to. </summary>
        protected Move move;

        protected virtual void Awake()
        {
            /* Get Move Component For Use In Move Conditions */
            move = GetComponent<Move>();

            /* Initialize Influencers */
            foreach(MoveModifier mod in mods)
                mod.Initialize(move);

        }

        private void OnEnable()
        {
            CheckOn.Subscribe(Check, Priority);
        }

        /// <summary> Evalulates result of move condition. </summary>
        /// <returns> True if condition passed. False if condition failed. </returns>
        public IEnumerator Check()
        {
            /* Evaluate */
            bool result = Evalulate();

            /* Perform Influncer Actions Sequentially Using Result*/
            foreach (MoveModifier mod in mods)
                yield return mod.Perform(result);

        }

        /// <summary> Gets result of move condition. </summary>
        /// <returns> True if condition passed. False if condition failed. </returns>
        protected abstract bool Evalulate();

        private void OnDisable()
        {
            CheckOn.Unsubscribe(Check);
        }

    }

}
