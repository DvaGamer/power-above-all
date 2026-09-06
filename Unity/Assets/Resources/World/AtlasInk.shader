Shader "PowerAboveAll/AtlasInk"
{
    Properties { _Color ("Ink tint", Color) = (1,1,1,1) _MainTex ("Paper", 2D) = "white" {} }
    SubShader
    {
        Tags { "RenderType"="Opaque" "Queue"="Geometry" }
        Cull Off ZWrite On
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
            struct appdata { float4 vertex:POSITION; float2 uv:TEXCOORD0; fixed4 color:COLOR; };
            struct v2f { float4 vertex:SV_POSITION; float2 uv:TEXCOORD0; fixed4 color:COLOR; };
            sampler2D _MainTex; float4 _MainTex_ST; fixed4 _Color;
            v2f vert(appdata v) { v2f o; o.vertex=UnityObjectToClipPos(v.vertex); o.uv=TRANSFORM_TEX(v.uv,_MainTex); o.color=v.color; return o; }
            fixed4 frag(v2f i):SV_Target { return fixed4(tex2D(_MainTex,i.uv).rgb*_Color.rgb*i.color.rgb,1); }
            ENDCG
        }
    }
}
