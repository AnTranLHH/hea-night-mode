Shader "Custom/UnlitViewNormalStencilMask"
{
    Properties
    {
        _MainTex ("Main Texture", 2D) = "white" {}
        _MaskValue ("Stencil Mask Value", Range(0,255)) = 1
    }
    SubShader
    {
        Tags { "Queue"="Geometry" "RenderType"="Opaque" }

        // Stencil test: Only draw if current stencil != MaskValue
        Stencil
        {
            Ref [_MaskValue]
            Comp NotEqual   // Render only if stencil != Ref
            Pass Keep
        }

        Pass
        {
            Name "UnlitViewNormal"
            Cull Back
            ZWrite On
            ZTest LEqual

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float _MaskValue;

            struct appdata
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float2 uv : TEXCOORD0;
                float3 worldNormal : TEXCOORD1;
                float3 viewDir : TEXCOORD2;
            };

            v2f vert (appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.worldNormal = UnityObjectToWorldNormal(v.normal);
                float3 worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                o.viewDir = _WorldSpaceCameraPos.xyz - worldPos;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float3 normal = normalize(i.worldNormal);
                float3 viewDir = normalize(i.viewDir);
                float ndotv = saturate(dot(normal, viewDir));

                fixed4 tex = tex2D(_MainTex, i.uv);
                fixed4 color = tex * ndotv;

                return color;
            }
            ENDCG
        }
    }
    FallBack Off
}
