using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Moves;

[System.Serializable]
public class PokeAnimation
{
    /* Public Properties */
    /// <summary> Move that plays this clip. </summary>
    [Tooltip("Move that plays this clip.")]
    public Move move;

    /// <summary> Clip pokemon plays when move is used. </summary>
    [Tooltip("Clip pokemon plays when move is used.")]
    public AnimationClip clip;
}
