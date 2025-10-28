Shader "Custom/ScreenRadialMask"
{
    Properties
    {
        [PerRendererData]_MainTex ("Sprite Texture", 2D) = "white" {} 
        _Color    ("Tint", Color) = (1,1,1,1)

        _Radius   ("Radius", Range(0,1)) = 0.3
        _Feather  ("Feather", Range(0,1)) = 0.18
        _Center   ("Center (Viewport XY)", Vector) = (0.5,0.5,0,0)
        _MaskColor("Mask Color", Color) = (0,0,0,0.95)
    }

    SubShader
    {
        Tags {
            "Queue"="Transparent"
            "IgnoreProjector"="True"
            "RenderType"="Transparent"
            "CanUseSpriteAtlas"="True"
        }
        Cull Off
        Lighting Off
        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
            #pragma vertex   vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata_t {
                float4 vertex : POSITION;
                float2 uv     : TEXCOORD0;
            };
            struct v2f {
                float4 pos     : SV_POSITION;
                float2 uv      : TEXCOORD0;
                float2 screenUV: TEXCOORD1;
            };

            sampler2D _MainTex;      
            float4    _MainTex_ST;
            fixed4    _Color;

            float     _Radius;
            float     _Feather;
            float4    _Center;       // xy = viewport center
            fixed4    _MaskColor;

            v2f vert (appdata_t v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv  = TRANSFORM_TEX(v.uv, _MainTex);

                float2 ndc = o.pos.xy / o.pos.w;    // -1..1
                o.screenUV = ndc * 0.5 + 0.5;       //  0..1
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 baseCol = tex2D(_MainTex, i.uv) * _Color;

                float2 delta = i.screenUV - _Center.xy;
                float  dist  = length(delta);

                // a = 0 inside light, 1 outside (soft edge)
                float edge0 = _Radius;
                float edge1 = _Radius + max(_Feather, 1e-5) * 0.5;
                float a = smoothstep(edge0, edge1, dist);

                // transparent in the lit circle, dark outside
                return lerp(fixed4(0,0,0,0), _MaskColor, a);
            }
            ENDCG
        }
    }

    FallBack "UI/Default"
}
