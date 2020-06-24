using UnityEngine;

namespace Moves.Effect
{
    /// <summary>
    /// Gameplay changing effect of this move.  Attached to
    /// move objects, as opposed to being attached to the move itself.
    /// </summary>
    [RequireComponent(typeof(MoveObject))]
    public abstract class MoveEffect : MonoBehaviour
    {
        /// <summary> Move that uses this effect. </summary>
        protected Move move;

        [Tooltip("Determines if this effect happens immedietly or on collision. " +
            "This move object must have a collider component with 'IsTrigger' enabled if this " +
            "setting is true."), SerializeField]
        private bool executeOnCollision = false;

        /// <summary> Application recorder to prevent multiple effect applications. </summary>
        private bool applied;

        /// <summary> Applies this effect. </summary>
        protected abstract void ApplyEffect();

        protected virtual void Awake()
        {

        }

        protected virtual void Start()
        {
            /* Get Move From Move Object */
            move = GetComponent<MoveObject>().Move;

            /* Initialize Application Recorder */
            applied = false;

            /* Apply Effect If Executing Immedietly */
            if (executeOnCollision == false)
                ApplyEffect();
        }

        private void OnTriggerEnter(Collider other)
        {
            /* Return If NOT Executing On Collision Or Effect Is Already Applied */
            if (executeOnCollision == false || applied == true)
                return;

            /* Otherwise Apply Effect And Mark That It Has Been Applied */
            ApplyEffect();
            applied = true;

        }
    }




}









