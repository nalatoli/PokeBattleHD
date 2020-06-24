using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(RectTransform))]
public class UIAnimationControl : ScriptedAnimationController
{
    [Tooltip("All children to hide and show when this field is clicked. ")]
    public OptionFieldChild[] children;

    [Tooltip("Sound that plays when this field is clicked. ")]
    public Sound selectSound;

    /// <summary> Determines if this field is selected. </summary>
    bool selected;

    /// <summary> References Activate routine so it can be stopped manually. </summary>
    Coroutine activateRoutine;

    /// <summary> Controls whether this UI field is clickable. </summary>
    public bool Clickable { get; set; }

    protected override void Awake()
    {
        /* Initialize Properties and Deactivate Children */
        Animator = GetComponent<Animator>();
        selected = false;
        Activate(false);
        Clickable = true;
    }


    /// <summary> Set hover when mouse enters field. </summary>
    public void Enter() { if(Clickable) SetAnimBool(AnimBool.IsHovering, true); }

    /// <summary> Clear hover when mouse exits field. </summary>
    public void Exit() { if (Clickable) SetAnimBool(AnimBool.IsHovering, false); }

    /// <summary> Contol select and deselection when mouse clicks on this field. </summary>
    public void Click()
    {
        if (!Clickable)
            return;

        /* Play Sound */
        SoundManager.PlaySound("Blip");

        /* If Already Selected, Deselect This */
        if(selected) {
            Deselect();
            selected = false;
        }
                
        /* Else, Animate Click And Enable All Children */
        else {
            SetAnimTrigger(AnimTrigger.OnClick);
            OptionManager.instance.SetPokeballDirection(transform);
            Activate(true);
            selected = true;
        }

    }

    /// <summary> Animate deselect and disable children. </summary>
    public void Deselect()
    {
        if (!Clickable)
            return;

        SetAnimTrigger(AnimTrigger.Deselect);
        selected = false;
        Activate(false);
    }

    /// <summary> Controls activity of children. </summary>
    /// <param name="state"> New state of children. </param>
    private void Activate(bool state)
    {
        /* If Setting To Inactive */
        if (state == false)
        {
            /* Disable Sequential Activation Routine If The Routine Is Active */
            if (activateRoutine != null)
                StopCoroutine(activateRoutine);

            /* Disable All Children */
            foreach (OptionFieldChild child in children)
                child.gameObject.SetActive(false);
        }

        /* Else (If Setting Active), Start Sequential Activation Routine */
        else
            activateRoutine = StartCoroutine(SequentiallyEnable());

    }

    /// <summary> Sequentially enables all children one by one over time. </summary>
    private IEnumerator SequentiallyEnable()
    {
        foreach (OptionFieldChild child in children)
        {
            child.gameObject.SetActive(true);
            yield return new WaitForSeconds(0.01f);
        }
    }

    private void OnDisable()
    {
        /* Disable Children */
        selected = false;
        Activate(false);
    }



}
