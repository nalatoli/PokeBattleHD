using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class OptionMainField : MonoBehaviour
{
    /// <summary> UI animation controller for this main option field. </summary>
    public UIAnimationControl Anim { get; private set; }

    protected virtual void Awake()
    {
        /* Get Components */
        Anim = GetComponent<UIAnimationControl>();
    }

    protected virtual void OnEnable()
    {
        /* Make This UI Clickable */
        Anim.Clickable = true;
    }
}
