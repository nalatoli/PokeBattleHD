using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;

[System.Serializable, VolumeComponentMenu("Post-processing/Custom/Transition")]
public sealed class Transition : CustomPostProcessVolumeComponent, IPostProcessComponent
{
    [Tooltip("Controls the intensity of the effect.")]
    public ClampedFloatParameter intensity = new ClampedFloatParameter(0f, 0f, 1f);

    [Tooltip("Controls cutoff in effect.")]
    public ClampedFloatParameter cutoffAlpha = new ClampedFloatParameter(0, 0, 1);

    [Tooltip("Texture to overlay across screen.")]
    public TextureParameter texture = new TextureParameter(null);

    [Tooltip("Turns screen entirely black if enabled.")]
    public BoolParameter darken = new BoolParameter(false);

    [Tooltip("Tiling of texture. (1,1) for single image.")]
    public Vector2Parameter tiling = new Vector2Parameter(new Vector2(1,1));

    [Tooltip("Texture offset. (0,0) for no offset.")]
    public Vector2Parameter offset = new Vector2Parameter(new Vector2(0, 0));

    /// <summary> Material to be assigned to camera render. </summary>
    private Material material;

    /// <summary> Determines if this effect to setup, render, and cleanup. </summary>
    public bool IsActive() => material != null && intensity.value > 0f && texture.value != null;

    /// <summary> After post-process injection point in render pipeline.</summary>
    public override CustomPostProcessInjectionPoint injectionPoint => CustomPostProcessInjectionPoint.AfterPostProcess;

    /// <summary> Directory of shader tied to this effect. </summary>
    const string kShaderName = "Hidden/Shader/Transition";

    public override void Setup()
    {
        if (Shader.Find(kShaderName) != null)
            material = new Material(Shader.Find(kShaderName));
        else
            Debug.LogError($"Unable to find shader '{kShaderName}'. Post Process Volume Transition is unable to load.");
    }

    public override void Render(CommandBuffer cmd, HDCamera camera, RTHandle source, RTHandle destination)
    {
        if (material == null)
            return;

        material.SetFloat("_Intensity", intensity.value);
        material.SetFloat("_CutoffAlpha", cutoffAlpha.value);
        material.SetInt("_Darken", darken.value ? 1 : 0);
        material.SetTexture("_ScreenTex", source);
        material.SetTexture("_MainTex", texture.value);
        material.SetVector("_Tiling", tiling.value);
        material.SetVector("_Offset", offset.value);
        HDUtils.DrawFullScreen(cmd, material, destination);
    }

    public override void Cleanup()
    {
        CoreUtils.Destroy(material);
    }
}
