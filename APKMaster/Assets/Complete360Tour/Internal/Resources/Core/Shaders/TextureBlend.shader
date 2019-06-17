Shader "Hidden/DigitalSalmon/UI/TextureBlend"
{
	Properties
	{
		_MainTex ("[Texture] A", 2D) = "white" {}
		_TextureB("[Texture] B", 2D) = "white" {}
		_Area ("[Rect] Area", Vector) = (0,0,0,0)
		_Alpha ("[Blend] Alpha", Float) = 1
	
	}
	SubShader
	{
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
			fixed4 _MainTex_ST;
			sampler2D _TextureB;
			float4 _Area;
			float _Alpha;

			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				return o;
			}

			float2 ApplyAreaMap(float2 uv, float4 area){

				float width = area.z;
				float height = area.w;
				float2 rUV = uv;
				rUV -= float2(area.x, area.y);
				rUV /= float2(width, height);
				return rUV;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				fixed4 textureA = tex2D(_MainTex, i.uv);
				fixed4 textureB = tex2D(_TextureB, ApplyAreaMap(i.uv, _Area));
				textureB.a *= _Alpha; 
				textureA.rgb = lerp(textureA.rgb, textureB.rgb, textureB.a);
				return textureA;
			}
			ENDCG
		}
	}
}
