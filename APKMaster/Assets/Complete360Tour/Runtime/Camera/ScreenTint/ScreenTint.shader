Shader "Hidden/DigitalSalmon/FX/ScreenTint" 
{
    Properties 
	{
	    _MainTex("MainTex", 2D) = "white" {}
        _Tint ("Tint", Color) = (0,0,0,0)
    }
    SubShader 
	{

		Cull Off ZWrite Off ZTest Always

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

			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv;
				return o;
			}

            uniform float4 _Tint;
            uniform sampler2D _MainTex; uniform float4 _MainTex_ST;

            float4 frag(v2f i) : SV_TARGET
			{
				float2 screenUV = UnityStereoScreenSpaceUVAdjust(i.uv, _MainTex_ST);
				
                float4 tex = tex2D(_MainTex, screenUV);			
                return float4(lerp(tex.rgb, _Tint.rgb, _Tint.a), 1);
            }
            ENDCG
        } 
    } 
	Fallback Off
}
