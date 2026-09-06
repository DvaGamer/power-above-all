Shader "PowerAboveAll/PowderWashAlpha"
{
    Properties
    {
        _MainTex ("Powder wash", 2D) = "white" {}
        _Color ("Linen tint and age fade", Color) = (0.96, 0.95, 0.88, 0.46)
    }
    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" "IgnoreProjector"="True" }
        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off
        Cull Off

        Pass
        {
            Name "POWDER_WASH"
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D _MainTex;
            float4 _MainTex_ST;
            fixed4 _Color;

            struct Input
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct Interpolators
            {
                float4 position : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            Interpolators vert(Input input)
            {
                Interpolators output;
                output.position = UnityObjectToClipPos(input.vertex);
                output.uv = TRANSFORM_TEX(input.uv, _MainTex);
                return output;
            }

            fixed4 frag(Interpolators input) : SV_Target
            {
                // Maske ve yaş alfası doğrudan çarpılır; aydınlık opak yüzey oluşmaz.
                return tex2D(_MainTex, input.uv) * _Color;
            }
            ENDCG
        }
    }
    FallBack Off
}
