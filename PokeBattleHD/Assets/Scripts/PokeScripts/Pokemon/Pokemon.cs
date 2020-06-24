using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Moves;

public class Pokemon : PokeEntity
{
    #region Properties

    #region Data

    [Tooltip("Default name of pokemon. Will be overriden by nickname if nickname has at least one character.")]
    public new string name = "Dummy";

    [Tooltip("Determines effectiveness of damage dealt and taken."), SerializeField]
    private PokeTypeList types = null;
    /// <summary> Various types of this pokemon. </summary>
    public PokeTypeList Types { get { return types; } }

    [Tooltip("Sprite to use for UI."), SerializeField]
    private Sprite face = null;
    /// <summary> Sprite to use for UI. </summary>
    public Sprite Face { get { return face; } set { face = value; Events.FaceChanged(); } }

    [Tooltip("Determines amount of damage that can be taken before fainting."), Range(0, 1000), SerializeField]
    private int baseHP = 100;
    /// <summary> Base HP of pokemon. </summary>
    public int BaseHP { get { return baseHP; } set { baseHP = value; Events.BaseStatChanged(); } }

    [Tooltip("Determines power when dealing physical damage."), Range(0, 1000), SerializeField]
    private int baseAttack = 50;
    /// <summary> Base Attack of pokemon. </summary>
    public int BaseAttack { get { return baseAttack; } set { baseAttack = value; Events.BaseStatChanged(); } }

    [Tooltip("Determines damage taken when dealt physical damage."), Range(0, 1000), SerializeField]
    private int baseDefense = 50;
    /// <summary> Base Defense of pokemon. </summary>
    public int BaseDefense { get { return baseDefense; } set { baseDefense = value; Events.BaseStatChanged(); } }

    [Tooltip("Determines power when dealing special damage."), Range(0, 1000), SerializeField]
    private int baseSpecialAttack = 50;
    /// <summary> Base Special Attack of pokemon. </summary>
    public int BaseSpecialAttack { get { return baseSpecialAttack; } set { baseSpecialAttack = value; Events.BaseStatChanged(); } }

    [Tooltip("Determines damage taken when dealt special damage."), Range(0, 1000), SerializeField]
    private int baseSpecialDefense = 50;
    /// <summary> Base Special Defense of pokemon. </summary>
    public int BaseSpecialDefense { get { return baseSpecialDefense; } set { baseSpecialDefense = value; Events.BaseStatChanged(); } }

    [Tooltip("Determines priority in using moves when moves have the same priority."), Range(0, 1000), SerializeField]
    private int baseSpeed = 50;
    /// <summary> Base Speed of pokemon. </summary>
    public int BaseSpeed { get { return baseSpeed; } set { baseSpeed = value; Events.BaseStatChanged(); } }
   
    [Tooltip("Sound that this pokemon makes when first summoned and in other instances."), SerializeField]
    private Sound battleCry = null;
    /// <summary> Battle cry of this pokemon. </summary>
    public Sound BattleCry { get { return battleCry; } set { battleCry = value; Events.BattleCryChanged(); } }

    #endregion

    #region Immediete State

    /// <summary> Reference to immediete state of pokemon. </summary>
    private PokemeonState state = null;

    /// <summary> Name of this pokemon. Will be pokemon's default name unless it has a nickname. </summary>
    public string Name { 
        get { return ((state.nickname.Length < 1) ? name : state.nickname); }
        set { state.nickname = value; Events.NameChanged(); }
    }

    /// <summary> Gender of this pokemon. </summary>
    public PokeGender Gender { get { return state.gender; } set { state.gender = value; Events.GenderChanged(); } }

    /// <summary> Current level of this pokemon. </summary>
    public int Level { get { return state.level; } set { state.level = value; Events.LevelChanged(); } }

    /// <summary> Current HP of the pokemon. </summary>
    public int Hp { 
        get { return (int)(state.hP * MaxHp.Value); }
        set { 
            state.hP = Mathf.Clamp((float)value / MaxHp.Value, 0, MaxHp.Value);
            Events.HPChanged();
        }
    }

