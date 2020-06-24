using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
/// <summary> 
/// Manage the state of the battle and other managers. 
/// </summary>
public class BattleManager : MonoBehaviour
{
    #region Properties

    /// <summary> Control battle flow. </summary>
    public static BattleManager instance;

    /// <summary> Determine if start sequence should be skipped. </summary>
    [Tooltip("Determine if start sequence should be skipped. " +
        "The battle will start at the options loop as if the battle" +
        "has been initialized normally.")]
    public bool skipSummons = false;

    [Tooltip("Determines if the camera will focus on a battler. " +
        "This mode is ideal for creating camera animations.")]
    public bool editCamAnim = false;

    [Tooltip("Animation to edit"), ConditionalHide("editCamAnim", false)]
    public CameraAnimation camAnim;

    /// <summary> Song to play when battle starts. </summary>
    [Tooltip("Song to play when battle starts.")]
    public Music song;

    /// <summary> Location in the middle of the two spawn middle. </summary>
    [Tooltip("Location in the middle of the two spawn middle.")]
    public Collider center;

    /// <summary> Spawn settings for battler 1. </summary>
    [Tooltip("Spawn settings for battler 1.")]
    public SpawnSettings spawn1;

    /// <summary> Spawn settings for player 2. </summary>
    [Tooltip("Spawn settings for player 2.")]
    public SpawnSettings spawn2;

    /// <summary> Reference to player battler. </summary>
    public Battler Battler1 { get; private set; }

    /// <summary> Reference to AI battler. </summary>
    public Battler Battler2 { get; private set; }

    /// <summary> Option routine tracker in order to start and stop option sequence loop. </summary>
    private Coroutine optionRoutine;

    #endregion

    private void Awake()
    {
        /* Initialize Instance (Throw Error If More Than One) */
        if (instance != null)
            Debug.LogWarning("More than one Battle Flow Manager in this scene.");
        else
            instance = this;
    }

    private void Start()
    {
        /* Initialize Battle Depending On Skip Summons Flag */
        StartCoroutine(skipSummons ? InitializeBattleInstantly() : InitializeBattle());
    }


    /// <summary>
    /// Initializes the battle state. This routine does the following in order.
    /// 1. Sets UI state so only dialogue box is showing.
    /// 2. Spawns players.
    /// 3. Starts battle music.
    /// 4. Announces battler 2 as a challenger.
    /// 4. Performs Battler 2's summon.
    /// 6. Performs Battler 1's (player's) summon.
    /// 7. Loads UI states for player.
    /// 8. Gives control to the options sequence.
    /// </summary>
    private IEnumerator InitializeBattle()
    {
        /* Initialize UI Settings */
        DialogueManager.SetTextVisibility(false);
        OptionManager.instance.SetPokeStates(false);
        OptionManager.instance.SetOptions(false);

        /* Generate Battlers */
        Battler1 = new Battler(spawn1);
        Battler2 = new Battler(spawn2);

        /* Start Battle Theme with Crowd Cheering */
        SoundManager.PlaySound("Crowd");
        SoundManager.PlayMusic(song);

        /* Announce Challenger and Focus On Him/Her */
        CameraManager.instance.Pan(Battler2.trainer.Collider, 0,0, 0.5f, 0.1f);
        yield return DialogueManager.PrintAndWait(Battler2.trainer.name + " would like to battle...", true);

        /* Perform Challenger's Summon */
        Battler2.trainer.QueuePartyMember(1);
        yield return CommandSummon(Battler2);

        /* Perform Player's Summon */
        Battler1.trainer.QueuePartyMember(1);
        yield return CommandSummon(Battler1);

        /* Perform Pokemon State UI Initial Loads */
        OptionManager.instance.state1.LoadState(Battler1.pokemon);
        OptionManager.instance.state2.LoadState(Battler2.pokemon);

        /* Perform Pokemon Move/Party UI Initial Loads */
        OptionManager.instance.LoadMoves(Battler1.pokemon);
        OptionManager.instance.LoadParty(Battler1.trainer);

        /* Proceed To Option Loop */
        optionRoutine = StartCoroutine(SetOptionLoop());

    }

