using UnityEngine;

namespace Moves.Setup
{
    /// <summary> Handles timing of move setups. </summary>
    [System.Serializable]
    public class MoveTiming
    {
        /// <summary> The timing of this move behavior. </summary>
        [Tooltip("The timing of this move behavior."), SerializeField]
        private MoveTimingType type = MoveTimingType.Immedietly;

        /// <summary> Seconds to wait before behavior execution. </summary>
        [Tooltip("Seconds to wait before behavior execution."), ConditionalHide("type", (int)MoveTimingType.AfterDelay), SerializeField]
        private float delay = 1;

        /// <summary> The number event to enable this behavior. </summary>
        [Tooltip("The event number to enable this behavior. " +
            "If set to 1, this behavior will enable on the first move event " +
            "of a pokemon's corrosponding move animation"), 
            ConditionalHide("type", (int)MoveTimingType.OnAnimationEvent), SerializeField]
        private uint eventNumber = 1;

        /// <summary> The number event to enable this behavior. </summary>
        [Tooltip("Seconds offset from event to execute behavior. "),
            ConditionalHide("type", (int)MoveTimingType.OnAnimationEvent), SerializeField]
        private float offset = 0;

        /// <summary> Gets a delay in behavior execution depening on the timing type. </summary>
        /// <returns> Seconds to wait before behavior execution. </returns>
        public float EvaluateTiming(AnimationClip clip)
        {
            switch(type) {
                case MoveTimingType.Immedietly: return 0;
                case MoveTimingType.AfterDelay: return delay;
                case MoveTimingType.OnAnimationEvent: return EvalulateMoveEvent(clip) + offset;
                default: Debug.LogError(type + " is NOT a valid move timing!");  return 0;
            }
        }

        /// <summary> 
        /// Returns a time in which an animation event 
        /// (dictated by 'eventNumber') is called in a pokemon's move animation.
        /// </summary>
        /// <returns> The time until the animation event of this pokemon's move animation is triggered. </returns>
        private float EvalulateMoveEvent(AnimationClip clip)
        {
            /* Warn User and Return 0 If NO Clip is Found */
            if (clip == null) {
                Debug.LogWarning("Can't evalulate move event. Unable to find a suitable animation.");
                return 0;
            }

            /* Warn User If No Move Events On Animation Clip */
            if (clip.events.Length == 0) {
                Debug.LogError("No move events on " + clip);
                return 0;
            }

            /* Initalize Marker For Current Move Event Count */
            int count = 0;

            /* For All Animation Events in The Pokemon's Move Animation */
            foreach (AnimationEvent moveEvent in clip.events)
            {
                /* If This Event Is A Move Event */
                if (moveEvent.functionName == "MarkMoveEvent")
                {
                    /* Increment The Count, Then If The Count = The Event Number */
                    if (++count == eventNumber)
                    {
                        /* Return The Time In The Animation For This Event */
                        return moveEvent.time;
                    }
                }
            }

            /* Warn User Since The Count Is Smaller Than The Event Number */
            Debug.LogWarning(clip + " only has " + clip.events.Length + ", but is asked for event # " + eventNumber);
            return 0;

        }

        /// <summary> Timing of a move setup property. </summary>
        public enum MoveTimingType
        {
            /// <summary> This behavior will act immedietly upon move performance. </summary>
            [Tooltip("This behavior will act immedietly upon move performance.")]
            Immedietly,

            /// <summary> This behavior will act some time after move performance start. </summary>
            [Tooltip("This behavior will act some time after move performance start.")]
            AfterDelay,

            /// <summary> "This behavior will act at a specified animation event in the pokemon's animation. </summary>
            [Tooltip("This behavior will act at a specified animation event in the pokemon's animation..")]
            OnAnimationEvent,

        }
    }
}



