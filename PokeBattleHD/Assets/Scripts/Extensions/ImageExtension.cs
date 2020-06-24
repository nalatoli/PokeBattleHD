using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary> Helper class for images and TMPro text. </summary>
public static class ImageExtension
{
    /// <summary> Changes alpha of image over time. Ends when alpha reaches 'a'</summary>
    /// <param name="image"> Image to change color of. </param>
    /// <param name="a"> Final alpha. </param>
    /// <param name="speed"> Speed to multiply to Time.deltaTime. </param>
    public static IEnumerator ChangeAlphaOvertime(this Image image, float a, float speed)
    {
        /* Get Current Alpha */
        float currentAlpha = image.color.a;

        while (currentAlpha != a) {
            currentAlpha = Mathf.Lerp(currentAlpha, a, Time.deltaTime * speed);
            image.SetAlpha(currentAlpha);
            yield return null;

        }
    }

    /// <summary> Changes alpha of text over time. Ends when alpha reaches 'a'</summary>
    /// <param name="text"> Text to change color of. </param>
    /// <param name="a"> Final alpha. </param>
    /// <param name="speed"> Speed to multiply to Time.deltaTime. </param>
    public static IEnumerator ChangeAlphaOvertime(this TextMeshProUGUI text, float a, float speed)
    {
        /* Get Current Alpha */
        float currentAlpha = text.color.a;

        while (currentAlpha != a) {
            currentAlpha = Mathf.Lerp(currentAlpha, a, Time.deltaTime * speed);
            text.SetAlpha(currentAlpha);
            yield return null;

        }
    }

    /// <summary> Changes color of image over time. Ends when color reaches 'color'</summary>
    /// <param name="image"> Image to change color of. </param>
    /// <param name="color"> Final color. </param>
    /// <param name="speed"> Speed to multiply to Time.deltaTime. </param>
    public static IEnumerator ChangeColorOvertime(this Image image, Color color, float speed)
    {
        while(image.color != color) {
            image.color = Color.Lerp(image.color, color, Time.deltaTime * speed);
            yield return null;

        }
    }

    /// <summary> Changes vertex color of text over time. Ends when color reaches 'color'</summary>
    /// <param name="text"> Text to change color of. </param>
    /// <param name="color"> Final color. </param>
    /// <param name="speed"> Speed to multiply to Time.deltaTime. </param>
    public static IEnumerator ChangeColorOvertime(this TextMeshProUGUI text, Color color, float speed)
    {
        while (text.color != color) {
            text.color = Color.Lerp(text.color, color, Time.deltaTime * speed);
            yield return null;
        }
    }

    /// <summary> Changes alpha of image's color. </summary>
    /// <param name="image"> Image to change. </param>
    /// <param name="a"> Alpha value to set the image's alpha to. </param>
    public static void SetAlpha(this Image image, float a)
    {
        image.color = new Color(image.color.r, image.color.g, image.color.b, a);
    }

    /// <summary> Changes alpha of text's color. </summary>
    /// <param name="text"> Text to change. </param>
    /// <param name="a"> Alpha value to set the image's alpha to. </param>
    public static void SetAlpha(this TextMeshProUGUI text, float a)
    {
        text.color = new Color(text.color.r, text.color.g, text.color.b, a);
    }

    /// <summary>
    /// Constantly adjusts the alpha of an image from its original alpha to 'a'.
    /// This coroutine runs as long as the image is active in the hierarchy.
    /// </summary>
    /// <param name="image"> Image to adjust alpha of. </param>
    /// <param name="aHigh"> Highest alpha to fluctuate to. </param>
    /// <param name="aLow"> Lowest alpha to fluctuate to. </param>
    public static IEnumerator FluxAlpha(this Image image, float aHigh, float aLow)
    {
        /* Initialize Elapsed Time */
        float elapsedTime = 0;

        /* Determine Middle Alpha and Sin Multiplication Factor */
        float aMedium = (aHigh + aLow) / 2;
        float sinFactor = aHigh - aMedium;

        /* Change Alpha Of Image */
        while (image.gameObject.activeInHierarchy) {
            SetAlpha(image, aMedium - sinFactor * Mathf.Sin(elapsedTime * 4));
            yield return null;
            elapsedTime += Time.deltaTime;
        }
    }

    /// <summary>
    /// Constantly adjusts the alpha of text from its original alpha to 'a'.
    /// This coroutine runs as long as the text is active in the hierarchy.
    /// </summary>
    /// <param name="text"> Text to adjust alpha of. </param>
    /// <param name="aHigh"> Highest alpha to fluctuate to. </param>
    /// <param name="aLow"> Lowest alpha to fluctuate to. </param>
    public static IEnumerator FluxAlpha(this TextMeshProUGUI text, float aHigh, float aLow)
    {
        /* Initialize Elapsed Time */
        float elapsedTime = 0;

        /* Determine Middle Alpha and Sin Multiplication Factor */
        float aMedium = (aHigh + aLow) / 2;
        float sinFactor = aHigh - aMedium;

        /* Change Alpha Of Text */
        while (text.gameObject.activeInHierarchy) {
            SetAlpha(text, aMedium - sinFactor * Mathf.Sin(elapsedTime * 4));
            yield return null;
            elapsedTime += Time.deltaTime;
        }
    }