    /// <summary> Current XP of the pokemon. </summary>
    public float Xp 
    { 
        get { return state.xP; }
        set {
            state.xP = Mathf.Clamp((float)value, 0, 1);
            Events.XPChanged();
        }
    }

    /// <summary> Current status effects on this pokemon. </summary>
    public StatusEffectList StatusEffects { get; private set; }

    /// <summary> Pokeball prefab this pokemon came from. </summary>
    public Pokeball Ball 
    {
        get
        {
            /* Warn User and Assign Default Ball If NO Ball Assigned */
            if (state.ball == null) {
                Debug.LogWarning("No ball assigned. Assigning default ball to this PokeInstanceStatus scriptable object.");
                state.ball = Resources.Load<Pokeball>("PokeAssets/Pokeballs/Pokeball/_PREFAB/PokeBall");
            }

            /* Return Ball */
            return state.ball;
        }

        set { state.ball = value; Events.BallChanged(); } }

    #endregion

    #region Stats

    /// <summary> Maximum HP of the pokemon. </summary>
    public Stat MaxHp { get; private set; }

    /// <summary> Attack stat of pokemon. </summary>
    public Stat Attack { get; private set; }

    /// <summary> Defense stat of pokemon. </summary>
    public Stat Defense { get; private set; }

    /// <summary> Special attack stat of pokemon. </summary>
    public Stat SpecialAttack { get; private set; }

    /// <summary> Special defense stat of pokemon. </summary>
    public Stat SpecialDefense { get; private set; }

    /// <summary> Speed stat of pokemon. </summary>
    public Stat Speed { get; private set; }

    #endregion

    #region Move Properties

    /// <summary> Array of move instances that pokemon can use. </summary>
    private Move[] moves;


    /// <summary> Queued move of pokemon. Will execute when 'UseMove' is called. </summary>
    public Move CurrentMove { get; set; }

    /// <summary> Current target of pokemon. Will use move on target when 'UseMove' is called. </summary>
    public Pokemon CurrentTarget { get; set; } = null;

    #endregion

    #region Components

    /// <summary> The 'Skinned Mesh Renderer' component of this pokemon. </summary>
    public SkinnedMeshRenderer SMRenderer { get; private set; }

    /// <summary> The 'PokeAnimationControl' component of this pokemon. </summary>
    public PokeAnimationControl Anim { get; private set; }

    /// <summary> The 'HitHandler' component of this pokemon. </summary>
    public HitHandler Hits { get; private set; }

    /// <summary> The 'PokemonEventHandler' component of this pokemon. </summary>
    public PokemonEventHandler Events { get; set; }

    /// <summary> Particle system for stat changes. </summary>
    private ParticleSystem statParticles;

    #endregion

    #endregion

    protected override void Awake()
    {
        /* Perform Base Awake Functionality */
        base.Awake();

        /* Get Components */
        SMRenderer = GetComponentInChildren<SkinnedMeshRenderer>();
        Anim = GetComponentInChildren<PokeAnimationControl>();
        Hits = GetComponent<HitHandler>();
        Events = new PokemonEventHandler(this);
        statParticles = GetComponent<ParticleSystem>();

    }

    /// <summary>
    /// Queue's one of the pokemon's available move for use.
    /// The move can be executed by starting the 'UseMove' coroutine.
    /// The move is queued in order for the battle system to react to
    /// a pending move, if any behaviors wish to use the information about
    /// this pokemon's pending move.
    /// </summary>
    /// <param name="slot"> 
    /// The slot number of the move to be executed.
    /// This will return an error if the slot is out of range or no move exsists in that slot.
    /// </param>
    /// <param name="target">
    /// The opposing pokemon in the battle. 
    /// The move itself may or may not use this information.
    /// </param>
    public void QueueMove(int slot, Pokemon target)
    {
        /* Throw Error At User and Return If Slot Is Out of Range */
        if (slot < 1 || slot > 4) {
            Debug.LogError(slot + " is NOT a valid slot for accessing moves. Use slots 1 -> 4");
            return;
        }

        /* Warn User and Return If NO Move Exsists In Slot */
        if (moves[slot - 1] == null) {
            Debug.LogWarning(slot + " does NOT have a move assigned to it.");
            return;
        }

        /* Otherwise, Set the Current Move and Current Target */
        CurrentMove = moves[slot - 1];
        CurrentTarget = target;

    }

