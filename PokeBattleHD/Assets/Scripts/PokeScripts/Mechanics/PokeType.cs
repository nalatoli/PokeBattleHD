using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

/// <summary> Determines effectiveness in certain matchups. </summary>
[CreateAssetMenu(fileName = "NewType", menuName = "Pokemon/Type")]
public class PokeType : ScriptableObject
{
    [Header("Cosmetic")]

    [Tooltip("Spritesheet to use for UI."), SerializeField]
    private List<Sprite> spritesheet = null;
    /// <summary> Spritesheet to use for UI. </summary>
    public List<Sprite> Spritesheet { get { return spritesheet; } }

    [Tooltip("Speed of spritesheet animation."), SerializeField, Range(0.1f, 2f)]
    private float spriteSheetSpeed = 1;
    /// <summary> Speed of spritesheet animation.. </summary>
    public float SpriteSheetSpeed { get { return spriteSheetSpeed; } }

    [Tooltip("Colors used for UI Text."), SerializeField]
    private VertexGradient textColorTheme = new VertexGradient
    {
        bottomLeft = Color.white,
        bottomRight = Color.white,
        topLeft = Color.white,
        topRight = Color.white
    };
    /// <summary> Colors used for UI Text. </summary>
    public VertexGradient TextColorTheme { get { return textColorTheme; } }

   [Tooltip("Color used for UI Border."), SerializeField]
    private Color borderColor = Color.white;
    /// <summary> Color used for UI Border. </summary>
    public Color BorderColor { get { return borderColor; } }


    [Header("Gameplay")]

    [Tooltip("When using a move of this type against a pokemon in any of these types, " +
        "the move will be super effective"), SerializeField]
    private PokeType[] strongAgainst = new PokeType[0];
    /// <summary> Pokemon types this type is strong against. </summary>
    public PokeType[] StrongAgainst { get { return strongAgainst; } }


    [Tooltip("When using a move of this type against a pokemon in any of these types, " +
        "the move will be not very effective."), SerializeField]
    private PokeType[] weakAgainst = new PokeType[0];
    /// <summary> Pokemon types this type is weak against. </summary>
    public PokeType[] WeakAgainst { get { return weakAgainst; } }

    [Tooltip("When using a move of this type against a pokemon in any of these types, " +
        "the move will not affect it."), SerializeField]
    private PokeType[] noEffectAgainst = new PokeType[0];
    /// <summary> Pokemon types this type has no efffect against. </summary>
    public PokeType[] NoEffectAgainst { get { return noEffectAgainst; } }

    /// <summary> Map of attack modifiers when using a move of this type against a pokemon. </summary>
    private readonly Dictionary<PokeType, float> mod = new Dictionary<PokeType, float>();

    private void OnEnable()
    {
        /* Add Types That This Move Type Is Strong Against To 'attackMod' With Values 1.5f */
        foreach (PokeType type in strongAgainst)
            AddType(mod, type, 1.5f);

        /* Add Types That This Move Type Is Weak Against To 'attackMod' With Values 0.5f */
        foreach (PokeType type in weakAgainst)
            AddType(mod, type, 0.5f);

        /* Add Types That This Move Type Has No Effect Against To 'attackMod' With Values 0 */
        foreach (PokeType type in noEffectAgainst)
            AddType(mod, type, 0f);

    }

    /// <summary> Modifier when using a move of this type against a pokemon. </summary>
    /// <param name="defender"> The defending pokemon. </param>
    /// <returns> Attack modifier depending on defender's types. </returns>
    public float EffectivnessMod(Pokemon defender)
    {
        /* Initialize Modifier */
        float currentMod = 1;

        /* Foreach type in Defender's Types */
        foreach (PokeType type in defender.Types)
        {
            /* If The Mod Dictionary Has This Type, Apply It To The Current Mod */
            if (mod.ContainsKey(type))
                currentMod *= mod[type];
        }
      
        /* Return The Final Attacking Modifier */
        return currentMod;
    }


    /// <summary> Modifier when a pokemon uses a move with a type that belong's to the Pokemon. </summary>
    /// <param name="attacker"> The attacking pokemon. </param>
    /// <returns></returns>
    public float STABMod(Pokemon attacker)
    {
        /* If The User Has This Type, Return 1.5 */
        foreach (PokeType type in attacker.Types)
            if (type == this)
                return 1.5f;

        /* Otherwise, Return 1 */
        return 1;
    }

    /// <summary> Adds type to modifer dictionary unless it is already present. </summary>
    /// <param name="mod"> Modifier dictionary. </param>
    /// <param name="type"> Type key to add. </param>
    /// <param name="value"> Value to corropond with type key. </param>
    private void AddType(Dictionary<PokeType, float> mod, PokeType type, float value)
    {
        if (!mod.ContainsKey(type))
            mod.Add(type, value);
        else
            Debug.LogError(type + " appears more than once in " + name + "!");
    }

}


