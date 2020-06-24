Shader "Hidden/Shader/Transition"
{
    HLSLINCLUDE

    #pragma target 4.5
    #pragma only_renderers d3d11 ps4 xboxone vulkan metal switch

    #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Common.hlsl"
    #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
    #include "Packages/com.unity.render-pipelines.high-definition/Runtime/ShaderLibrary/ShaderVariables.hlsl"
    #include "Packages/com.unity.render-pipelines.high-definition/Runtime/PostProcessing/Shaders/FXAA.hlsl"
    #include "Packages/com.unity.render-pipelines.high-definition/Runtime/PostProcessing/Shaders/RTUpscale.hlsl"

    struct Attributes
    {
        uint vertexID : SV_VertexID;
        UNITY_VERTEX_INPUT_INSTANCE_ID
    };

    struct Varyings
    {
        float4 positionCS : SV_POSITION;
        float2 texcoord   : TEXCOORD0;
        UNITY_VERTEX_OUTPUT_STEREO
    };

    Varyings Vert(Attributes input)
    {
        Varyings output;
        UNITY_SETUP_INSTANCE_ID(input);
        UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);
        output.positionCS = GetFullScreenTriangleVertexPosition(input.vertexID);
        output.texcoord = GetFullScreenTriangleTexCoord(input.vertexID);
        return output;
    }

    // List of properties to control your post process effect
    float _Intensity;
    float _CutoffAlpha;
    int _Darken;
    TEXTURE2D_X(_ScreenTex);
    sampler2D _MainTex;
    float2 _Tiling;
    float2 _Offset;

    float4 CustomPostProcess(Varyings input) : SV_Target
    {
        /* Configure This Input For VR Compatibility */
        UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);
       
        /* Declare Screen Color */
        float4 screenColor;

        /* Get Screen Color from Source Camera Render Texture At Screen Coordinates If Darken Disabled */
        if (_Darken == 0)
            screenColor = LOAD_TEXTURE2D_X(_ScreenTex, input.texcoord * _ScreenSize.xy);

        /* Else Simple Make The Screen Color Return Black */
        else
            screenColor = float4(0, 0, 0, 1);

        /* Get Texture Color At Screen Coordinates */
        float4 colorMain = tex2D(_MainTex, input.texcoord);

        /* Get Interpolation Of Screen Color To Texture Color Based On Cutoff */
        if (colorMain.w < _CutoffAlpha)
            colorMain = colorMain;
        else
            colorMain = screenColor;

        
        /* Return Interpolation Of Screen Color To Texture Color Based On Intensity */
        return lerp(screenColor, colorMain, _Intensity);
    }

    ENDHLSL

    SubShader
    {
        Pass
        {
            Name "Transition"

            ZWrite Off
            ZTest Always
            Blend Off
            Cull Off

            HLSLPROGRAM
                #pragma fragment CustomPostProcess
                #pragma vertex Vert
            ENDHLSL
        }
    }
    Fallback Off
}