    /// <summary> 
    /// Instantly performs battle initialization. 
    /// This function does the following on the same frame:
    /// 1. Disables visibility of all UI.
    /// 2  Generates new battlers.
    /// 3. Starts music.
    /// 4. Makes battlers instantly summons new pokemon.
    /// 5. Loads Pokemon state UIs
    /// 6. Loads Move/Party UIs
    /// </summary>
    private IEnumerator InitializeBattleInstantly()
    {
        /* Initialize UI Settings */
        DialogueManager.SetTextVisibility(false);
        OptionManager.instance.SetPokeStates(false);
        OptionManager.instance.SetOptions(false);

        /* Generate Battlers */
        Battler1 = new Battler(spawn1);
        Battler2 = new Battler(spawn2);

        /* Start Music */
        SoundManager.PlayMusic(song);

        /* Instantly Summon Pokemon */
        Battler1.trainer.SummonInstant(0, Battler1.pokeSpawner.transform);
        Battler2.trainer.SummonInstant(0, Battler2.pokeSpawner.transform);
        Battler1.pokemon = Battler1.trainer.CurrentPokemon;
        Battler2.pokemon = Battler2.trainer.CurrentPokemon;

        /* Let Pokemon Awake/Start Calls Be Performed */
        yield return null;

        /* Load Pokemon State UIs */
        OptionManager.instance.state1.LoadState(Battler1.pokemon);
        OptionManager.instance.state2.LoadState(Battler2.pokemon);

        /* Load Move/Party UIs */
        OptionManager.instance.LoadMoves(Battler1.pokemon);
        OptionManager.instance.LoadParty(Battler1.trainer);

        /* Start Options Loop */
        optionRoutine = StartCoroutine(SetOptionLoop());

    }

    /// <summary>
    /// Enables the options UI and loops scenic camera animations.
    /// This routine only enables the UI. All control is given to
    /// UI elements through event triggers.
    /// This routine ends only when an evaluation from the options
    /// results in a BattleManager performance.
    /// </summary>
    private IEnumerator SetOptionLoop()
    {   
        /* Disable Dialogue and Enable Options and Pokemon State UIs */
        DialogueManager.SetTextVisibility(false);

        /* Setup Camera Editing Mode If Enabled */
        if(editCamAnim == true) {
            CameraManager.instance.PlayAnimation(Battler2.pokeSpawner, camAnim);
            yield return new WaitUntil(() => editCamAnim == false);
        }

        /* Unhide Options UI */
        OptionManager.instance.SetOptions(true);
        OptionManager.instance.SetPokeStates(true);

        /* Update Player's UI */
        OptionManager.instance.LoadParty(Battler1.trainer);

        /* Perform Scenic Camera Animations Until This Routine Is Externally Stopped */
        while (true)
        {
            /* Do Zoomed Out Center Pan */
            CameraManager.instance.Pan(center,
                Random.Range(-5, 0),
                Random.Range(7, 13),
                Mathf.Sign(Random.Range(-1, 1)),
                Random.Range(0, 1));
            yield return new WaitForSeconds(5);

            /* Do Pan Of Battler 1 */
            CameraManager.instance.Pan(Battler1.pokemon.Collider);
            yield return new WaitForSeconds(5);

            /* Do Pan Of Battler 2 */
            CameraManager.instance.Pan(Battler2.pokemon.Collider);
            yield return new WaitForSeconds(5);

        }
    }

