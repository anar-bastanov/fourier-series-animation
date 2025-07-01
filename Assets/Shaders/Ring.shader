Shader "Sprites/ProceduralRing"
{
    Properties
    {
        _MainTex ("Sprite Texture", 2D) = "white" {}
        _Color ("Color", Color) = (1,1,1,1)
        _Radius ("Radius", Float) = 0.4
        _Thickness ("Thickness", Float) = 0.05
    }
    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" "IgnoreProjector"="True" "PreviewType"="Plane" "CanUseSpriteAtlas"="True" }
        LOD 100

        Pass
        {
            Blend SrcAlpha OneMinusSrcAlpha
            Cull Off
            ZWrite Off

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 2.0
            #include "UnityCG.cginc"

            sampler2D _MainTex;
            float4 _MainTex_ST;
            fixed4 _Color;
            float _Radius;
            float _Thickness;

            struct appdata_t
            {
                float4 vertex : POSITION;
                float2 texcoord : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            v2f vert(appdata_t IN)
            {
                v2f OUT;
                OUT.vertex = UnityObjectToClipPos(IN.vertex);
                OUT.uv = IN.texcoord * 2.0 - 1.0;
                return OUT;
            }

            fixed4 frag(v2f IN) : SV_Target
            {
                float dist = length(IN.uv);
                float halfThick = _Thickness / 2.0;
                float outer = _Radius + halfThick;
                float inner = _Radius - halfThick;

                if (dist < inner || dist > outer)
                    discard;

                return _Color;
            }
            ENDCG
        }
    }
}
