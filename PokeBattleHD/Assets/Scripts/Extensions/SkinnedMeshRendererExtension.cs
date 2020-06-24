using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class SkinnedMeshRendererExtension
{
    /// <summary> 
    /// Gradually changes all the emission colors of this
    /// skinned mesh renderer's material. Use 'ClearEmission' to remove emission. 
    /// </summary>
    /// <param name="smr"> Skinned mesh renderer to get materials from. </param>
    /// <param name="color"> The final emissive color. </param>
    /// <param name="intenstyMultiplier"> Value to multiply to color to get HDR color. 1 is normal color. 2 is glow. </param>
    /// <param name="speed"> Linear speed of transition. </param>
    public static IEnumerator ToEmmisiveHDRColorOverTime(this SkinnedMeshRenderer smr, Color color, float intenstyMultiplier, float speed)
    {
        /* Get HDR Color */
        Vector4 hdrColor = color * intenstyMultiplier;   

        /* Set All Material Exposure Weight To 0 So That They Can Emit Light */
        foreach(Material material in smr.materials)
            material.SetFloat("_EmissiveExposureWeight", 0);

        /* Get Starting Color */
        Vector4 currentColor = smr.material.GetColor("_EmissiveColor");

        /* While Current Color Is NOT Desired Color */
        while (currentColor != hdrColor)
        {
            /* Wait Till Next Frame */
            yield return null;

            /* Move Current Color Towards Desired Color */
            currentColor = Vector4.MoveTowards(currentColor, hdrColor, Time.deltaTime * speed);

            /* Update All Material Emissive Colors */
            foreach (Material material in smr.materials)
                material.SetColor("_EmissiveColor", currentColor);

        }
    }

    /// <summary> 
    /// Instantly changes all the emission colors of this
    /// skinned mesh renderer's material. Use 'ClearEmission' to remove emission. 
    /// </summary>
    /// <param name="smr"> Skinned mesh renderer to get materials from. </param>
    /// <param name="color"> The final emissive color. </param>
    /// <param name="intenstyMultiplier"> Value to multiply to color to get HDR color. 1 is normal color. 2 is glow. </param>
    public static void ToEmmisiveHDRColor(this SkinnedMeshRenderer smr, Color color, float intenstyMultiplier)
    {
        /* Get HDR Color */
        Vector4 hdrColor = color * intenstyMultiplier;

        /* Set All Material Exposure Weight To 0 So That They Can Emit Light and Their Colors */
        foreach(Material material in smr.materials) {
            material.SetFloat("_EmissiveExposureWeight", 0);
            material.SetColor("_EmissiveColor", hdrColor);

        }
    }



    /// <summary>
    /// Gradually clears emission of materials.
    /// </summary>
    /// <param name="smr"> Skinned mesh renderer to get materials of. </param>
    /// <param name="speed"> Speed of change. </param>
    public static IEnumerator ClearEmmisionOverTime(this SkinnedMeshRenderer smr, float speed)
    {
        /* Get Current Emission Weight Of Materials */
        float currentWeight = smr.material.GetFloat("_EmissiveExposureWeight");

        /* While Current Weight Is NOT 1 */
        while (currentWeight != 1)
        {
            /* Wait Till Next Frame */
            yield return null;

            /* Move Current Weight Towards 1 */
            currentWeight = Mathf.MoveTowards(currentWeight, 1, Time.deltaTime * speed);

            /* Update All Material Weights */
            foreach (Material material in smr.materials)
                material.SetFloat("_EmissiveExposureWeight", currentWeight);

        }

        /* Change All HDR Colors Back To Default */
        foreach (Material material in smr.materials)
            material.SetColor("_EmissiveColor", Color.black);

    }

    /// <summary>
    /// Instantly clears emission of materials.
    /// </summary>
    /// <param name="smr"> Skinned mesh renderer to get materials of. </param>
    /// <param name="speed"> Speed of change. </param>
    public static void ClearEmmision(this SkinnedMeshRenderer smr)
    {
        /* Change Emission Weight To 1 and Color To Black */
        foreach (Material material in smr.materials) {
            material.SetFloat("_EmissiveExposureWeight", 1);
            material.SetColor("_EmissiveColor", Color.black);

        }
    }
}
