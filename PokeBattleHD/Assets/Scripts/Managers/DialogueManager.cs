using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;
using UnityEngine;
using System;
using System.Threading;

/// <summary> Provides access to the dialogue box located at the bottom of the screen. </summary>
public class DialogueManager : MonoBehaviour
{
    #region Properties

    /// <summary> Normal delay from printing one character to printing another character. </summary>
    public static float printDelay = 0.01f;

    /// <summary> Delay from printing one character to printing another character when 'Z' is held. </summary>
    public static float printDelayFast = 1E-5f;

    /// <summary> Sound that plays when a continue is processed. </summary>
    public static Sound continueSound;

    /// <summary> Text container. Controls visibibility. </summary>
    private static GameObject dialogueBox = null;

    /// <summary> Text backed by text mesh pro UGUI. </summary>
    private static TextMeshProUGUI tmptext = null;

    /// <summary> Monobehavior instance to start coroutines. </summary>
    public static DialogueManager instance;

    /// <summary> Determines if dialogue manager is already printing something. </summary>
    public static bool Printing { get; private set; } = false;

    /// <summary> Determines if a current printing routine should end due to another routine requesting print. </summary>
    private static bool overrideFlag = false;

    #endregion

    void Awake ()
    {
        /* Get References */
        instance = this;
        continueSound = new Sound(Resources.Load<AudioClip>("Sound/SFX/Blip"));
        dialogueBox = transform.GetChild(0).gameObject;
        tmptext = dialogueBox.transform.GetChild(0).GetComponent<TextMeshProUGUI>();

        /* Reset Dialogue Box */
        dialogueBox.SetActive(false);

    }

    /// <summary> Set whether dialogue box visibility. The text box is automatically cleared. </summary>
    /// <param name="state"> True for visible, false for invisible. </param>
    public static void SetTextVisibility(bool state)
    {
        /* Clear Text If Just Now Making Visible */
        if(!dialogueBox.activeInHierarchy && state == true)
            ClearText();

        /* Set Active State of Dialogue Box */
        dialogueBox.SetActive(state);
    }

    /// <summary> Manually clear text in dialogue box. </summary>
    public static void ClearText()
    {
        tmptext.text = "";
    }

    /// <summary> Set vertex color of all text in dialogue box. </summary>
    /// <param name="color"> Vertex color of text. </param>
    public static void SetTextColor(Color color)
    {
        /* Set Color of Text */
        tmptext.color = color;
    }

    /// <summary> Prints text, then waits for continue. </summary>
    /// <param name="text"> Text to print. </param>
    /// <param name="clear"> True will clear exsiting text. False will leave text. </param>
    public static IEnumerator PrintAndWait(string text, bool clear)
    {
        /* Print Text, Then Wait For Continue */
        SetTextVisibility(true);
        yield return Print(text, clear);
        yield return WaitForContinue();
    }

    /// <summary> Prints text instantly, then waits for continue. </summary>
    /// <param name="text"> Text to print. </param>
    /// <param name="clear"> True will clear exsiting text. False will leave text. </param>
    public static IEnumerator PrintInstantAndWait(string text, bool clear)
    {
        SetTextVisibility(true);
        PrintInstant(text, clear);
        yield return WaitForContinue();
    }

    /// <summary> Prints text and waits. Routine ends after duration has elapsed and text has printed. </summary>
    /// <param name="text"> Text to print. </param>
    /// <param name="duration"> Duration to wait. </param>
    /// <param name="clear"> True will clear exsiting text. False will leave text. </param>
    public static IEnumerator PrintAndWaitFor(string text, float duration, bool clear)
    {
        SetTextVisibility(true);
        float startTime = Time.time;
        yield return Print(text, clear);
        yield return new WaitForSeconds(Mathf.Clamp(duration - (Time.time - startTime), 0, duration)); ;
    }

    /// <summary> Prints text instantly and waits. Routine ends after duration has elapsed. </summary>
    /// <param name="text"> Text to print. </param>
    /// <param name="duration"> Duration to wait. </param>
    /// <param name="clear"> True will clear exsiting text. False will leave text. </param>
    public IEnumerator PrintInstantAndWaitFor(string text, float duration, bool clear)
    {
        SetTextVisibility(true);
        PrintInstant(text, clear);
        yield return new WaitForSeconds(duration);
    }

    /// <summary>
    /// Instantly print string onto dialogue box. 
    /// This text will be appended to any text that may already be
    /// in the dialogue box.
    /// </summary>
    /// <param name="text"> Text to print. </param>
    /// <param name="clear"> True will clear exsiting text. False will leave text. </param>
    public static void PrintInstant(string text, bool clear)
    {
        SetTextVisibility(true);
        if (clear) ClearText();
        tmptext.text += text;
    }


    /// <summary>
    /// Print string onto dialogue box overtime. 
    /// Change the public print delay properties to adjust the speed. 
    /// This text will be appended to any text that may already be
    /// in the dialogue box.
    /// </summary>
    /// <param name="text"> Text to print. </param>
    /// <param name="clear"> True will clear exsiting text. False will leave text. </param>
    public static IEnumerator Print(string text, bool clear)
    {
        /* Enable Dialogue Box and Optionally Clear Text */
        SetTextVisibility(true);
        if (clear) ClearText();

        /* If There Is A Running Print Routine */
        if (Printing)
        {
            /* Set Override and Wait Until The Running Routine Clears It */
            overrideFlag = true;
            yield return new WaitUntil(() => overrideFlag == false);
        }

        /* Mark That There is A Running Print Routine */
        Printing = true;

        /* Initialize Leftover Text */
        string leftOverText = text;

        /* Foreach Character in Text */
        foreach (char c in text)
        {
            /* Print Character and Update Leftover text */
            tmptext.text += c;
            leftOverText = leftOverText.Substring(1);

            /* Wait Until Override Occurs or For Duration (Faster Wait if 'Z' is Held) */
            yield return new WaitUntilOrFor(() => overrideFlag, Input.GetKey(KeyCode.Z) ? printDelayFast : printDelay);

            /* If Override Detected */
            if(overrideFlag)
            {
                /* Reset Override Flag, Append Rest Of Text, and Break */
                overrideFlag = false;
                tmptext.text += leftOverText;
                yield break;
            }                      
        }

        /* Mark That There Is NO Running Print Routine */
        Printing = false;

    }

    /// <summary> Print little Arrow and waits until player presses 'Z'. </summary>
    public static IEnumerator WaitForContinue ()
    {
        /* Print Arrow and Initialize Offset */
        tmptext.text += "   " + "<sprite index=0>";
        int off = 0;

        /* Initialize Offset Delay Properties */
        int frameCount = 0;
        const int frameThresh = 3;

        /* While Z Is NOT Pressed */
        while (!Input.GetKeyDown(KeyCode.Z))
        {
            /* Update Arrow */
            tmptext.text = tmptext.text.Substring(0, tmptext.text.Length - "<sprite index=0>".Length);
            tmptext.text += "<sprite index=" + off.ToString() + ">";

            /* Wait */
            yield return null;

            /* Update Offset */
            off = (++frameCount / frameThresh) % tmptext.spriteAsset.spriteGlyphTable.Count;

        }

        /* Play Sound And Remove Arrow */
        SoundManager.PlaySound(continueSound);
        tmptext.text = tmptext.text.Substring(0, tmptext.text.Length - "<sprite index=0>".Length);

    }

}

