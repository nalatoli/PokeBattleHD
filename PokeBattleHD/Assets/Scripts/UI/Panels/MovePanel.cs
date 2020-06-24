using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Moves;

public class MovePanel : Panel
{
    /* Public Instance */
    public static MovePanel instance;

    /* Private Properties */
    private Move lastMove;

    protected override void Awake()
    {
        /* Perform Base Awake Function */
        base.Awake();

        /* Initialize Instance (Throw Error If More Than One) */
        if (instance != null)
            Debug.LogWarning("More than one Move Data UI in this scene.");
        else
            instance = this;

        /* Initialze Private Properties */
        lastMove = null;

    }

    /// <summary> Fills move data panel with information about a move. </summary>
    /// <param name="_move"> The move to show information about. </param>
    public void LoadState(Move _move)
    {
        /* Return If Move Already Loaded */
        if (_move == lastMove)
            return;

        /* Record Move So That Loading Doesn't Happen Uneccesarily */
        lastMove = _move;

        /* Fill Panel With Move Effecs */
        FillPanel(_move.Name, _move.EffectsDescriptions, _move.Description);

    }

}
