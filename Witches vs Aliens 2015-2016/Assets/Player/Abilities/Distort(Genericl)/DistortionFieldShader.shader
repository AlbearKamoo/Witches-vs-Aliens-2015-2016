Shader "Unlit/DistortionField"
{
	Properties
	{
		_MainTex("Bumpmap (RG)", 2D) = "bump" {}
		_EffectTex("Bumpmap (RG)", 2D) = "bump" {}
		_StrengthTex("White/Black", 2D) = "white" {}
		_XShift ("XShift", Range(-1,1)) = 1
		_YShift ("YShift", Range(-1,1)) = 1
		_DistortionStrength ("DistortionStrength", Range(0,1)) = 0.1
		_NoiseStrength ("NoiseStrength", Range(0,1)) = 0.1
	}
	SubShader
	{
		Tags { "Queue"="Transparent" "PreviewType"="Plane"}
		LOD 100

		GrabPass { "_GrabTexture"}

		Pass
		{
			ZWrite On
			Blend One Zero
			Lighting Off
			Fog { Mode Off }
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				half2 uv : TEXCOORD0;
			};

			struct v2f
			{
				half2 uv : TEXCOORD0;
				half2 uv_2 : TEXCOORD1;
				float4 vertex : SV_POSITION;
			};

			sampler2D _EffectTex;
			float4 _MainTex_ST;
			sampler2D _MainTex;
			sampler2D _StrengthTex;
			sampler2D _GrabTexture;
			fixed _XShift;
			fixed _YShift;
			half _DistortionStrength;
			half _NoiseStrength;

			inline half2 distortion(half2 rg)
			{
				return (2*rg - 1);
			}

			inline float2 Repeat(float2 t, float2 length)
			{
				return t - floor(t / length) * length;
			}

			inline float2 PingPong(float2 t, float2 length)
			{
				t = Repeat(t, length * 2);
				return length - abs(t - length);
			}
			
			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				//add time factor to o.uv here
				o.uv_2 = ComputeGrabScreenPos(o.vertex);

				return o;
			}
			
			fixed4 frag (v2f i) : COLOR
			{
				half4 effectColor = tex2D(_EffectTex, i.uv);
				half4 strengthColor = tex2D(_StrengthTex, i.uv);
				i.uv += fixed2(_XShift, _YShift) * _Time.gg;
				half4 effectNoiseColor = tex2D(_EffectTex, i.uv);  //ping pong gets rid off artifacts when sampling outside of textures
				half2 uv = i.uv_2 + (strengthColor.rg * (_DistortionStrength * distortion(effectColor.rg) + _NoiseStrength * distortion(effectNoiseColor.rg)));
				fixed4 col = tex2D(_GrabTexture, uv);
				UNITY_OPAQUE_ALPHA(col.a);
				return col;
			}
			ENDCG
		}
	}
}
