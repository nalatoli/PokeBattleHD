using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Moves;

public class EventManager : MonoBehaviour
{
    /* Public Instance */
    public static EventManager instance; 

    void Awake()
    {
        /* Initialize Instance (Throw Error If More Than One) */
        if (instance != null)
            Debug.LogWarning("More than one Event Manager in this scene.");
        else
            instance = this;

    }


    public event Action<Move> onPPChanged;
    public void PPChanged(Move move) { onPPChanged?.Invoke(move); }

    /// <summary> Actions to invoke when the player selects a move. </summary>
    public event Action<Pokemon> OnMoveSelect;
    /// <summary> Invoke actions that listen to player move selection. </summary>
    public void MoveSelected(Pokemon pokemon) { OnMoveSelect?.Invoke(pokemon); }

    /// <summary> Actions to invoke when the player selects a pokemon from its party. </summary>
    public event Action<Trainer> OnPokemonSelect;
    /// <summary> Invoke actions that listen to player party pokemon selection. </summary>
    public void PokemonSelected(Trainer trainer) { OnPokemonSelect?.Invoke(trainer); }



    /// <summary> Event that raises coroutines listeners sequentially before a turn's priority check. </summary>
    public RoutinesHandler BeforePriorityCheck = new RoutinesHandler();

    /// <summary> Event that raises coroutines listeners sequentially after a turn's priority check. </summary>
    public RoutinesHandler AfterPriorityCheck = new RoutinesHandler();

    /// <summary> Event that raises coroutines listeners sequentially when a turn has ended. </summary>
    public RoutinesHandler TurnEnd = new RoutinesHandler();


}


