using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Moves.Effect
{
    [AddComponentMenu("Pokemon Moves/Move Effects/Color Skin")]
    public class ColorSkin : MoveEffect
    {
        [Tooltip("Target color of mesh."), ColorUsage(false, true)]
        public Color color = Color.white;

        [Tooltip("Speed at which skin color is changed.")]
        public float speedToColor = 3;

        [Tooltip("Duration of new color.")]
        public float duration = 1;

        [Tooltip("Speed at which skin color is cleared.")]
        public float speedToClear = 3;

        [Tooltip("True for color change on self. False for color change on target.")]
        public bool toSelf = true;

        protected override void ApplyEffect()
        {
            /* Get Recipient */
            Pokemon recipient = toSelf ? move.User : move.User.CurrentTarget;

            /* Change Color */
            recipient.StartCoroutine(ToColorAndBack(recipient, color, duration, speedToColor, speedToClear));
        }

        /// <summary> Change color of recipients mesh and back. </summary>
        /// <param name="recipient"> Pokemon to change mesh of. </param>
        /// <param name="target"> HDR color to set. </param>
        /// <param name="duration"> Duration of color exsistence. </param>
        /// <param name="riseSpeed"> Speed to reach target color. </param>
        /// <param name="fallSpeed"> Speed to clear target color. </param>
        /// <returns></returns>
        private IEnumerator ToColorAndBack(Pokemon recipient, Color target, float duration, float riseSpeed, float fallSpeed)
        {
            yield return recipient.SMRenderer.ToEmmisiveHDRColorOverTime(target, 1, riseSpeed);
            yield return new WaitForSeconds(duration);
            yield return recipient.SMRenderer.ClearEmmisionOverTime(fallSpeed);
        }
    }

}
