using UnityEngine;

namespace Moves.Effect
{
    public class MoveObject : MonoBehaviour
    {
        /// <summary> Move tied to this on hit effect. </summary>
        public Move Move { get; set; }

        /// <summary> Recorder to prevent multiple effect applications. </summary>
        private bool applied;

        protected void Awake()
        {
            /* Initialize Application Recorder */
            applied = false;
        }



        protected void OnTriggerEnter(Collider reciever)
        {
            /* Return If Effect Applied */
            if (applied)
                return;

            /* Otherwise, Make Announcements */
        }
    }
}
