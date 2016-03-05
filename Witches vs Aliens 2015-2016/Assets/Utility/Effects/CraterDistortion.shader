Shader "Unlit/CraterDistortion"
{
	Properties
	{
		_MainTex("Main Texture", 2D) = "white" {}
		_EffectTex1("Bumpmap1 (RG)", 2D) = "bump" {}
		_Intensity ("Distortion Strength", Float) = 0.1
	}
	SubShader
	{
		Tags { "Queue"="Transparent" "PreviewType"="Plane"}
		LOD 100

		Pass
		{
			ZWrite On
			Blend SrcAlpha OneMinusSrcAlpha
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
				half2 uv0 : TEXCOORD0;
				half2 uv1 : TEXCOORD1;
				half2 uv2 : TEXCOORD2;
				float4 vertex : SV_POSITION;
			};

			sampler2D _MainTex;
			sampler2D _EffectTex1;
			float4 _MainTex_ST;
			float4 _EffectTex1_ST;
			half _Intensity;

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
				o.uv0 = TRANSFORM_TEX(v.uv, _MainTex);
				o.uv1 = TRANSFORM_TEX(v.uv, _EffectTex1) + _Time.xy;
				o.uv2.x = o.uv1.y;
				o.uv2.y = -o.uv1.x;
				return o;
			}
			
			fixed4 frag (v2f i) : COLOR
			{
				half4 effectColor;
				effectColor.r = tex2D(_EffectTex1, i.uv1).r;
				effectColor.g = tex2D(_EffectTex1, i.uv2).g;
				half2 uv = i.uv0 + _Intensity * distortion(effectColor.rg);
				fixed4 col = tex2D(_MainTex, uv);
				col.a *= -4 * i.uv0.y * (i.uv0.y - 1);
				return col;
			}
			ENDCG
		}
	}
}
