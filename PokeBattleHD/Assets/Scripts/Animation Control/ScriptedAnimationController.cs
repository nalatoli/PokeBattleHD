using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Animations;


/// <summary> 
/// Interfaces 'Pokemon' animation controller with external commands 
/// </summary>
[RequireComponent(typeof(Animator))]
public class ScriptedAnimationController : MonoBehaviour
{
    /// <summary> Animator driven by this scripted animation controller. </summary>
    public Animator Animator { get; protected set; }

    protected virtual void Awake()
    {
        /* Initialize Properties */
        Animator = GetComponent<Animator>();
    }

    protected void SetAnimBool(AnimBool param, bool state) { Animator.SetBool(param.ToString(), state); }

    protected void SetAnimTrigger(AnimTrigger param) { Animator.SetTrigger(param.ToString()); }

    protected void SetAnimInt(AnimInt param, int value) { Animator.SetInteger(param.ToString(), value); }

    protected void SetAnimFloat(AnimFloat param, float value) { Animator.SetFloat(param.ToString(), value); }

}



