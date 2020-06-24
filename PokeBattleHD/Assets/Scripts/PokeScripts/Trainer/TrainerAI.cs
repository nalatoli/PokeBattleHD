using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary> Used to simulate AI for player's opponent. </summary>
[RequireComponent(typeof(Trainer))]
public class TrainerAI : MonoBehaviour
{
    /// <summary> The trainer that this AI is attached to. </summary>
    private Trainer trainer;

    private void Awake()
    {
        /* Get This Trainer */
        trainer = GetComponent<Trainer>();

    }

    /// <summary> Queues a pokemon from this trainer's party to summon. </summary>
    public void QueueNewPokemon()
    {
        /* Return A Pokemon With Some HP */
        foreach(PokemonInstanceStatus status in trainer.party) {
            if (status.state.hP > 0) {
                trainer.QueuedPokemon = status;
                return;
            }         
        }

        /* Return If Nothing Found */
        return;
    }

    /// <summary> Gets a list of all pokemon that are above 0 HP. </summary> 
    public List<PokemonInstanceStatus> GetUseablePokemon()
    {
        /* Initialize List */
        List<PokemonInstanceStatus> list = new List<PokemonInstanceStatus>();

        /* Add Pokemon With More Than 0 HP To List */
        foreach (PokemonInstanceStatus status in trainer.party)
            if (status.state.hP > 0)
                list.Add(status);

        /* Return List */
        return list;
    }


    /// <summary> 
    /// Gets the slot corrosponding to the move that deals the most
    /// damage to the opponent's pokemon. WIP
    /// </summary>
    /// <returns> Slot number of move to select. </returns>
    public int ChooseMoveSlot()
    {
        return 1;
    }


}
