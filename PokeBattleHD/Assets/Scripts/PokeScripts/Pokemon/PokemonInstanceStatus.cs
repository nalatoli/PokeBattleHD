using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;
using UnityEngine.Rendering;
using Moves;

[CreateAssetMenu(fileName = "NewState", menuName = "Pokemon/Pokemon State")]
public class PokemonInstanceStatus : ScriptableObject
{
    [Tooltip("The pokemon to use for this state.")]
    public Pokemon pokemon;

    [Tooltip("State of the pokemon.")]
    public PokemeonState state;

    /// <summary> Name to use for this unique pokemon. </summary>
    public string Name { get{ return (state.nickname.Length < 1) ? pokemon.name : state.nickname; } }
}

[System.Serializable]
public class PokemeonState
{
    /// <summary> Overrides species name. If empty, will NOT override. </summary>
    [Tooltip("Overrides species name. If empty, will NOT override.")]
    public string nickname;

    /// <summary> Gender. Affects certain moves, abilities, and breedability. </summary>
    [Tooltip("Gender. Affects certain moves, abilities, and breedability.")]
    public PokeGender gender = PokeGender.Female;

    /// <summary> Current level. Affects stats and experience gain. </summary>
    [Tooltip("Current level. Affects stats and experience gain."), Range(1, 100)]
    public int level = 50;

    /// <summary> Remaining HP as a ratio. </summary>
    [Tooltip("Remaining HP as a ratio."), Range(0, 1)]
    public float hP = 1;

    /// <summary> Remaining XP to full. Converts to actual XP when fetched. </summary>
    [Tooltip("Remaining XP to full. Converts to actual XP when fetched."), Range(0, 1)]
    public float xP = 0;
 
    [Tooltip("List of status effects for this pokemon.")]
    public List<StatusEffect> statusEffects;

    /// <summary> Move number 1. </summary>
    [Tooltip("Move number 1.")]
    public Move move1;

    /// <summary> Remaining PP for move number 2. Converts to actual PP when fetched. </summary>
    [Tooltip("Remaining PP for move number 1. Converts to actual PP when fetched."), Range(0, 1)]
    public float pP1;

    /// <summary> Move number 2. </summary>
    [Tooltip("Move number 2.")]
    public Move move2;

    /// <summary> Remaining PP for move number 2. Converts to actual PP when fetched. </summary>
    [Tooltip("Remaining PP for move number 2. Converts to actual PP when fetched."), Range(0, 1)]
    public float pP2;

    /// <summary> Move number 3. </summary>
    [Tooltip("Move number 3.")]
    public Move move3;

    /// <summary> Remaining PP for move number 3. Converts to actual PP when fetched. </summary>
    [Tooltip("Remaining PP for move number 3. Converts to actual PP when fetched."), Range(0, 1)]
    public float pP3;

    /// <summary> Move number 4. </summary>
    [Tooltip("Move number 4.")]
    public Move move4;

    /// <summary> Remaining PP for move number 4. Converts to actual PP when fetched. </summary>
    [Tooltip("Remaining PP for move number 4. Converts to actual PP when fetched."), Range(0, 1)]
    public float pP4;

    [Tooltip("Pokeball prefab reference that pokemon resides in.")]
    public Pokeball ball;

}