    /// <summary>
    /// Makes pokemon use it's queued (current) move selection.
    /// The pokemon will call events before and after move usage. If
    /// an event nullifies the move, this coroutine will return immedietly.
    /// </summary>
    public IEnumerator UseMove()
    {
        if (CurrentMove == null)
            yield break;

        /* Call PP Changed Event */
        CurrentMove.PP--;

        /* Use The Move */
        yield return CurrentMove.Use();

    }

    /// <summary> Returns move in designated slot. </summary>
    /// <param name="slot"> Slot of move from [1,4] </param>
    /// <returns> The move. Null if no valid move is found. </returns>
    public Move Move(int slot)
    {
        /* Throw Error At User If Slot Is Out of Range */
        if (slot < 1 || slot > 4)
        {
            Debug.LogError(slot + " is NOT a valid slot for accessing moves. Use slots 1 -> 4");
            return null;
        }

        /* Otherwise Return Move */
        return moves[slot - 1];
    }

    /// <summary> Applys a state to an instanced pokemon. </summary>
    /// <param name="status"> Scriptable object state to apply to this instanced pokemon. </param>
    public void ApplyState(PokemonInstanceStatus status)
    {
        /* Establish Reference To Scriptable Object State */
        state = status.state;

        /* Update Stats Using Level */
        GetStats();

        /* Generate Move Instances As Children To Pokemon */
        GenerateMoves();

        /* Generate Status Effect Instances As Children To Pokemon */
        GenerateStatusEffects();

    }

    /// <summary> Update all stats using the state of the pokemon. </summary>
    private void GetStats()
    {
        /* Get Stat Particle System for Stat Generation */
        ParticleSystem p = GetComponent<ParticleSystem>();

        /* Generate HP Stat  */
        MaxHp = new Stat(baseHP, state.level, true, Color.red);

        /* Generate All Other Stats */
        Attack = new Stat(baseAttack, state.level, false, new Color(1f, 0.5f, 0));
        Defense = new Stat(baseDefense, state.level, false, Color.yellow);
        SpecialAttack = new Stat(baseSpecialAttack, state.level, false, Color.blue);
        SpecialDefense = new Stat(baseSpecialDefense, state.level, false, Color.green);
        Speed = new Stat(baseSpeed, state.level, false, new Color(1f, 0, 1f));

    }

    /// <summary> Generates move instances as children to this pokemon. </summary>
    private void GenerateMoves()
    {
        /* Initialize New Array Of Moves */
        moves = new Move[4];

        /* If NO Move Is Assigned, Warn User And Return */
        if (!state.move1 && !state.move2 && !state.move3 && !state.move4) {
            Debug.LogWarning("No moves assigned to " + Name + "'s state. Assiging default move to state");
            return;
        }

        /* Instantiate Move 1 From State If A Move Is Assigned */
        if (state.move1 != null) {
            moves[0] = Instantiate(state.move1, transform);
            moves[0].PP = (int)(state.pP1 * state.move1.MaxPP);
            moves[0].name = state.move1.name;
        }

        /* Instantiate Move 2 From State If A Move Is Assigned */
        if (state.move2 != null) {
            moves[1] = Instantiate(state.move2, transform);
            moves[1].PP = (int)(state.pP2 * state.move2.MaxPP);
            moves[1].name = state.move2.name;
        }

        /* Instantiate Move 3 From State If A Move Is Assigned */
        if (state.move3 != null) {
            moves[2] = Instantiate(state.move3, transform);
            moves[2].PP = (int)(state.pP3 * state.move3.MaxPP);
            moves[2].name = state.move3.name;
        }

        /* Instantiate Move 4 From State If A Move Is Assigned */
        if (state.move4 != null) {
            moves[3] = Instantiate(state.move4, transform);
            moves[3].PP = (int)(state.pP4 * state.move4.MaxPP);
            moves[3].name = state.move4.name;
        }       
    }