    /// <summary> 
    /// Focuses on pokemon and announces an effect in dialogue box. 
    /// Camera properties and wait time are automatically assigned. 
    /// </summary>
    /// <param name="target"> Pokemon to focus on.</param>
    /// <param name="announcment"> Announcment to make. </param>
    /// <param name="clear"> True to clear text. False will leave text as is. </param>
    public static IEnumerator Announce(Pokemon target, string announcment, bool clear)
    {
        CameraManager.instance.Focus(target.Collider);
        yield return DialogueManager.Print(announcment, clear);
        yield return new WaitForSeconds(1);
    }

    /// <summary> Focuses on pokemon and announces an effect in dialogue box. </summary>
    /// <param name="target"> Pokemon to focus on.</param>
    /// <param name="announcment"> Announcment to make. </param>
    /// <param name="clear"> True to clear text. False will leave text as is. </param>
    /// <param name="distanceOffset"> Camera's distance offset from target. 0 is no additional offset. (+) is further away. </param>
    /// <param name="verticalOffset"> Camera's vertical offset from target. 0 is no additional offset. (+) is further up. </param>
    /// <param name="timeOffset"> Camera's normalized Time in animation to start, from [0,1]. 0 is facing the back of target and 0.5 is facing the front. </param>
    /// <param name="waitTime"> Time to wait after announcment is finished. </param>
    public static IEnumerator Announce(Pokemon target, string announcment, bool clear, float distanceOffset, float verticalOffset, float timeOffset, float waitTime)
    {
        CameraManager.instance.Focus(target.Collider, distanceOffset, verticalOffset, timeOffset);
        yield return DialogueManager.Print(announcment, clear);
        yield return new WaitForSeconds(waitTime);
    }

    /// <summary> Initates move evalulation based on user's selection. </summary>
    /// <param name="slot"> User's move slot selection of player. </param>
    /// <returns> True if slot yields a valid move, false otherwise. </returns>
    public bool EvalulateMoveSelection(int slot)
    {
        /* Queue Moves On Pokemon */
        Battler1.pokemon.QueueMove(slot, Battler2.pokemon);
        
        /* Call All Events That Listen To Move Selection */
        EventManager.instance.MoveSelected(Battler1.pokemon);

        /* Return Failure If The Player's Move Was Nullified By Event */
        if (Battler1.pokemon.CurrentMove == null)
            return false;

        /* Otherwise, Disable Options, Queue Move On Challenger's Pokemon, Execute Turn, and Return Success */
        StopCoroutine(optionRoutine);
        Battler2.pokemon.QueueMove(Battler2.trainer.AI.ChooseMoveSlot(), Battler1.pokemon);
        StartCoroutine(PerformTurn());
        return true;

    }

    /// <summary> Initiates switch evaluation based on user's selection. </summary>
    /// <param name="slot"> User's party slot selection from 1 -> 6 . </param>
    /// <returns> True if switch can be performed, false otherwise. </returns>
    public bool EvaluateSwitchSelection(int slot)
    {
        /* Return Failure If Trainer's Target Pokemon Is The Trainer's Current Pokemon */
        if (Battler1.trainer.party[slot - 1].Name == Battler1.trainer.CurrentPokemon.Name)
            return false;

        /* Otherwise, Queue Pokemon Selection */
        Battler1.trainer.QueuePartyMember(slot);

        /* Call Events That Listen To A Pokemon Selection Being Made */
        EventManager.instance.PokemonSelected(Battler1.trainer);

        /* If Trainer's Current Pokemon Was Nullified, Return Failure */
        if (Battler1.trainer.QueuedPokemon == null)
            return false;

        /* Otherwise, Stop Option Loop, Perform Switch, and Return Success */
        StopCoroutine(optionRoutine);
        StartCoroutine(CommandPlayerSwitch());
        return true;

    }

