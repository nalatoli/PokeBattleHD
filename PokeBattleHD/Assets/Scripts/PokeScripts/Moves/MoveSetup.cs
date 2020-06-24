using System.Collections;
using UnityEngine;

namespace Moves.Setup
{
    /// <summary>
    /// Setup property of this move. This is a behavior that does
    /// not directly affect the gameplay state, but instead sets up 
    /// MoveEffect behaviors.
    /// </summary>
    [RequireComponent(typeof(Move))]
    public abstract class MoveSetup : MonoBehaviour
    {
        [Tooltip("Sets timing before this setup behavior is executed"), SerializeField]
        private MoveTiming timing = null;

        /// <summary> The move that this behavior is tied to. </summary>
        protected Move move;

        /// <summary> Determines if behavior is still active </summary>
        public bool IsActive { get; private set; }

        /// <summary> Delay before behavior execution. </summary>
        private float delay;

        protected virtual void Awake()
        {
            /* Get Move Component For Use In Move Behaviors */
            move = GetComponent<Move>();
        }

        protected virtual void Start()
        {
            /* Get Timing */
            delay = timing.EvaluateTiming(GetComponent<AnimatePokemon>().Clip);
        }

        /// <summary> Enables a specific move behavior depending on the 'missed' flag.  </summary>
        public IEnumerator Enable()
        {
            /* Mark This Behavior As Active */
            IsActive = true;

            /* Wait Timing Delay */
            yield return new WaitForSeconds(delay);

            /* Start Behavior Depending On Accuracy Check */
            yield return OnBehaviorEnable();

            /* Mark This Behavior As Inactive */
            IsActive = false;

        }

        /// <summary> Behavior to perform. </summary>
        protected abstract IEnumerator OnBehaviorEnable();

    }
}
