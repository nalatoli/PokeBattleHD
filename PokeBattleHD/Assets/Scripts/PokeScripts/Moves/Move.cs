using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Moves.Setup;
using Moves.Effect;

namespace Moves
{
    /// <summary> 
    /// Pokemon move wrapper for a Gameobject that contains
    /// monobehavior components for behaviors, conditions, and effects.
    /// </summary>
    [AddComponentMenu("Pokemon Moves/Move")]
    public class Move : MonoBehaviour
    {
        #region Properties

        [Tooltip("Move name."), SerializeField]
        private new string name = "Dummy";
        /// <summary> Name of pokemon move. </summary>
        public string Name { get { return name; } }

        [Tooltip("Move type. Determines effectiveness against pokemon types."), SerializeField]
        private PokeType type = null;
        /// <summary> Type of pokemon move. </summary>
        public PokeType Type { get { return type; } }

        [Tooltip("Accuracy of move. " +
            "If set to 0, then this move will alwayas pass an accuracy check. " +
            "Use 0 for damage moves that always hit or non-targeted moves. "), Range(0,1), SerializeField]
        private float accuracy = 1;
        /// <summary> Accuracy of move. </summary>
        public float Accuracy { get { return accuracy; } }

        [Tooltip("Power points of move. Determines maximum amount of times a move can be used."), SerializeField]
        private int maxPp = 35;
        /// <summary> Maximum PP of move. </summary>
        public int MaxPP { get { return maxPp; } }

        /// <summary> Priority of move. </summary>
        [Tooltip("Priority of move. " +
            "Moves with higher priority then other moves " +
            "will always go before moves of lower priority."), Range(-7,7)]
        public int Priority = 0;

        [Tooltip("Description of move. This is for the UI and has no effect on gameplay."), TextArea, SerializeField]
        private string description = "";
        /// <summary> Description of move. </summary>
        public string Description { get { return description; } }

        [Tooltip("Move Effects' descriptions for UI. Every element will be a new field on the Move Data UI Panel."), SerializeField]
        private EffectDescription[] effectsDescriptions = null;
        /// <summary> Description of move effects. </summary>
        public EffectDescription[] EffectsDescriptions { get { return effectsDescriptions; } }

        /// <summary> Private PP field that backs public accessor. </summary>
        private int pP;

        /// <summary> Current PP of move. </summary>
        public int PP
        {
            get { return pP; }
            set { pP = value; EventManager.instance.PPChanged(this); }
        }

        /// <summary> Pokemon user of this move. </summary>
        public Pokemon User { get; private set; }

        /// <summary> Move behavior components attached to this gameObject. </summary>
        private MoveSetup[] setups;

        /// <summary> Move conditions components attached to this gameObject. </summary>
        private List<MoveObject> moveObjects = null;

        /// <summary> Whether move passed its accuracy check. </summary>
        public bool Missed { get; private set; }

        /// <summary> Whether move passed its crit check. </summary>
        public bool Crited { get; private set; }

        #endregion

        private void Awake()
        {
            /* Get All Move Setups */
            setups = GetComponents<MoveSetup>();

            /* Get All Move Objects */
            moveObjects = new List<MoveObject>();
            foreach (MoveSetup setup in setups)
                if (setup is Spawn)
                    moveObjects.Add((setup as Spawn).moveObject);                    

            /* Throw Error If This Move Is NOT Parented To Anything */
            if (transform.parent == null) {
                Debug.LogError(name + "'s parent is not parented to anything.", gameObject);
                return;
            }

            /* Get Pokemon Parent */
            User = transform.parent.GetComponent<Pokemon>();

            /* Throw Error If No Scripted Animation Controller Is Found */
            if (User == null)
                Debug.LogError(name + "'s parent is not a pokemon.", gameObject);

            /* Initialize Properties */
            Missed = false;
            Crited = false;

        }

        /// <summary> Enables all behaviors for this move. </summary>
        public IEnumerator Use()
        {
            /* Perform Accuracy/Crit Checks */
            Missed = accuracy != 0 && Random.value > accuracy;
            Crited = Random.value <= (1/16f);
            
            /* Enable All Behaviors */
            foreach (MoveSetup setup in setups)
                StartCoroutine(setup.Enable());

            /* Intialize Behavior Checker */
            bool stillEnabled = true;

            /* Loop Until All Behaviors Are Disabled */
            do
            {
                /* Wait, Then Reset Behavior Checker */
                yield return null;
                stillEnabled = false;

                /* Check If Any Behaviors Are Still Active */
                foreach (MoveSetup setup in setups)
                    stillEnabled |= setup.IsActive;

            } while (stillEnabled);

            /* Wait Until The Target Is Done Reacting To Hit */
            yield return new WaitUntil(() => User.CurrentTarget.Hits.IsReactingToHit == false);

        }

        public MoveSetup GetSetup<MoveSetup>()
        {
            return GetComponent<MoveSetup>();
        }

        public List<MoveEffect> GetEffects<MoveEffect>()
        {
            /* Initialize List Of Effects */
            List<MoveEffect> effects = new List<MoveEffect>();

            /* Add All Effects Of This Type From Move Objects Into Effect List */
            foreach(MoveObject moveObject in moveObjects)
                effects.AddRange(moveObject.GetComponents<MoveEffect>());
            
            /* Return Effects */
            return effects;
        }
    }
}

