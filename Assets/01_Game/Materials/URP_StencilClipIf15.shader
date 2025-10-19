Shader "Custom/URP_StencilClipIf15"
{
    Properties
    {
        _BaseMap("Base Texture", 2D) = "white" {}
        _BaseColor("Base Color (with Alpha)", Color) = (1,1,1,1)
        _Alpha("Global Alpha", Range(0,1)) = 1
    }

    SubShader
    {
        Tags 
        { 
            "RenderPipeline"="UniversalPipeline" 
            "RenderType"="Transparent" 
            "Queue"="Transparent" 
        }

        Pass
        {
            Name "ForwardUnlit"
            Tags { "LightMode"="UniversalForward" }

            //T ransparent rendering setup
            Blend SrcAlpha OneMinusSrcAlpha
            ZWrite Off        // Usually off for transparent objects
            ZTest LEqual
            Cull Back

            // Stencil test: Skip rendering where stencil == 15
            Stencil
            {
                Ref 15
                Comp NotEqual
                Pass Keep
                Fail Keep
                ZFail Keep
            }

            HLSLPROGRAM
            #pragma vertex Vert
            #pragma fragment Frag
            #pragma target 3.0

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes
            {
                float3 positionOS : POSITION;
                float2 uv         : TEXCOORD0;
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float2 uv         : TEXCOORD0;
            };

            TEXTURE2D(_BaseMap);
            SAMPLER(sampler_BaseMap);

            float4 _BaseColor;
            float _Alpha;

            Varyings Vert(Attributes v)
            {
                Varyings o;
                o.positionCS = TransformObjectToHClip(float4(v.positionOS, 1.0));
                o.uv = v.uv;
                return o;
            }

            half4 Frag(Varyings i) : SV_Target
            {
                half4 texColor = SAMPLE_TEXTURE2D(_BaseMap, sampler_BaseMap, i.uv);
                texColor *= _BaseColor;
                texColor.a *= _Alpha; // Final alpha control
                return texColor;
            }
            ENDHLSL
        }
    }

    FallBack Off
}
