Shader "Custom/ScreenRadialMask"
{
    Properties
    {
        _DarkColor ("Dark Color", Color) = (0,0,0,0.95)
        _Center ("Center (Viewport XY)", Vector) = (0.5, 0.5, 0, 0)
        _Radius ("Radius", Float) = 0.35
        _Feather ("Feather", Float) = 0.15
    }
    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" }
        ZWrite Off
        Cull Off
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            fixed4 _DarkColor;
            float4 _Center;
            float _Radius;
            float _Feather;

            struct appdata { float4 vertex : POSITION; float2 uv : TEXCOORD0; };
            struct v2f { float4 pos : SV_POSITION; float2 uv : TEXCOORD0; };

            v2f vert (appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float2 p = i.uv;                   // 0..1 across the screen
                float2 c = _Center.xy;             // 0..1 viewport center of the hole
                float d = distance(p, c);

                 // edge = 0 inside the hole, 1 fully dark outside
                float edge = smoothstep(_Radius - _Feather, _Radius, d);

                fixed4 col = _DarkColor;
                col.a = _DarkColor.a * edge;       // transparent center, dark outside
                return col;
            }
            ENDCG
        }
    }
}
