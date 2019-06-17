Shader "Complet360Tour/SimpleHotspot"
{
	Properties
	{
		_Icon("[Icon] Map", 2D) = "white" {}
		_IconColor("[Icon] Color", Color) = (1,1,1,1)
		_IconAlpha("[Icon] Alpha Multiplier", Float) = 1
		_IconScale("[Icon] Scale", Float) = 1

		_BaseColor("[Base] Color", Color) = (0,0,0,1)
		_FillColor("[Fill] Color", Color) = (1,1,1,1)

		_OuterRadius("[Ring] Outer Radius", Float) = 0
		_InnerRadius("[Ring] Inner Radius", Float) = 0

		_FillValue("[Fill] Value", Float) = 0
		_Smoothing("[Global] Smoothing", Float) = 1
		_Alpha("[Global] Alpha", Float) = 1
	}
	SubShader
	{
		Tags { "RenderType"="Transparent" "Queue"="Transparent" }
		Cull Off
		ZWrite Off

		Pass
		{
			Blend SrcAlpha OneMinusSrcAlpha

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"
			#include "Assets/Complete360Tour/Internal/Resources/Shaders/DigitalSalmon.Fields.cginc"

			uniform sampler2D _Icon; float4 _Icon_ST;
			uniform float4 _IconColor;
			uniform float _IconAlpha;
			uniform float _IconScale;

			uniform float4 _BaseColor;
			uniform float4 _FillColor;

			uniform float _OuterRadius;
			uniform float _InnerRadius;

			uniform float _FillValue;
			uniform float _Smoothing;
			uniform float _Alpha;

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float4 vertex : SV_POSITION;
				float2 uv : TEXCOORD0;
				float distance : TEXCOORD1;
			};

			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _Icon);
				o.distance = length(mul(unity_ObjectToWorld, v.vertex) - _WorldSpaceCameraPos);
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{		
				_Smoothing *= pow(i.distance,0.8);

				float2 quadUv = uvrQuad(i.uv);

				float ringInner = sdSphere(quadUv, _InnerRadius);
				float ringOuter = sdSphere(quadUv, _OuterRadius);
				
				float innerRingMask = sampleSmooth(ringInner, _Smoothing);
				float iconMask = sampleSmooth(ringInner+0.04, _Smoothing);
				float ringMask = sampleSmooth(opSubtraction(ringOuter, ringInner), _Smoothing);
				
				float fillField = (udRadial(quadUv) / (1 + _Smoothing/100)) - _FillValue;
				float fillMask = sampleSmooth(fillField, _Smoothing);

				float4 ringColor = lerp(_BaseColor, _FillColor, fillMask);
				ringColor.a = min(ringColor.a, ringMask);

				float2 iconUv = uvrScaleQuad(i.uv,_IconScale);
				float4 iconColor = tex2D(_Icon, iconUv);
				iconColor *= _IconColor;
				iconColor.a *= _IconAlpha;
				iconColor.a = min(iconColor.a, iconMask);

				float3 finalColor = lerp(ringColor.rgb, iconColor.rgb, iconColor.a);
				if (ringColor.a == 0) finalColor = iconColor.rgb;

				float finalAlpha = max(iconColor.a, ringColor.a);
				finalAlpha *= _Alpha;

				return float4(finalColor, finalAlpha); 
			}
			ENDCG
		}
	}
}
