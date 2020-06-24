using System.Collections;
using UnityEngine;

namespace Moves.Setup
{
    /// <summary> Camera targets this move's user. </summary>
    [AddComponentMenu("Pokemon Moves/Move Setups/Animate Camera Focus")]
    public class AnimateCameraFocus : MoveSetup
    {
        /// <summary> Determines if a custom animation is used for this move. </summary>
        [Tooltip("Determines if a custom animation is used for this move.")]
        public bool useCustomAnimation = false;

        /// <summary> Animation camera plays when camera focuses on the pokemon using this move. </summary>
        [Tooltip("Animation camera plays when camera focuses on the pokemon using this move."), ConditionalHide("useCustomAnimation", false)]
        public CameraAnimation cameraAnimation;

        /// <summary> Collider zone that move's user resides in. </summary>
        private Collider zone;

        protected override IEnumerator OnBehaviorEnable()
        {
            /* Get Zone Of User */
            if (zone == null)
                zone = move.User.transform.parent.GetComponent<Collider>();

            /* Play The Camera-Focus Animation */
            if (useCustomAnimation)
                CameraManager.instance.PlayAnimation(zone, cameraAnimation);
            else
                CameraManager.instance.Focus(move.User.Collider);

            /* Return Immedietly */
            yield break;

        }

    }
}
