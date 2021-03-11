Shader "MARS/CompositeBlit"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {} // Composite Camera
    }
    SubShader
    {
        Tags { }
        Cull Off ZWrite Off

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
            #pragma multi_compile _ DESATURATE_OVERLAY DESATURATE_BASE
            #pragma multi_compile COMPOSITEUNDER _

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
            sampler2D _CompositeBlitOverlayTex;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            float4 frag (v2f i) : SV_Target
            {
#if COMPOSITEUNDER
                float4 a = saturate(tex2D(_CompositeBlitOverlayTex, i.uv));
                float4 b = saturate(tex2D(_MainTex, i.uv));
#else // composite under
                float4 a = saturate(tex2D(_MainTex, i.uv));
                float4 b = saturate(tex2D(_CompositeBlitOverlayTex, i.uv));
#endif
#if DESATURATE_OVERLAY
                a.rgb = (a.r + a.g + a.b) * 0.33334;
#endif
#if DESATURATE_BASE
                b.rgb = (b.r + b.g + b.b) * 0.33334;
#endif
                float4 col;
                // a over b
                col.rgb = (a.rgb * a.a) + (b.rgb * (1-a.a));
                col.a = 1;
                return col;
            }
            ENDCG
        }
    }
}
