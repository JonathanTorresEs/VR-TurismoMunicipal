Shader "Hidden/DigitalSalmon/UI/RoundedBox" 
{
	Properties
	{
		_BaseColor("[Base] Color", Color) = (0,0,0,0)
		_OutlineColor("[Outline] Color", Color) = (0,0,0,0)
		_OutlineInfo("[Outline] Info", Vector) = (0,0,0,0)
	}
	SubShader
	{
		Pass
		{
			CGPROGRAM

			#include "UnityCG.cginc"

			#pragma vertex vert_min
			#pragma fragment frag	

			uniform float4	_BaseColor;
			uniform float4	_OutlineColor;
			uniform float4	_OutlineInfo;
			uniform float	_Smoothing;

			struct appdata_min
			{
				float4 uv		: TEXCOORD0;
				float4 vertex	: POSITION; 
			};

			struct v2f_min
			{
				float2 uv		: TEXCOORD0;
				float4 vertex	: SV_POSITION;
			};

			// #pragma vertex vert_min
			v2f_min vert_min(appdata_min v)
			{
				v2f_min o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv;
				return o;
			}
			
			// Remap 0->1 uv space to -0.5 -> 0.5 uv space.
			float2 uvrQuad(float2 uv) {
				return uv - float2(0.5, 0.5);
			}

			// 2D Box Rounded
			float udRoundBox(float2 domain, float2 size, float radius) {
				size -= radius;
				return length(max(abs(domain) - size, 0.0)) - radius;
			}

			// Custom smoothing sampling.
			float sampleSmooth(float field, float smoothing) {
				smoothing /= 100;
				return smoothstep(0, -smoothing, field);
			}
			
			fixed4 frag (v2f_min i) : SV_Target
			{				
				float outputWidth = _OutlineInfo.r;
				float outlineThickness = _OutlineInfo.g;
				float outerOutlineRadius = _OutlineInfo.b / outputWidth;
				float smoothing = _OutlineInfo.a * outputWidth / 100;

				float2 fieldUv = uvrQuad(i.uv);
				float field = udRoundBox(fieldUv,0.5 - (outlineThickness/ (outputWidth)), outerOutlineRadius);
				
				float innerMask = sampleSmooth(field, smoothing);
				float outerMask = sampleSmooth(field - (outlineThickness / (outputWidth)), smoothing);

				float4 col = lerp(_OutlineColor, _BaseColor, innerMask);
				
				if (_OutlineColor.a == 0) 
				{
					col.rgb = _BaseColor.rgb;
				}
				if (_BaseColor.a == 0) 
				{
					col.rgb = _OutlineColor.rgb;
				}

				col.a = min(col.a, outerMask);

				return col;
			}
			ENDCG
		}
	}
}
