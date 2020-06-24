using System.Collections;
using UnityEngine;

public class EyeController : MonoBehaviour
{
    [Header("Blink Settings")]

    /// <summary> Determines if random blinking will occur. </summary>
    [Tooltip("Determines if random blinking will occur.")]
    public bool EnableBlink = true;

    /// <summary> Determines if random blinking is enabled on this character's skinned mesh renderer. </summary>
    [Tooltip("Chance that this character will blink."), ConditionalHide("EnableBlink", false, 0, 1)]
    public float blinkChance = 0.01f;

    /// <summary> Minimum duration of blink. </summary>
    [Tooltip("Minimum duration of blink"), ConditionalHide("EnableBlink", false)]
    public float minBlinkDuration = 0.1f;

    /// <summary> Maximum duration of blink. </summary>
    [Tooltip("Maximum duration of blink"), ConditionalHide("EnableBlink", false)]
    public float maxBlinkDuration = 0.3f;

    [Header("Renderer Settings")]

    [Tooltip("The mesh with the eye material."), SerializeField]
    private SkinnedMeshRenderer smRenderer = null;

    [Tooltip("The slot number of the skinned mesh renderer's eye material."), SerializeField]
    private int eyeMaterialIndex = 0;

    [Tooltip("UV coordinates for the eye texture's 'eyes open' tile (for blinking)."), SerializeField]
    private Vector2 eyesOpenCoord = Vector2.zero;

    [Tooltip("UV coordinates for the eye texture's 'eyes closed' tile (for blinking)."), SerializeField]
    private Vector2 eyesClosedCoord = Vector2.zero;

    /// <summary> Determines if character is currently blinking. </summary>
    public bool Blinking { get; private set; }

    private void Update()
    {
        /* Return If Blinking Is Disabled Or Already Blinking */
        if (!EnableBlink || Blinking)
            return;

        /* Otherwise, Start Blinking If RNG Blesses */
        if (blinkChance >= Random.value)
            StartCoroutine(Blink(Random.Range(minBlinkDuration, maxBlinkDuration)));

    }

    /// <summary> Make eyes blink for specified duration. </summary>
    /// <param name="duration"> Duration of blink. </param>
    private IEnumerator Blink(float duration)
    {
        Blinking = true;
        smRenderer.materials[eyeMaterialIndex].SetTextureOffset("_BaseColorMap", eyesClosedCoord);
        yield return new WaitForSeconds(duration);
        smRenderer.materials[eyeMaterialIndex].SetTextureOffset("_BaseColorMap", eyesOpenCoord);
        Blinking = false;
    }
}