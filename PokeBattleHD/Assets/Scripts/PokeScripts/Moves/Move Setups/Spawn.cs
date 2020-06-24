using System.Collections;
using UnityEngine;
using Moves.Effect;

namespace Moves.Setup
{
    /// <summary> Spawns a moveobject prefab with move effects. </summary>
    [AddComponentMenu("Pokemon Moves/Move Setups/Spawn")]
    public class Spawn : MoveSetup
    {
        /// <summary> Gameobject prefab to spawn. </summary>
        [Tooltip("Moveobject prefab to spawn.")]
        public MoveObject moveObject = null;

        /// <summary> Lifetime of instantiated object. </summary>
        [Tooltip("Lifetime of instantiated object")]
        public float objectLife = 0.2f;

        /// <summary> Determines if object is launched. </summary>
        [Tooltip("Determines if object is launched.")]
        public bool launch = false;

        /// <summary> How fast launch will be. </summary>
        [Tooltip("How fast launch will be."), ConditionalHide("launch", false)]
        public float launchSpeed = 1;

        /// <summary> Delay in launch. </summary>
        [Tooltip("Delay in launch."), ConditionalHide("launch", false)]
        public float launchDelay = 0;

        /// <summary> Where to spawn this object. </summary>
        [Tooltip("Where to spawn this object."), ConditionalHide("launch", true)]
        public SpawnEntity spawnAt;

        /// <summary> Parent body part of this spawned object. </summary>
        [Tooltip("Parent body part of this spawned object.")]
        public PokeBodyPart spawnParent;

        /// <summary> Spawned instance of prefab object. </summary>
        private MoveObject instance;

        protected override IEnumerator OnBehaviorEnable()
        {
            /* Don't Do Anything If Move Missed */
            if (move.Missed && spawnAt == SpawnEntity.target)
                yield break;

            /* Spawn Moveobject With This Move As The Parent */
            instance = SpawnInstance(moveObject);

            /* Orient Beam Emitter */
            move.User.Anim.beamEmission.EnableLockOn(move.User.CurrentTarget.Collider.bounds.center);

            if(launch)
            {
                yield return new WaitForSeconds(launchDelay);

                StartCoroutine(instance.transform.MoveOvertime(
                    move.User.CurrentTarget.Collider.bounds.center, 
                    launchSpeed, 
                    MoveOvertimeType.Linear));
            }

            /* Wait For Gameobject Lifetime */
            yield return new WaitForSeconds(objectLife);

            StopAllCoroutines();

            /* Disable Lock On For Beam Emitter */
            move.User.Anim.beamEmission.DisableLockOn();

            /* Destroy The Gameobject */
            Destroy(instance.gameObject);

        }

        /// <summary> Spawn a moveobject as a child of this move. </summary>
        /// <param name="moveObjectPrefab"> Moveobject prefab to spawn. </param>
        /// <returns> The instance of the moveobject. </returns>
        private MoveObject SpawnInstance(MoveObject moveObjectPrefab)
        {
            if(spawnAt == SpawnEntity.self || launch)
            {
                Transform parent = move.User.Anim.GetPart(spawnParent);

                /* Spawn Moveobject With This Move As The Parent */
                MoveObject o = Instantiate(moveObjectPrefab, parent.position, transform.rotation, parent);
                o.Move = move;
                return o;
            }    

            else
            {
                /* Spawn Moveobject With This Move As The Parent */
                MoveObject o = Instantiate(moveObjectPrefab, move.User.CurrentTarget.Anim.GetPart(spawnParent).position, transform.rotation, transform);
                o.Move = move;
                return o;
            }

        }

        /// <summary> Where to spawn a moveobject. </summary>
        public enum SpawnEntity
        {
            [Tooltip("At this move's user.")]
            self,

            [Tooltip("At this move's target.")]
            target,

        }
    }
}

