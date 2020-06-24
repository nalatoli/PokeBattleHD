using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pokeball : PokeEntity
{
    #region Properties

    /// <summary> Sound that plays when a pokemon is being summoned. </summary>
    [Tooltip("Sound that plays when a pokemon is being summoned.")]
    public Sound summonSound;

    /// <summary> Particle system that activates when a pokemon is being summoned. </summary>
    [Tooltip("Particle system that activates when a pokemon is being summoned.")]
    public ParticleSystem summonBeam;

    /// <summary> Sound that plays when a pokemon is being returned. </summary>
    [Tooltip("Sound that plays when a pokemon is being returned.")]
    public Sound returnSound;

    /// <summary> Particle system that activates when a pokemon is being summoned. </summary>
    [Tooltip("Particle system that activates when a pokemon is being summoned.")]
    public ParticleSystem returnBeam;

    /// <summary> Particle system that contains returned pokemon. </summary>
    [Tooltip("Particle system that contains returned pokemon.")]
    public ParticleSystem returnContainer;

    /// <summary> Determines if pokemon is ready to instance. </summary>
    public bool ReadyToInstance { get; private set; }

    /// <summary> Position where summon beam has made contact with the ground. </summary>
    public Vector3 SummonPosition { get; private set; }

    /// <summary> Determines if pokemon is ready to destroy. </summary>
    public bool ReadyToDestroy { get; private set; }

    /// <summary> The animation controller for the pokeball. </summary>
    public BallAnimationControl Anim { get; private set; }

    /// <summary> The Rigidbody component of this pokeball. </summary>
    Rigidbody rb;

    /// <summary> The audiosource component of this pokeball. </summary>
    AudioSource sound;

    #endregion

    protected override void Awake()
    {
        /* Perform Base Awake Functionality */
        base.Awake();

        /* Initialize Properties */
        rb = GetComponent<Rigidbody>();
        Anim = GetComponent<BallAnimationControl>();
        sound = GetComponent<AudioSource>();
        ReadyToInstance = false;
        ReadyToDestroy = false;
        
    }


    /// <summary> Launches ball in desired force direction. Will open up right when it reaches peak y-distance. </summary>
    /// <param name="force"> Force direction and magnitude of launch. </param>
    /// <param name="pokeSpawner"> Transform to put the pokemon after opening. </param>
    public IEnumerator Launch(Vector3 force, Transform pokeSpawner)
    {
        /* Set Up Gameobject */
        transform.parent = null;
        rb.isKinematic = false;

        /* Launch The Ball */
        rb.AddForce(force, ForceMode.Force);

        /* Rotate The Ball Gradually to Face Poke Position Until Open Condition Is Met */
        while (rb.velocity.y >= 0) {
            Vector3 dir = pokeSpawner.position - transform.position;
            Quaternion rot = Quaternion.LookRotation(dir);
            transform.rotation = Quaternion.Slerp(transform.rotation, rot, 10 * Time.deltaTime);
            yield return null;
        }

        /* Animate Pokeball Opening, Which Will Give Control To The Particle System */
        rb.isKinematic = true;
        Anim.Open();
        SoundManager.PlaySound(summonSound);
        summonBeam.Stop();
        summonBeam.Play();
    }

    /// <summary> Instantiates prefab pokemon inside ball at target location. </summary>
    /// <param name="pos"> World position of pokemon instance. </param>
    public IEnumerator OnSummonContact(Vector3 pos)
    {
        /* Mark That This Ball Is Ready To Instance */
        SummonPosition = pos;
        ReadyToInstance = true;

        /* Stop The Beam And Close After 1 Second */
        summonBeam.Stop();
        yield return new WaitForSeconds(1f);
        rb.isKinematic = false;
        Anim.Close();
    }


    /// <summary> Summons return beam from pokeball. </summary>
    /// <param name="target"> Pokemon to return. </param>
    public void StartReturn(Pokemon target)
    {
        /* Rotate Pokeball To Look at The Target */
        transform.LookAt(target.Collider.bounds.center);

        /* Open The Ball, Play Sound, And Start Return Beam. Control Is Given To The Beam */
        Anim.Open();
        SoundManager.PlaySound(returnSound);
        returnBeam.Stop();
        returnBeam.Play();

    }

    /// <summary> Returns pokemon to pokeball. </summary>
    /// <param name="target"> Pokemon target that this ball is returning. </param>
    public IEnumerator OnReturnContact(Pokemon target)
    {
        /* Wait Until Pokemon Scale Is Very Small */
        yield return target.SMRenderer.ToEmmisiveHDRColorOverTime(Color.red, 10, 20);

        /* Enable Kinematic Property So This Pokemon Won't React To Physics */
        target.Rigidbody.isKinematic = true;
        target.Rigidbody.useGravity = false;

        /* Turn Pokemon To Small Red Orb */
        yield return ReturnShape(target);

        target.transform.parent = transform;
        ParticleSystem r = Instantiate(returnContainer, target.transform);
        ParticleSystem.ShapeModule s = r.shape;
        s.skinnedMeshRenderer = target.SMRenderer;

        while (target.transform.localPosition != Vector3.zero)
        {
            target.transform.localPosition = Vector3.MoveTowards(target.transform.localPosition, Vector3.zero, Time.deltaTime * 16);
            yield return null;
        }

        /* Destroy The Pokemon, Stop The Beam, and Close The Ball */
        Destroy(target.gameObject);
        returnBeam.Stop();
        Anim.Close();
        
    }

    private IEnumerator ReturnShape(Pokemon pokemon)
    {
        /* Make Pokemon White */
        pokemon.SMRenderer.ToEmmisiveHDRColor(Color.red, 10f);

        /* Initilaize Shape Weight */
        float weight = 0;

        /* While Still Shaping */
        while (weight < 100)
        {
            /* Update BlendShape And Scale Of Pokemon */
            weight = Mathf.MoveTowards(weight, 100, Time.deltaTime * 200);
            pokemon.SMRenderer.SetBlendShapeWeight(0, weight);
            pokemon.transform.localScale = Vector3.one * (1 - 0.0095f * weight);
            yield return null;
        }

    }

}
