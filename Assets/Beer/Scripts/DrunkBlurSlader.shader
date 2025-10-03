Shader "Custom/DrunkBlurShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Strength ("Blur Strength", Range(0,1)) = 0
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float _Strength;

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            float4 frag(v2f i) : SV_Target
            {
                float2 uv = i.uv;

                // blur sample rondom
                float offset = _Strength * 0.02;
                float4 col = tex2D(_MainTex, uv) * 0.36;
                col += tex2D(_MainTex, uv + float2(offset,0)) * 0.16;
                col += tex2D(_MainTex, uv + float2(-offset,0)) * 0.16;
                col += tex2D(_MainTex, uv + float2(0,offset)) * 0.16;
                col += tex2D(_MainTex, uv + float2(0,-offset)) * 0.16;

                return col;
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
}