    /// <summary>
    /// Starts turn performance sequence. 
    /// After each process, there will be checks to determine how the routine should proceed.
    /// This routine does the following proocesses in order:
    /// 1. Determines which pokemon should go first.
    /// 2. Calls all 'Turn Start' events from both pokemon.
    /// 3. Calls all 'BeforeMove' events for first pokemon.
    /// 4. Performs first pokemon's move.
    /// 5. Call all 'AfterMove' events for first pokemon.
    /// 6. Calls all 'BeforeMove' events for second pokemon.
    /// 7. Performs second pokemon's move.
    /// 8. Call all 'AfterMove' events for second pokemon.
    /// 9. Calls all 'Turn End' events from both pokemon.
    /// </summary>
    private IEnumerator PerformTurn()
    {
        /* Disable Random Animation */
        Battler1.pokemon.Anim.EnableRandomAnimation = false;
        Battler2.pokemon.Anim.EnableRandomAnimation = false;

        /* Call 'Before Priority Check' Events */
        yield return EventManager.instance.BeforePriorityCheck.Raise();
        yield return CheckHP();

        /* Get An Array Of Battlers That Represents The Order Of Which To Use Moves */
        Battler[] battlers = GetPriorityCheckResult();

        /* Call 'After Priority Check' Events */
        yield return EventManager.instance.AfterPriorityCheck.Raise();
        yield return CheckHP();

        /* Call All Events That Occur Before The First Pokemon Uses Its Move If Its Move Isnt Null */
        if (battlers[0].pokemon.CurrentMove != null) {
            yield return battlers[0].pokemon.Events.BeforeMoveUse.Raise();
            yield return CheckHP();
        }

        /* If First Pokemon's Move Is NOT null */
        if (battlers[0].pokemon.CurrentMove != null)
        {
            /* Announce First Pokemon's Move */
            yield return Announce(battlers[0].pokemon, battlers[0].pokemon.Name + " used " + battlers[0].pokemon.CurrentMove.Name + ".", true);

            /* Disable Dialogue Box */
            DialogueManager.SetTextVisibility(false);

            /* Use First Pokemon's Move */
            yield return battlers[0].pokemon.UseMove();
            yield return CheckHP();

            /* Call All Events That Occur After This Pokemon Uses Its Move */
            yield return battlers[0].pokemon.Events.AfterMoveUse.Raise();
            yield return CheckHP();

        }

        /* Call All Events That Occur Before The Second Pokemon Uses Its Move If Its Move Isnt Null */
        if (battlers[1].pokemon.CurrentMove != null) {
            yield return battlers[1].pokemon.Events.BeforeMoveUse.Raise();
            yield return CheckHP();
        }

        /* If First Pokemon's Move Is NOT null */
        if (battlers[1].pokemon.CurrentMove != null)
        {
            /* Announce First Pokemon's Move */
            yield return Announce(battlers[1].pokemon, battlers[1].pokemon.Name + " used " + battlers[1].pokemon.CurrentMove.Name + ".", true);

            /* Disable Dialogue Box */
            DialogueManager.SetTextVisibility(false);

            /* Use First Pokemon's Move */
            yield return battlers[1].pokemon.UseMove();
            yield return CheckHP();

            /* Call All Events That Occur After This Pokemon Uses Its Move */
            yield return battlers[1].pokemon.Events.AfterMoveUse.Raise();
            yield return CheckHP();

        }

        /* Call 'After Turn' Events */
        yield return EventManager.instance.TurnEnd.Raise();
        yield return CheckHP();

        /* Go back To Options Loop */
        Battler1.pokemon.Anim.EnableRandomAnimation = true;
        Battler2.pokemon.Anim.EnableRandomAnimation = true;
        Battler1.pokemon.transform.ResetLocal();
        Battler2.pokemon.transform.ResetLocal();
        optionRoutine = StartCoroutine(SetOptionLoop());

    }

