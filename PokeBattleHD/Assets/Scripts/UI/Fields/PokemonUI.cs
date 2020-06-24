using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PokemonUI : OptionMainField
{
    /// <summary> Reference to PartyMembers in UI. </summary>
    [Tooltip("Reference to PartyMembers in UI.")]
    [SerializeField] private PartyMemberUI[] partyMembers = null;

    /// <summary> Load party from a trainer. Automatically fills in information. </summary>
    /// <param name="trainer"> Trainer to get the party from. </param>
    public void LoadParty(Trainer trainer)
    {
        int i = 0;
         
        for(; i < trainer.party.Length && i < 6; i++)
        {
            if (trainer.party[i] != null) partyMembers[i].LoadState(trainer.party[i]);
            else partyMembers[i].ClearState();
        }

        for(; i < 6; i++)
            partyMembers[i].ClearState();

    }


    public void EvaluatePartyslot(int slot)
    {
        if(slot < 1 || slot > 6) {
            Debug.LogError("Party slot " + slot + " is invalid.");
            return;
        }

        /* Evaluate Moves and Disable All Options */
        if(BattleManager.instance.EvaluateSwitchSelection(slot))
            OptionManager.instance.SetOptions(false);
        else
        {
            Debug.Log("bad boi");
        }

    }


}