    /// <summary>
    /// Constantly animates spritesheet on an image. 
    /// The animation will start from the current sprite if it is on the spritesheet.
    /// This coroutine runs as long as the image is active in the hierarchy.
    /// </summary>
    /// <param name="image"> The image to animate. </param>
    /// <param name="spritesheet"> List of sprites to iterate through. </param>
    /// <param name="speed"> Speed of sprite iterations. </param>
    public static IEnumerator AnimateSpritesheet(this Image image, List<Sprite> spritesheet, float speed)
    {
        /* Initialize Sprite Counter */
        int currentSpriteIndex = spritesheet.Contains(image.sprite) ? spritesheet.IndexOf(image.sprite) : 0;

        /* Keep Loading Sprites, Incrementing Counter, and Waiting */
        while (image.gameObject.activeInHierarchy) {
            image.sprite = spritesheet[currentSpriteIndex];
            currentSpriteIndex = ++currentSpriteIndex % spritesheet.Count;
            yield return new WaitForSeconds(Time.deltaTime / speed);
        }
    }


    /// <summary>
    /// Animates spritesheet on an image. Consecutive sprites will take longer and longer to load. 
    /// The animation will start from the current sprite if it is on the spritesheet.
    /// This coroutine will stop automatically.
    /// </summary>
    /// <param name="image"> The image to animate. </param>
    /// <param name="spritesheet"> List of sprites to iterate through. </param>
    public static IEnumerator AnimateSpritesheetToStop(this Image image, List<Sprite> spritesheet)
    {
        /* Initialize Sprite Counter */
        int currentSpriteIndex = spritesheet.Contains(image.sprite) ? spritesheet.IndexOf(image.sprite) : 0;

        /* Initialize Wait Length and Wait Length Multiplier */
        float waitLength = Time.deltaTime;
        float v = Time.deltaTime;

        /* Initalize Max Wait Length and Wait Length Multiplier Factor */
        const float maxWaitLength = 0.05f;
        const float vMultiplier = 1.4f;

        /* While Wait Length Does NOT Reach Max Wait Length */
        while (waitLength != maxWaitLength)
        {
            /* Load Sprite, Increment Counter, And Wait For Extended Duration */
            image.sprite = spritesheet[currentSpriteIndex];
            currentSpriteIndex = ++currentSpriteIndex % spritesheet.Count;
            yield return new WaitForSeconds(waitLength);

            /* Increase Wait Length With Smooth Start */
            waitLength = Mathf.Lerp(waitLength, maxWaitLength, v);
            v *= vMultiplier;

        }
    }

    /// <summary>
    /// Constantly adjusts color of an image in a non-linear way.
    /// This coroutine runs as long as the image is active in the hierarchy.
    /// </summary>
    /// <param name="image"> The image to adjust the color of. </param>
    /// <param name="cPrimary"> Primary color. This color will be shown more than the secondary color. </param>
    /// <param name="cSecondary"> Secondary color. This color will be shown less than the primary color.</param>
    /// <param name="speed"> Speed of fluctuations. </param>
    public static IEnumerator FluxColor(this Image image, Color cPrimary, Color cSecondary, float speed)
    {
        /* Initialize Elapsed Time */
        float elapsedTime = 0;

        /* Change Color Of Text */
        while (image.gameObject.activeInHierarchy) {
            image.color = Color.Lerp(cSecondary, cPrimary, Mathf.Abs(Mathf.Cos(elapsedTime * speed)));
            yield return null;
            elapsedTime += Time.deltaTime;
        }
    }

    /// <summary>
    /// Constantly adjusts color of text in a non-linear way.
    /// This coroutine runs as long as the text is active in the hierarchy.
    /// </summary>
    /// <param name="text"> The text to adjust the color of. </param>
    /// <param name="cPrimary"> Primary color. This color will be shown more than the secondary color. </param>
    /// <param name="cSecondary"> Secondary color. This color will be shown less than the primary color.</param>
    /// <param name="speed"> Speed of fluctuations. </param>
    public static IEnumerator FluxColor(this TextMeshProUGUI text, Color cPrimary, Color cSecondary, float speed)
    {
        /* Initialize Elapsed Time */
        float elapsedTime = 0;

        /* Change Color Of Text */
        while (text.gameObject.activeInHierarchy) {
            text.color = Color.Lerp(cSecondary, cPrimary, Mathf.Abs(Mathf.Sin(elapsedTime * speed)));
            yield return null;
            elapsedTime += Time.deltaTime;
        }
    }

}