    /// <summary> 
    /// Get the result of a priority check between two pokemon.
    /// The priority check returns an array of battlers where battlers use
    /// their moves from index 0 onward. The check consists of:
    /// 1. Checking if moves are null.
    /// 1. Checking move priority.
    /// 2. Checking speed.
    /// 3. RNG.
    /// </summary>
    /// <returns> Index 0 is the first battler to perform. </returns>
    private Battler[] GetPriorityCheckResult()
    {
        /* If Move 1 Is Null, Return 2 -> 1 */
        if (Battler1.pokemon.CurrentMove == null)
            return new Battler[] { Battler2, Battler1 };

        /* If Move 2 Is Null, Return 1 -> 2 */
        if (Battler2.pokemon.CurrentMove == null)
            return new Battler[] { Battler1, Battler2 };

        /* If Move 1 Has Higher Priority Than Move 2, Return 1 -> 2 */
        if (Battler1.pokemon.CurrentMove.Priority > Battler2.pokemon.CurrentMove.Priority)
            return new Battler[] { Battler1, Battler2 };

        /* Otherwise, If Move 2 Has Higher Priority Than Move 1, Return 2 -> 1 */
        if (Battler1.pokemon.CurrentMove.Priority < Battler2.pokemon.CurrentMove.Priority)
            return new Battler[] { Battler2, Battler1 };

        /* Otherwise, If Pokemon 1 Is Faster Than Pokemon 2, Return 1 -> 2 */
        if (Battler1.pokemon.Speed.Value > Battler2.pokemon.Speed.Value)
            return new Battler[] { Battler1, Battler2 };

        /* Otherwise, If Pokemon 2 Is Faster Than Pokemon 1, Return 2 -> 1 */
        if (Battler1.pokemon.Speed.Value < Battler2.pokemon.Speed.Value)
            return new Battler[] { Battler2, Battler1 };

        /* Otherwise, If RNG Blesses, Return 1 -> 2 */
        if (Random.value <= 0.5f)
            return new Battler[] { Battler1, Battler2 };

        /* Otherwise, Return 2 -> 1 */
        return new Battler[] { Battler2, Battler1 };

    }

    /// <summary> Checks HP during a turn performance. If an HP drops to 0, a switch is commenced. </summary>
    private IEnumerator CheckHP()
    {
        /* Return Pokemon 1 If Fainted */
        if (Battler1.pokemon.Hp == 0)
            yield return GetNewPlayerPokemon();


        /* Return Pokemon 1 If Fainted */
        if (Battler2.pokemon.Hp == 0)
            yield return GetNewChallengerPokemon();
       
    }

    /// <summary> Commands a summon of a battler's queued pokemon. </summary>
    /// <param name="battler"> The battler to command a summon from. </param>
    private IEnumerator CommandSummon(Battler battler)
    {
        /* Announce Selection and Focus On Ball */
        battler.trainer.StartCoroutine(DialogueManager.Print(battler.trainer.name + " chose " + battler.trainer.QueuedPokemon.Name, true));
        CameraManager.instance.PlayClip(battler.trainer.Anim.ballThrowAnimation, battler.trainer.transform);

        /* Perform Summon and Get Reference To New Pokemon */
        yield return battler.trainer.Summon(battler.pokeSpawner.transform);
        battler.pokemon = battler.trainer.CurrentPokemon;

        /* Focus On Pokemon and Wait */
        CameraManager.instance.Pan(battler.pokemon.Collider);
        yield return new WaitForSeconds(3);

    }

    /// <summary> Commands return of battler's current pokemon. </summary>
    /// <param name="battler"> The battler to command a switch from. </param>
    private IEnumerator CommandReturn(Battler battler)
    {
        /* Return Pokemon */
        battler.pokemon = null;
        yield return battler.trainer.Return();

    }

    /// <summary> Command a switch of the player's pokemon to the player's queued pokemon. </summary>
    private IEnumerator CommandPlayerSwitch()
    {
        /* Command Return If Pokemon NOT Fainted */
        if (Battler1.trainer.CurrentPokemon != null)
        {
            CameraManager.instance.Pan(Battler1.pokeSpawner, -1.3f, 1.4f, 0.2f, Random.Range(0.1f, 0.2f));
            StartCoroutine(DialogueManager.Print(Battler1.trainer.name + " is returning " + Battler1.pokemon.Name, false));
            yield return CommandReturn(Battler1);
        }
            
        /* Command Summon Of Queud Pokemon */
        yield return CommandSummon(Battler1);

    }

