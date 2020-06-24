using System.Collections;
using UnityEngine;

namespace Moves.Setup
{
    /// <summary> Camera targets this move's target. </summary>
    [AddComponentMenu("Pokemon Moves/Move Setups/Animate Camera Reaction")]
    public class AnimateCameraReaction : MoveSetup
    {
        /// <summary> Determines if a custom animation is used for this move. </summary>
        [Tooltip("Determines if a custom animation is used for this move.")]
        public bool useCustomAnimation = false;

        /// <summary> Animation camera plays when camera focuses on the pokemon reacting to this move. </summary>
        [Tooltip("Animation camera plays when camera focuses on the pokemon reacting to this move."), ConditionalHide("useCustomAnimation", false)]
        public CameraAnimation cameraAnimation;

        protected override IEnumerator OnBehaviorEnable()
        {
            /* If Move Missed */
            if (move.Missed)
            {
                /* Focus On Target */
                CameraManager.instance.Focus(
                    move.User.CurrentTarget.Collider,
                    Random.Range(0.4f, 0.6f),
                    Random.Range(0.4f, 0.6f),
                    Random.Range(0.70f, 0.80f));

                /* Print Miss */
                StartCoroutine(DialogueManager.Print("Missed" + '\n', false));

            }

            /* Else, Play This Move's Camera-Reaction Animation */
            else
            {
                if(useCustomAnimation)
                    CameraManager.instance.PlayAnimation(
                        move.User.CurrentTarget.transform.parent.GetComponent<Collider>(),
                        cameraAnimation);

                else
                    CameraManager.instance.Focus(move.User.CurrentTarget.Collider);
            }


            /* Return */
            yield break;

        }
    }
}
