using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PokeAnimationControl : CharacterAnimationControl
{
    /// <summary> Determines if eye candy animation and blinking will occur. </summary>
    public bool EnableRandomAnimation { set { enableEyeCandy = value; Eyes.EnableBlink = value; } }

    /// <summary> Determines if eye candy animation will occur. </summary>
    private bool enableEyeCandy = true;

    [Tooltip("Chance that this pokemon performs its eye candy animation every second."), Range(0,1)]
    public float eyeCandyChance = 0.01f;

    /// <summary> Point to emit beam effects. </summary>
    [Tooltip("Point to emit beam effects.")]
    public BeamEmissionLookAt beamEmission;

    /// <summary> Point to emit right handed effects. </summary>
    [Tooltip("Point to emit right handed  effects.")]
    public Transform rightHand;

    /// <summary> Point to emit left handed effects. </summary>
    [Tooltip("Point to emit left handed effects.")]
    public Transform leftHand;

    /// <summary> Point to emit right handed effects. </summary>
    [Tooltip("Point to emit right footed effects.")]
    public Transform rightFoot;

    /// <summary> Point to emit right handed effects. </summary>
    [Tooltip("Point to emit left footed effects.")]
    public Transform leftFoot;

    /// <summary> Map that links this pokemon's moves to its corrosponding move animations. </summary>
    public PokeAnimation[] animationMap;

    /// <summary> Eye control for randomized blinking. </summary>
    public EyeController Eyes { get; private set; }

    /// <summary> Pokemon this animation controller is tied to. </summary>
    private Pokemon pokemon;

    /// <summary> Gets corrosponding transform from body part selection. </summary>
    /// <param name="part"> Body part to get.</param>
    /// <returns> Transform of that body part on this pokemon. </returns>
    public Transform GetPart(PokeBodyPart part)
    {
        switch(part)
        {
            case PokeBodyPart.Root : return transform;
            case PokeBodyPart.BeamEmission : return beamEmission.transform;
            case PokeBodyPart.LeftFoot : return leftFoot;
            case PokeBodyPart.LeftHand : return leftHand;
            case PokeBodyPart.RightFoot : return rightFoot;
            case PokeBodyPart.RightHand : return rightHand;
            default: return transform;
        }
    }

    protected override void Awake()
    {
        /* Get Components */
        base.Awake();
        pokemon = GetComponentInParent<Pokemon>();
        Eyes = GetComponent<EyeController>();
    }

    private void Update()
    {
        if (enableEyeCandy && (eyeCandyChance / 16) >= Random.value)
            SetAnimTrigger(AnimTrigger.DoEyeCandy);
    }

    /// <summary> A function to simply mark when an move event should be triggered in a move animation. This function doesn't do anything. </summary>
    private void MarkMoveEvent() { }

    /// <summary> Animation event to make pokmemon cry. </summary>
    private void MakeCrySound()
    {
        SoundManager.PlaySound(pokemon.BattleCry);
    }

    /// <summary> Makes pokemon do faint animation. </summary>
    public void Faint()
    {
        SetAnimTrigger(AnimTrigger.Faint);
    }

    /// <summary> Sets hurt reaction in designated normalized direction. </summary>
    /// <param name="force"> Force recieved. </param>
    public void SetHurt(Vector3 force)
    {
        /* Normalize Force */
        force = force.normalized;

        /* Set Animation Properties */
        SetAnimFloat(AnimFloat.HurtXAxis, force.x);
        SetAnimFloat(AnimFloat.HurtYAxis, force.y);
        SetAnimFloat(AnimFloat.HurtZAxis, force.z);
        SetAnimBool(AnimBool.IsHurt, true);
    }

    /// <summary> Ends hurt reaction. Use 'EndMove' to stop moving if moving. </summary>
    /// <param name="startMoving"> Whether or not pokemon should start moving. </param>
    public void ClearHurt(bool startMoving)
    {
        SetAnimBool(AnimBool.IsMoving, startMoving);
        SetAnimBool(AnimBool.IsHurt, false);
    }

    /// <summary> Ends movement animation if movement animation was started. </summary>
    public void EndMove()
    {
        SetAnimBool(AnimBool.IsMoving, false);
    }

}

/// <summary> Body part to spawn at. </summary>
public enum PokeBodyPart
{
    /// <summary> Emit root transform effects. </summary>
    [Tooltip("Emit root transform effects.")]
    Root,

    /// <summary> Emit beam effects. </summary>
    [Tooltip("Emit beam effects.")]
    BeamEmission,

    /// <summary> Emit right handed effects. </summary>
    [Tooltip("Emit right handed effects.")]
    RightHand,

    /// <summary> Emit left handed effects. </summary>
    [Tooltip("Emit left handed effects.")]
    LeftHand,

    /// <summary> Emit right handed effects. </summary>
    [Tooltip("Emit right footed effects.")]
    RightFoot,

    /// <summary> Emit right handed effects. </summary>
    [Tooltip("Emit left footed effects.")]
    LeftFoot
}