    /// <summary> Switch from fainting for player. </summary>
    private IEnumerator GetNewPlayerPokemon()
    {
        /* Perform Faint Animation */
        Battler1.pokemon.Anim.Faint();

        /* Announce Faint */
        SoundManager.PlaySound("Crowd");
        CameraManager.instance.Focus(Battler1.pokeSpawner, -1.3f, 1.4f, Random.Range(0.1f, 0.2f));
        StartCoroutine(DialogueManager.Print(Battler1.pokemon.Name + " has fainted!", true));
        yield return new WaitForSeconds(1f);

        /* Return Player's Fainted Pokemon */
        yield return CommandReturn(Battler1);

        /* Terminate Battle If Player Has No More Useable Pokemon */
        if(Battler1.trainer.AI.GetUseablePokemon().Count == 0)
            yield return TerminateBattle(Battler2);

        /* Wait For Player To Select New Pokemon */
        optionRoutine = StartCoroutine(SetOptionLoop());
        OptionManager.instance.ForcePokemonSelection();

        /* Wait Until Pokemon Is Summoned */
        yield return new WaitUntil(() => Battler1.pokemon != null);

        /* Load Pokemon State UI */
        OptionManager.instance.state1.LoadState(Battler1.pokemon);
        OptionManager.instance.LoadMoves(Battler1.pokemon);

        /* Focus On Pokemon and Wait */
        CameraManager.instance.Pan(Battler1.pokemon.Collider);
        yield return new WaitForSeconds(3);

    }

    /// <summary> Switch from fainting for challenger. </summary>
    private IEnumerator GetNewChallengerPokemon()
    {
        /* Perform Faint Animation */
        Battler2.pokemon.Anim.Faint();

        /* Announce Faint */
        SoundManager.PlaySound("Crowd");
        CameraManager.instance.Focus(Battler2.pokeSpawner, -1.3f, 1.4f, Random.Range(0.1f, 0.2f));
        StartCoroutine(DialogueManager.Print(Battler2.pokemon.Name + " has fainted!", true));
        yield return new WaitForSeconds(1f);

        /* Return Challengers Fainted Pokemon */
        yield return CommandReturn(Battler2);

        /* Terminate Battle If Challenger Has No More Useable Pokemon */
        if (Battler2.trainer.AI.GetUseablePokemon().Count == 0)
            yield return TerminateBattle(Battler1);

        /* Queue A New AI Selected Pokemon For Challenger */
        Battler2.trainer.AI.QueueNewPokemon();

        /* Summon Challenger Summon */
        yield return CommandSummon(Battler2);

        /* Load Pokemon State UI */
        OptionManager.instance.state2.LoadState(Battler2.pokemon);

    }

    /// <summary> End the battle and announce the winner. </summary>
    /// <param name="winner"> Winner of battle. </param>
    private IEnumerator TerminateBattle(Battler winner)
    {
        /* Pan Around Challenger and Disable Options */
        CameraManager.instance.Focus(Battler2.trainer.Collider);
        OptionManager.instance.SetOptions(false);
        OptionManager.instance.SetPokeStates(false);

        /* Command Challenger To React To Winner */
        Battler2.trainer.Anim.BattleEnd(winner == Battler2);
        yield return DialogueManager.PrintAndWait(winner == Battler2 ? "I did it!" : "Darn, I need to do better...", true);

        /* Disable UI Perform Screen Transition */
        DialogueManager.SetTextVisibility(false);
        SoundManager.StopMusic(0.5f);
        yield return RenderManager.DoScreenTransition(ScreenTransition.SimulationEnd, 0, 0.5f, 0.5f, 0.5f, true, 20);

        /* For Now, Do Nothing */
        while (true) yield return null;

    }
}
