Shader "Hidden/DigitalSalmon/C360/StereoEvaluate"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
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
			
			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				return o;
			}

			float Simplex1D(float input, float iterations) {
				if (iterations == 0)
					return 0;

				float ret = 0;

				for (int i = 0; i < iterations; ++i)
				{
					float2 p = floor(input * (i + 1));
					float2 f = frac(input * (i + 1));
					f = f * f * (3.0 - 2.0 * f);
					float n = p.x + p.y * 57.0;
					float4 noise = float4(n, n + 1, n + 57.0, n + 58.0);
					noise = frac(sin(noise)*437.585453);
					ret += lerp(lerp(noise.x, noise.y, f.x), lerp(noise.z, noise.w, f.x), f.y) * (iterations / (i + 1));
				}
				return ret / (iterations);
			}

			float4 BlurredTexSample(sampler2D tex, float2 uv, float blurDistance, float sampleCount)
			{
				sampleCount = (int)sampleCount;
				blurDistance /= 100;
				float4 v = 0;
				for (int i = 0; i < sampleCount; i++)
				{
					float n = ((float)i / sampleCount);
					float noise = Simplex1D(n, 2) - 0.5;
					float2 sampleUV = uv + (float2(cos(noise * 3.14 * 2), sin(noise * 3.14 * 2)) * n * blurDistance);
					v += tex2D(tex, sampleUV);
				}

				v /= sampleCount;

				return v;
			}

			float4 CalculateDelta(sampler2D tex, float2 uv)
			{
				float4 delta = float4(10,10,10,1);
				
				float searchDistance = lerp(0.012, 0.003, 1 - (distance(uv.y, 0.5) * 2));

				for (int i = -14; i < 14; i++)
				{
					float2 fUV = uv + float2(i * searchDistance, 0);
					fUV.y /= 2;

					float4 t = BlurredTexSample(tex, fUV, 1, 2);
					fUV.y += 0.5;
					float4 b = BlurredTexSample(tex, fUV, 1, 2);

					delta =  min(delta, abs(b - t));
				}

				return delta;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				fixed4 col = CalculateDelta(_MainTex, i.uv);
				
				return col;
			}
			ENDCG
		}
	}
}
