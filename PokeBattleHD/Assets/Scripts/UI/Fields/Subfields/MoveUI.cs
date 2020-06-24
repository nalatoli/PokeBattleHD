using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Moves;

public class MoveUI : OptionFieldChild
{
    /* Inspector Fields */
    [SerializeField] private Image border = null;
    [SerializeField] private Image border_Typesprite = null;
    [SerializeField] private new TextMeshProUGUI name = null;
    [SerializeField] private TextMeshProUGUI ppCount = null;
    [SerializeField] private Image circle = null;
    [SerializeField] private TextMeshProUGUI circle_Question = null;

    /* Private Fields */
    private Move move;
    List<Sprite> spritesheet = null;
    float spriteSpeed;
    IEnumerator circleRoutine, questionRoutine;
    IEnumerator r1, r2, r3;
    bool observingMove;

    /// <summary> Determines if move slot has a loaded move. </summary>
    public bool IsLoaded { get { return move != null; } }

    protected override void Awake() { }

    protected override void OnEnable() { StartCoroutine(ScaleUp());  }

    private void Start()
    {
        /* Perform Base Start Functionality amd Subscribe PP Change Event */
        EventManager.instance.onPPChanged += OnPPChanged;
        observingMove = false;
    }

    /// <summary> Instantly re-writes the move state to match all the information about the move. </summary>
    /// <param name="_move"> Move to extract information from. </param>
    public void LoadState(Move _move)
    {
        /* Warn User and Return If This Move is Already Being Referenced */
        if (move == _move) {
            Debug.LogWarning(gameObject.name + " is already referencing " + move + "'s information.");
            return;
        }

        /* Make A Reference To This Move */
        move = _move;

        /* Set Name and PP Text Information and Aestetics */
        name.text = move.Name;
        name.colorGradient = move.Type.TextColorTheme;
        name.color = Color.white;
        ppCount.text = move.PP + "/" + move.MaxPP;
        ppCount.colorGradient = move.Type.TextColorTheme;     
        ppCount.color = Color.white;

        /* Set The Border Color */
        border.color = move.Type.BorderColor;

        /* Initalize Circle Colors */
        circle.color = Color.white * 0.7f;
        circle_Question.color = Color.clear;

        /* Initialize Type Sprite To Show First Sprite in Move's Spritesheet */
        spritesheet = move.Type.Spritesheet;
        border_Typesprite.enabled = true;
        border_Typesprite.sprite = spritesheet[0];
        spriteSpeed = move.Type.SpriteSheetSpeed;

        /* Initialize Move Observation State */
        observingMove = false;

    }

    /// <summary> Instantly clears all info about any moves. </summary>
    public void ClearState()
    {
        name.text = "";
        ppCount.text = "";
        spritesheet = null;
        border.color = Color.white * 0.7f;
        border_Typesprite.enabled = false;
        circle.color = Color.white * 0.7f;
        circle_Question.color = Color.clear;
        move = null;
    }

    /// <summary> Animates the border and text on the UI. </summary>
    /// <param name="state"> True for animation, false for no animation. </param>
    public void AnimateTypeUI(bool state)
    {
        /* Do Nothing If Move Is NOT Loaded */
        if (!IsLoaded || !gameObject.activeInHierarchy)
            return;

        /* If Setting Off, Start Slowdown and Return */
        if (state == false) {
            this.StartInterruptableCoroutine(border_Typesprite.AnimateSpritesheetToStop(spritesheet), ref r1);
            this.StartInterruptableCoroutine(name.ChangeColorOvertime(Color.white, 3f), ref r2);
            this.StartInterruptableCoroutine(ppCount.ChangeColorOvertime(Color.white, 3f), ref r3);
            return;
        }

        /* Otherwise, Start Animating Sprite */
        this.StartInterruptableCoroutine(border_Typesprite.AnimateSpritesheet(spritesheet, spriteSpeed), ref r1);
        this.StartInterruptableCoroutine(name.FluxColor(name.color, Color.white / 1.5f, 2f), ref r2);
        this.StartInterruptableCoroutine(ppCount.FluxColor(ppCount.color, Color.white / 1.5f, 2f), ref r3);
    }

    /// <summary> Sets hover state of circle. </summary>
    /// <param name="state"> True for brighter circle with question mark and false for darker cirlce. </param>
    public void HoverCircle(bool state)
    {
        /* Do Nothing If Move Is NOT Loaded */
        if (!IsLoaded || observingMove || !gameObject.activeInHierarchy)
            return;

        /* Set Colors Of Circle And Question Mark */
        this.StartInterruptableCoroutine(circle.ChangeColorOvertime(Color.white * (state ? 1 : 0.7f), 16), ref circleRoutine);
        this.StartInterruptableCoroutine(circle_Question.ChangeColorOvertime(state ? Color.white : Color.clear, 16), ref questionRoutine);

    }

    /// <summary> Sets select state of circle. </summary>
    /// <param name="state"> True for always in hover state and enable move data UI. False for disable move data UI.</param>
    public void SelectCircle(bool state)
    {
        /* Do Nothing If Move Is NOT Loaded */
        if (!IsLoaded || !gameObject.activeInHierarchy)
            return;

        /* Make Blip Sound */
        SoundManager.PlaySound("Blip");

        /* If Selecting The Cirlce */
        if (state == true)
        {
            /* If This Move Is NOT Being Observed */
            if (!observingMove)
            {
                /* Enable Move Data UI, Load This Move, and Set To Observing This Move */
                MovePanel.instance.SetVisibility(true);
                MovePanel.instance.LoadState(move);
                observingMove = true;
            }

            /* Else (This Move Is Already Being Observed) */
            else
            {
                /* Disable Move Data UI and Set to NOT Observing This Move */
                MovePanel.instance.SetVisibility(false);
                observingMove = false;
            }
        }

        /* Else (Deselecting the Circle) */
        else
        {
            /* Set Observing Move To False and Update Hover Mode */
            observingMove = false;
            HoverCircle(false);
        }

    }

    /// <summary> Event callback for when PP of move changes. This updates the PP on UI. </summary>
    /// <param name="move"> Move that called the callback. </param>
    private void OnPPChanged(Move move)
    {
        if (move == this.move)
            ppCount.text = this.move.PP + "/" + this.move.MaxPP;

    }


    private void OnDisable()
    {
        /* Disable Circle Select On Disable */
        SelectCircle(false);
    }
}
