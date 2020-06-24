using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trainer : PokeEntity
{
    #region Properties

    /// <summary> Prefab pokemon with data representing their current state. </summary>
    [Tooltip("Prefab pokemon with data representing their current state.")]
    public PokemonInstanceStatus[] party;

    /// <summary> Transform to parent pokeball to. </summary>
    [Tooltip("Transform to parent pokeball to. Will be automatically positioned and oriented.")]
    public Transform pokeBallContainer;

    /// <summary> Vertical angle at which trainer throws his/her ball. </summary>
    [Tooltip("Vertical angle at which trainer throws his/her ball."), Range(0, 90)]
    public float ballThrowAngle = 45;

    /// <summary> Force with which trainer throws the ball with. </summary>
    [Tooltip("Force with which trainer throws the ball with.")]
    public float ballThrowForce = 20;

    /// <summary> Current pokemon this trainer has out. </summary>
    public Pokemon CurrentPokemon { get; private set; }

    /// <summary> Current ball this trainer has out. </summary>
    public Pokeball Ball { get; private set; }

    /// <summary> Animation controller for the trainer. </summary>
    public TrainerAnimationController Anim { get; private set; }

    /// <summary> AI for this trainer. </summary>
    public TrainerAI AI { get; private set; }

    /// <summary> Transform to base new pokemon location off of. </summary>
    private Transform pokeSpawner;

    /// <summary> The party pokemon that will be chosen after selecting a pokemon. </summary>
    public PokemonInstanceStatus QueuedPokemon { get; set; }

    #endregion

    protected override void Awake()
    {
        /* Perform Base Functionality */
        base.Awake();

        /* Initialize Components */
        Anim = GetComponent<TrainerAnimationController>();
        Ball = null;
        CurrentPokemon = null;
        AI = GetComponent<TrainerAI>();
    }


    /// <summary>
    /// Queue's one of the trainer's available move for summon.
    /// The move can be executed by starting the 'Summon' coroutine.
    /// The move is queued in order for the battle system to react to
    /// a pending summon, if any behaviors wish to use the information about
    /// this pokemon's pending move.
    /// </summary>
    /// <param name="slot"> 
    /// The slot number of the move to be executed.
    /// This will return an error if the slot is out of range or no move exsists in that slot.
    /// </param>
    public void QueuePartyMember(int slot)
    {
        /* Throw Error At User and Return If Slot Is Out of Range */
        if (slot < 1 || slot > 6) {
            Debug.LogError(slot + " is NOT a valid slot for accessing party memebers. Use slots 1 -> 6");
            return;
        }

        /* Warn User and Return If NO Move Exsists In Slot */
        if (party[slot - 1] == null) {
            Debug.LogWarning(slot + " does NOT have a party member assigned to it.");
            return;
        }

        /* Otherwise, Set the Current Move and Current Target */
        QueuedPokemon = party[slot - 1];

    }


    /// <summary> Summons a queued party member. </summary>
    /// <param name="_pokeSpawner"> The parent of the pokemon. Will be positioned and oriented properly </param>
    public IEnumerator Summon(Transform _pokeSpawner)
    {
        /* Throw Error At User If NO Move Queued */
        if (QueuedPokemon == null) {
            Debug.LogError("No party member queued on " + name);
            yield break;
        }

        /* Save The Position */
        pokeSpawner = _pokeSpawner;

        if (Ball != null)
            Destroy(Ball.gameObject);

        /* Instantiate and Fix Position of Pokeball Instance Found in Designated Pokemon's Ball Slot */
        Ball = Instantiate(QueuedPokemon.state.ball, pokeBallContainer);
        Ball.transform.localPosition = Vector3.zero;

        /* Start Throw Animation, Which Gives Control To The Animation */
        Anim.ThrowBall();

        /* Wait Unity The Pokemon is Outside The Pokeball */
        yield return new WaitUntil(() => Ball.ReadyToInstance == true);

        /* Get Instanced Pokemon From Pokeball and Parent It To Spawner */
        CurrentPokemon = Instantiate(QueuedPokemon.pokemon, Ball.SummonPosition, pokeSpawner.rotation, pokeSpawner);

        /* Apply The Pokestate To This New Pokemon */
        CurrentPokemon.ApplyState(QueuedPokemon);

        /* Shape Pokemon */
        StartCoroutine(SummonShape());

        /* Destroy the Pokeball after some time. */
        Destroy(Ball.gameObject, 3f);


    }

    private IEnumerator SummonShape()
    {
        /* Make Pokemon White */
        CurrentPokemon.SMRenderer.ToEmmisiveHDRColor(Color.white, 2f);

        /* Initilaize Shape Weight */
        float weight = 100;

        /* While Still Shaping */
        while (weight > 0)
        {
            /* Update BlendShape And Scale Of Pokemon */
            weight = Mathf.MoveTowards(weight, 0, Time.deltaTime * 150);
            CurrentPokemon.SMRenderer.SetBlendShapeWeight(0, weight);
            CurrentPokemon.transform.localScale = Vector3.one * (1 - 0.009f * weight);
            yield return null;

        }

        /* Clear White Color and Size*/
        yield return CurrentPokemon.SMRenderer.ClearEmmisionOverTime(3f);

    }

    /// <summary> Instantly summons a pokemon at designated slot. </summary>
    /// <param name="slot"> The slot number of the pokemon to summon. </param>
    /// <param name="_pokeSpawner"> The parent of the pokemon. Will be positioned and oriented properly </param>
    public void SummonInstant(int slot, Transform _pokeSpawner)
    {
        pokeSpawner = _pokeSpawner;
        CurrentPokemon = Instantiate(party[slot].pokemon, _pokeSpawner);
        CurrentPokemon.ApplyState(party[slot]);
    }

    /// <summary> Animation event for launching the pokeball. </summary>
    private void LaunchBall()
    {
        /* Get Launch Force Vector */
        Vector3 throwForce = new Vector3(0, Mathf.Sin(ballThrowAngle * Mathf.Deg2Rad), Mathf.Cos(ballThrowAngle * Mathf.Deg2Rad)) * ballThrowForce * 10;

        /* Launch The Ball, Giving Control To The Pokeball */
        StartCoroutine(Ball.Launch(transform.TransformDirection(throwForce), pokeSpawner));
        
    }

    /// <summary> Returns this trainer's current pokemon on the field. </summary>
    public IEnumerator Return()
    {
        /* Instantiate and Fix Position of Pokeball Instance Found in Current Pokemon's Ball Slot */
        Ball = Instantiate(CurrentPokemon.Ball, pokeBallContainer);
        Ball.transform.localPosition = Vector3.zero;

        /* Start Trainer's Return Animation, Which Gives Control To The Animation */
        Anim.Returning(true);

        /* Wait Until The Pokeball Has Destroyed The Pokemon */
        yield return new WaitUntil(() => CurrentPokemon == null);

        /* End Returning Animation and Destroy The Pokeball after some time.*/      
        yield return new WaitForSeconds(0.5f);
        Anim.Returning(false);
        Destroy(Ball.gameObject, 1.5f);


    }

    /// <summary> Animation event for starting return. </summary>
    private void CommandReturnBeam()
    {
        Ball.StartReturn(CurrentPokemon);
    }

}
