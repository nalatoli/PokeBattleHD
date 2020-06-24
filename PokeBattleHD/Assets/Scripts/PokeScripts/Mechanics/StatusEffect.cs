using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary> Determines effectiveness in certain matchups. </summary>
[RequireComponent(typeof(ParticleSystem))]
public class StatusEffect : MonoBehaviour
{
    [Tooltip("Animated spritesheet that UI displays when pokemon is inflicted with this aliment."), SerializeField]
    private List<Sprite> sprites = null;
    /// <summary> Animated spritesheet that UI displays when pokemon is inflicted with this aliment. </summary>
    public List<Sprite> Sprites { get { return sprites; } }

    [Tooltip("Background color of sprites."), SerializeField]
    private Color borderColor = Color.gray;
    /// <summary> Background color of sprites. </summary>
    public Color BorderColor { get { return borderColor; } }

    [Tooltip("Description of aliment. This is for the UI and has no effect on gameplay."), TextArea, SerializeField]
    private string description = "";
    /// <summary> Description of aliment. </summary>
    public string Description { get { return description; } }

    [Tooltip("Aliment's effects' descriptions for UI. Every element will be a new field on the Aliment Data UI Panel."), SerializeField]
    private EffectDescription[] effectsDescriptions = null;
    /// <summary> Description of aliment effects. </summary>
    public EffectDescription[] EffectsDescriptions { get { return effectsDescriptions; } }

}


