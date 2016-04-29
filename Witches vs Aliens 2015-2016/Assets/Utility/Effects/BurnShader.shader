Shader "Unlit/BurnShader"
{
	Properties
	{
		_MainTex("MainTexture", 2D) = "white" {}
		_HeightTex("Height Map", 2D) = "white" {}
		_VectorTex("Normal Map", 2D) = "white" {}
		_SmokeTex("Smoke Texture", 2D) = "white" {}
		_DistortionStrength("Distortion Strength", Float) = 0.1
		_TimeScale("Time Scale", Float) = 0.1
		_Cutoff("Cutoff", Range(0, 1)) = 0.5
		_EdgeWidth("Edge Width", Range(0, 1)) = 0.05
		_EdgeColor("Edge Color", Color) = (0.5, 0.5, 0.5, 1)
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
				float2 uv_main : TEXCOORD1;
				float2 uv_smoke : TEXCOORD2;
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;
			sampler2D _SmokeTex;
			float4 _SmokeTex_ST;

			sampler2D _HeightTex;
			sampler2D _VectorTex;
			fixed _Cutoff;
			fixed _EdgeWidth;
			fixed4 _EdgeColor;

			half _DistortionStrength;
			half _TimeScale;

			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
				o.uv = v.uv;
				o.uv_main = TRANSFORM_TEX(v.uv, _MainTex);
				o.uv_smoke = TRANSFORM_TEX(v.uv, _SmokeTex);
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				fixed4 height = tex2D(_HeightTex, i.uv);
				if(height.r > 2 * _Cutoff)
				{
					return tex2D(_MainTex, i.uv_main);
				}
				else if(height.r > 2 * _Cutoff - _EdgeWidth)
				{
					fixed4 mainColor = tex2D(_MainTex, i.uv_main);
					return lerp(_EdgeColor, mainColor, (height - (2 * _Cutoff - _EdgeWidth)) / _EdgeWidth);
				}
				else
				{
					float2 displacement = tex2D(_VectorTex, i.uv).xy;

					//map from [0, 1] to [-1, 1] space
					displacement -= 0.5;
					displacement *= 2 * _DistortionStrength;

					half timeFrac1 = frac(_TimeScale * _Time.y);
					half timeFrac2 = frac(_TimeScale * _Time.y + 0.5);

					half lerpValue = 2 * min(timeFrac1, 1 - timeFrac1);
				
					fixed4 value1 = tex2D(_SmokeTex, i.uv_smoke + timeFrac1 * displacement);
					fixed4 value2 = tex2D(_SmokeTex, i.uv_smoke + timeFrac2 * displacement);
					fixed4 result = lerp(value2, value1, lerpValue);

					result.a = 1 - saturate(3 * (2 * _Cutoff - 2 * _EdgeWidth - height.r));

					result.a *= tex2D(_MainTex, i.uv_main).a;
					return result;
				}
			}
			ENDCG
		}
	}
}