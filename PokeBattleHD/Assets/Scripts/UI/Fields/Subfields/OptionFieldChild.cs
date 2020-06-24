using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OptionFieldChild : MonoBehaviour
{
    /* Private Properties */
    private List<ImageHelper> imageHelpers = null;
    private Vector3 s = Vector3.zero;

    protected virtual void Awake()
    {
        /* Get All Image Helpers */
        Image[] images = GetComponentsInChildren<Image>();
        imageHelpers = new List<ImageHelper>();
        foreach (Image image in images)
            imageHelpers.Add(new ImageHelper(image));

    }

    protected virtual void OnEnable()
    {
        /* Scale Up */
        StartCoroutine(ScaleUp());

        /* Return If No Images Found */
        if (imageHelpers == null)
            return;

        /* Phase In */
        foreach (ImageHelper imageHelper in imageHelpers)
            StartCoroutine(imageHelper.PhaseIn());
        
    }

    /// <summary> Makes field small then enlarges it over time. </summary>
    protected IEnumerator ScaleUp()
    {
        /* If Local Size Isn't Saved, Save It */
        if (s == Vector3.zero)
            s = transform.localScale;

        /* Decrease Size */
        transform.localScale /= 10;

        /* Lerp Overtime To Original Scale */
        while(transform.localScale != s) {
            transform.localScale = Vector3.Lerp(transform.localScale, s, Time.deltaTime * 16);
            yield return null;
        }

    }

    private class ImageHelper
    {
        /* Private Properties */
        readonly Image image;
        Color c = Color.clear;

        /// <summary> Create image helper. </summary>
        /// <param name="image"> Image to use. </param>
        public ImageHelper(Image image)
        {
            this.image = image;
        }

        /// <summary> Changes image color from black to its orignal color over time. </summary>
        public IEnumerator PhaseIn()
        {
            /* Set Original Color If NOT Set */
            if (c == Color.clear)
                c = image.color;

            /* Clear Image */
            image.color = Color.clear;

            /* Lerp Overtime To Original Color */
            while (image.color != c)
            {
                image.color = Color.Lerp(image.color, c, Time.deltaTime * 16);
                yield return null;
            }
        }

    }
}
