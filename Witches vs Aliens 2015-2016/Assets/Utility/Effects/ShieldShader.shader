Shader "Custom/ShieldShader"
{
	Properties
	{
		_MainTex("MainTexture", 2D) = "white" {}
		_NoiseTex1("NoiseTexture1", 2D) = "white" {}
		_NoiseTex2("NoiseTexture2", 2D) = "white" {}
		_ScrollSpeed("Noise Scroll Speed", Range(0, 1)) = 1
		_EffectStrength("Effect Strength", Float) = 1
		_StretchFactor("Stretch Factor", Float) = 1
		_MiddleCutoff("Magnitude of the selected region of noise", Range(0, 0.5)) = 0.1
	}
	SubShader
	{
		Tags { "RenderType"="Transparent" "IgnoreProjector"="True" "Queue"="Transparent"  "PreviewType"="Plane"}
		Blend SrcAlpha OneMinusSrcAlpha
		ZWrite Off
		Lighting Off

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
			sampler2D _NoiseTex1;
			sampler2D _NoiseTex2;
			fixed _ScrollSpeed;
			half _StretchFactor;
			half _MiddleCutoff;
			half _EffectStrength;

			//quadratic approximation of the bell curve
			half quadraticNormal(half value) 
			{
				return 4 * value * (value - 1);
			}

			//quartic approximation of the bell curve
			half quarticNormal(half value)
			{
				half quadraticValue = quadraticNormal(value);
				return quadraticValue * quadraticValue;
			}

			//maps [0,1] to [0.5, 0.5 + _MiddleValue]
			half mapToMiddleSection(half value)
			{
				return saturate((value - 0.5) / _MiddleCutoff);
			}

			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
				o.uv = v.uv;
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				half2 tex1UV = i.uv + _ScrollSpeed * _Time.gg;
				half2 tex2UV = i.uv - _ScrollSpeed * _Time.gg;
				tex1UV.y /= _StretchFactor;
				tex2UV.y /= _StretchFactor;
				half noise = (tex2D(_NoiseTex1, tex1UV).r + tex2D(_NoiseTex2, tex2UV).r) / 2;
				noise = quarticNormal(mapToMiddleSection(noise));
				// i.uv.y /= _StretchFactor;
				fixed4 mainCol = tex2D(_MainTex, i.uv);
				mainCol.a = saturate(mainCol.a * (1 + _EffectStrength * noise));
				return mainCol;
			}
			ENDCG
		}
	}
}