    /// <summary> Generate status effect instances as children to pokemon. </summary>
    private void GenerateStatusEffects()
    {
        /* Initialize New List Of Instanced Status Effects */
        StatusEffects = new StatusEffectList(this, state.statusEffects, transform);
        
    }

    /// <summary> Gets this pokemons stat by stat type. </summary>
    /// <param name="type"> The stat type to return. </param>
    /// <returns>Stat by stat type. </returns>
    public Stat GetStat(StatType type)
    {
        switch(type)
        {
            case StatType.Attack: return Attack;
            case StatType.Defense: return Defense;
            case StatType.SpAttack: return SpecialAttack;
            case StatType.SpDefense: return SpecialDefense;
            case StatType.MaxHP: return MaxHp;
            case StatType.Speed: return Speed;
            default: Debug.Log(type + " is NOT a valid stat type."); return null;
        }
    }

    /// <summary> Gets this pokemons stats by stat types. </summary>
    /// <param name="types"> The stat types to return. </param>
    /// <returns> Stats by stat types in the order of the passed array. </returns>
    public Stat[] GetStats(StatType[] types)
    {
        Stat[] stats = new Stat[types.Length];
        for (int i = 0; i < types.Length; i++)  stats[i] = GetStat(types[i]);
        return stats;
    }

    /// <summary> Adds a stat modifier to pokemon and summons particles depending on new value. </summary>
    /// <param name="stat"> Stat to change. </param>
    /// <param name="mod"> Modifier to apply. </param>
    public void AddStatMod(StatType stat, StatModifier mod)
    {
        /* Get Stat By Type */
        Stat s = GetStat(stat);

        /* Get Current Stat Value */
        int value = s.Value;

        /* Apply Modifier */
        s.AddModifier(mod);

        /* If Value Has , Animate Particles */
        if (value != s.Value)
            StartCoroutine(AnimateStatParticles(s.Color, value < s.Value));

    }

    /// <summary> Adds a stat modifier to pokemon and summons particles depending on new value. </summary>
    /// <param name="stat"> Stat to change. </param>
    /// <param name="mod"> Modifier to apply. </param>
    public void AddStatMod(StatType[] stats, StatModifier mod)
    {
        /* Get Stats By Types */
        Stat[] s = GetStats(stats);

        /* Get Current First Stat Value */
        int value = s[0].Value;

        /* Add Modifiers To Each Stat */
        foreach (Stat stat in s)
            stat.AddModifier(mod);

        /* If First Value Has Changed, Animate Particles */
        if (value != s[0].Value)
            StartCoroutine(AnimateStatParticles(Color.white, value < s[0].Value));

    }

    /// <summary> Animates stat particle system. </summary>
    /// <param name="color"> Color to make the particles. </param>
    /// <param name="rise"> True for rising values and false for lowering values.</param>
    private IEnumerator AnimateStatParticles(Color color, bool rise)
    {
        /* Declare Stat Change Sound */
        Sound sound;

        /* Get Modules */
        ParticleSystem.MainModule main = statParticles.main;
        ParticleSystem.ShapeModule shape = statParticles.shape;

        /* Setup Position and Shape of Particle System As Well As Sound Effect */
        if (rise) {
            shape.rotation = new Vector3(-90, 0, 0);
            shape.position = new Vector3(0, 0, 0);
            sound = SoundManager.GetSound("StatRise");
        }   else  {
            shape.rotation = new Vector3(90, 0, 0);
            shape.position = new Vector3(0, 3, 0);
            sound = SoundManager.GetSound("StatFall");
        }

        /* Set Particle Color, Start Particles, and Play Sound */
        main.startColor = color;
        statParticles.Restart();
        SoundManager.PlaySound(sound);

        /* Change Color Of Pokemon */
        yield return SMRenderer.ToEmmisiveHDRColorOverTime(color, 1.5f, 6f);
        yield return new WaitForSeconds(0.6f);
        yield return SMRenderer.ClearEmmisionOverTime(6f);

    }

}




