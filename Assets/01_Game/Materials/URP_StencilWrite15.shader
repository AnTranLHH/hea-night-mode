Shader "Custom/URP_StencilWrite15"
{
    Properties
    {
        _Color("Color (if visible)", Color) = (1,1,1,1)
    }

    SubShader
    {
        Tags { "RenderPipeline"="UniversalPipeline" "Queue"="Geometry" "RenderType"="Opaque" }

        Pass
        {
            Name "StencilWrite"
            Tags { "LightMode" = "UniversalForward" }

            // Culling / Depth / Color behavior - change as needed
            Cull Off                // change to Back/Front as you need
            ZWrite Off             // set Off if you DO NOT want depth written
            ZTest Always           // depth comparison

            // ColorMask 0 means "don't write any color" (invisible mask).
            // change to ColorMask 15 if you want the object to also render its color.
            ColorMask 0

            // Stencil block: writes Ref = 88 whenever this pass runs
            Stencil
            {
                Ref 15
                Comp Always   // always pass stencil test
                Pass Replace  // replace stencil value with Ref
                ReadMask 255
                WriteMask 255
            }

            HLSLPROGRAM
            #pragma vertex Vert
            #pragma fragment Frag
            #pragma target 3.0

            // URP core helper
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes
            {
                float3 positionOS : POSITION;
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
            };

            Varyings Vert(Attributes v)
            {
                Varyings o;
                // Unity helper to transform object space to clip space
                o.positionCS = TransformObjectToHClip(float4(v.positionOS, 1.0));
                return o;
            }

            float4 _Color;

            // Fragment doesn't matter when ColorMask 0; keep simple
            half4 Frag(Varyings i) : SV_Target
            {
                return _Color;
            }
            ENDHLSL
        } // Pass
    } // SubShader

    FallBack "Universal Render Pipeline/Unlit"
}
