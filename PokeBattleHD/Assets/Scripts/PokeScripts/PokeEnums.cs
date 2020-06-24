using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary> Defines sex of pokemon. Affects certain moves, abilities, and breedability. </summary>
public enum PokeGender
{
    None, Male, Female
}

/// <summary> Condition to call event. </summary>
public enum PokeEventType
{
    [Tooltip("Calls events right before a turn's priority check is performed.")]
    /// <summary> Calls events right before a turn's priority check is performed. </summary>
    beforePriorityCheck,

    [Tooltip("Calls events right after a turn's priority check is performed.")]
    /// <summary> Calls events right after a turn's priority check is performed. </summary>
    afterPriorityCheck,

    [Tooltip("Calls events right before a pokemon uses its move.")]
    /// <summary> Calls events right before a pokemon uses its move. </summary>
    beforeMove,

    [Tooltip("Calls events right after a pokemon uses its move.")]
    /// <summary> Calls events right after a pokemon uses its move. </summary>
    afterMove,

    [Tooltip("Calls events right before a turn ends.")]
    /// <summary> Calls events right before a turn ends. </summary>
    onTurnEnd,

}

/// <summary> Type of pokemon stat. </summary>
public enum StatType
{
    MaxHP, Attack, Defense, SpAttack, SpDefense, Speed
}

