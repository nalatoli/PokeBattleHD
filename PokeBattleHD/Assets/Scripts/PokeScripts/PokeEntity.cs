using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PokeEntity : MonoBehaviour
{
    /// <summary> Collider component of this entity. </summary>
    public Collider Collider { get; protected set; }

    /// <summary> Rigidbody component of this entity. </summary>
    public Rigidbody Rigidbody { get; protected set; }

    /// <summary> Assign collider component. </summary>
    protected virtual void Awake()
    {
        /* Get Components */
        Collider = GetComponent<Collider>();
        Rigidbody = GetComponent<Rigidbody>();
    }

}
