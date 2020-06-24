using System.Collections;
using UnityEngine;

namespace Moves
{
    /// <summary> Controls sludgebomb visual effects. </summary>
    public class SludgeBomb : MonoBehaviour
    {
        /// <summary> Explosion particle system on impact. </summary>
        private ParticleSystem ps;

        /// <summary> Collider that detects impact. </summary>
        private GameObject visualCollider;

        /// <summary> Material using custom shader. </summary>
        private Material mat;

        /// <summary> Determines if sludgebomb has collided. Prevents repeat collisions. </summary>
        private bool collided;

        /// <summary> Resize routine tracker to interrupt resize routine. </summary>
        private IEnumerator routine;

        void Start()
        {     
            /* Get Components */
            visualCollider = transform.GetChild(0).gameObject;
            mat = visualCollider.GetComponent<Renderer>().material;
            ps = GetComponentInChildren<ParticleSystem>();

            /* Initilaize Property */
            collided = false;

            /* Scale Sludgebomb Up */
            visualCollider.transform.localScale = Vector3.zero;
            this.StartInterruptableCoroutine(CR_Resize(Vector3.one, 2.0f), ref routine);

        }


        /// <summary> Resizes sludgebomb over time to target size. </summary>
        /// <param name="size"> Target size. </param>
        /// <param name="time"> Speed factor. </param>
        private IEnumerator CR_Resize(Vector3 size, float time)
        {
            Vector3 scale = visualCollider.transform.localScale;

            /* Linearly Scale Until It Reaches Target Size */
            while (scale != size) {
                scale = Vector3.MoveTowards(scale, size, Time.deltaTime * time);          
                visualCollider.transform.localScale = scale;
                yield return null;

            }
        }

        /// <summary> Increases and decreases sludgebomb size and starts explosion particle system. </summary>
        private IEnumerator CR_Explode()
        {
            /* Make Material Pulsate As If Exploding */
            mat.SetFloat("timeScale", 5);
            mat.SetFloat("noiseConstraintFactor", 1.0f);

            /* Increase Size Of Sludgebomb */
            ps.Restart();
            yield return CR_Resize(new Vector3(2, 2, 2), 8.0f);
            StartCoroutine(CR_Resize(Vector3.zero, 16.0f));

        }

        private void OnTriggerEnter(Collider other)
        {
            if (!collided && !transform.IsChildOf(other.transform))
            {
                collided = true;
                this.StartInterruptableCoroutine(CR_Explode(), ref routine);
            }
        }
    }
}

