Shader "Custom/TrailAdditiveTop"
{
    Properties
    {
        _MainTex     ("Texture", 2D)              = "white" {}
        _GlobalAlpha ("Global Alpha", Range(0,1)) = 1
    }

    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" }
        Lighting Off
        ZWrite On
        ZTest  Less
        Blend  SrcAlpha OneMinusSrcAlpha
        Cull   Off

        Pass
        {
            CGPROGRAM
            #pragma vertex   vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D _MainTex;
            fixed     _GlobalAlpha;

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv     : TEXCOORD0;
                fixed4 color  : COLOR;
            };

            struct v2f
            {
                float2 uv     : TEXCOORD0;
                fixed4 color  : COLOR;
                float4 vertex : SV_POSITION;
            };

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv     = v.uv;
                o.color  = v.color;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 tex = tex2D(_MainTex, i.uv);
                fixed4 col = tex * i.color;

                col.a *= _GlobalAlpha;
                return col;
            }
            ENDCG
        }
    }
}
