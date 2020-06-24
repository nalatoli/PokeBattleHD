using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FightUI : OptionMainField
{
    /// <summary> Reference to move state 1 in UI. </summary>
    [Tooltip("Reference to move state 1 in UI.")]
    [SerializeField] private MoveUI move1 = null;

    /// <summary> Reference to move state 2 in UI. </summary>
    [Tooltip("Reference to move state 2 in UI.")]
    [SerializeField] private MoveUI move2 = null;

    /// <summary> Reference to move state 3 in UI. </summary>
    [Tooltip("Reference to move state 3 in UI.")]
    [SerializeField] private MoveUI move3 = null;

    /// <summary> Reference to move state 4 in UI. </summary>
    [Tooltip("Reference to move state 4 in UI.")]
    [SerializeField] private MoveUI move4 = null;

    /// <summary> Load moves from a pokemon. Automatically fills the UI for moves. </summary>
    /// <param name="pokemon"> Pokemon to get moves from. </param>
    public void LoadMoves(Pokemon pokemon)
    {
        if (pokemon.Move(1)) move1.LoadState(pokemon.Move(1));
        else move1.ClearState();

        if (pokemon.Move(2)) move2.LoadState(pokemon.Move(2));
        else move2.ClearState();

        if (pokemon.Move(3)) move3.LoadState(pokemon.Move(3));
        else move3.ClearState();

        if (pokemon.Move(4)) move4.LoadState(pokemon.Move(4));
        else move4.ClearState();

    }

    public void ResetMoveUIs()
    {
        /* Clear Hover States */
        if (move1.IsLoaded) move1.SelectCircle(false);
        if (move2.IsLoaded) move2.SelectCircle(false);
        if (move3.IsLoaded) move3.SelectCircle(false);
        if (move4.IsLoaded) move4.SelectCircle(false);

        /* Turn Off Move UI */
        MovePanel.instance.SetVisibility(false);
    }


    public void EvaluateMoveslot(int slot)
    {
        /* Get Selected Move */
        MoveUI selectedMove;
        switch(slot)
        {
            case 1: selectedMove = move1; break;
            case 2: selectedMove = move2; break;
            case 3: selectedMove = move3; break;
            case 4: selectedMove = move4; break;
            default: Debug.LogError("Fight UI cannot evalulate move slot " + slot); return;
        }

        /* If Slot Is Loaded */ 
        if (selectedMove.IsLoaded)
        {
            /* Evaluate Moves and Disable All Options */
            BattleManager.instance.EvalulateMoveSelection(slot);
            OptionManager.instance.SetOptions(false);

        } 
    }
}
