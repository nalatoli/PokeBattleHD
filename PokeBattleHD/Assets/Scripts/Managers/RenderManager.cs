using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

[RequireComponent(typeof(Volume))]
public class RenderManager : MonoBehaviour
{
    /// <summary> Volume component. </summary>
    private static Volume volume;

    /// <summary> Screen transition post process volume override component. </summary>
    private static Transition transition;

    private void Awake()
    {
        /* Get Properties */
        volume = GetComponent<Volume>();
        volume.profile.TryGet(out transition);

        /* Intitialize Transition Propery */
        transition.intensity.value = 0;
    }

    /// <summary> Performs a screen transition by applying a overlay texture ontop of the screen. </summary>
    /// <param name="screenTransition"> The type of screen transition to perform. </param>
    /// <param name="startDelay"> The delay before the transition begins. </param>
    /// <param name="upSpeed"> The speed at which the transition progresses to its final point. </param>
    /// <param name="duration"> The time for staying at the final point in the transition. </param>
    /// <param name="downSpeed"> The speed at which the transition dissapates. </param>
    /// <param name="darken"> Whether or not the screen becomes black upon reaching the final point in the transition. </param>
    /// <param name="endDelay"> The time before ending the effect after going to original point. </param>
    public static IEnumerator DoScreenTransition(ScreenTransition screenTransition, float startDelay, float upSpeed, float duration, float downSpeed, bool darken, float endDelay)
    {
        /* Wait For Start Delay */
        yield return new WaitForSeconds(startDelay);

        /* Enable Effect */                                         // *** 
        transition.texture.value = GetTexture(screenTransition);    // Get texture for this transition.  
        transition.cutoffAlpha.value = 0;                           // Start transition at beginning.
        transition.darken.value = false;                            // Disable darken state.
        transition.intensity.value = 1;                             // Enable the transition.

        /* Gradually Increase Cutoff Alpha (Progresses Through Transition) */
        while (transition.cutoffAlpha.value != 1) {
            transition.cutoffAlpha.value = Mathf.MoveTowards(transition.cutoffAlpha.value, 1, Time.deltaTime * upSpeed);
            yield return null;
        }

        /* Wait For Duration */
        yield return new WaitForSeconds(duration);

        /* Turn Screen Black If Darken Is Enabled */
        if (darken)
            transition.darken.value = true;

        /* Gradually Decrease Cutoff Alpha (Progresses Backwards To Original Point) */
        while (transition.cutoffAlpha.value != 0) {
            transition.cutoffAlpha.value = Mathf.MoveTowards(transition.cutoffAlpha.value, 0, Time.deltaTime * downSpeed);
            yield return null;
        }

        /* Wait For End Delay */
        yield return new WaitForSeconds(endDelay);

        /* Disable Effect */
        transition.intensity.value = 0;

    }

    /// <summary> Gets texture corrosponding to specified transition. </summary>
    /// <param name="transition"> The screen transition to get the texture for. </param>
    /// <returns> The texture corrosponding to the screen transition. </returns>
    private static Texture2D GetTexture(ScreenTransition screenTransition)
        => Resources.Load<Texture2D>("Shaders/PostProcessing/Screen Transitions/" + screenTransition.ToString());
}

/// <summary> Possible screen effects for transitions. </summary>
public enum ScreenTransition
{
    SimulationEnd
}
