Shader "Unlit/MultipleUVDistortion"
{
	Properties
	{
		_MainTex("Main Texture", 2D) = "white" {}
		_EffectTex1("Bumpmap1 (RG)", 2D) = "bump" {}
		_EffectTex2("Bumpmap2 (RG)", 2D) = "bump" {}
	}
	SubShader
	{
		Tags { "Queue"="Transparent" "PreviewType"="Plane"}
		LOD 100

		Pass
		{
			ZWrite On
			Blend SrcAlpha One
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
				half2 uv  : TEXCOORD0;
				half2 uv0 : TEXCOORD1;
				half2 uv1 : TEXCOORD2;
				half2 uv2 : TEXCOORD3;
				float4 vertex : SV_POSITION;
			};

			sampler2D _MainTex;
			sampler2D _EffectTex1;
			sampler2D _EffectTex2;
			float4 _MainTex_ST;
			float4 _EffectTex1_ST;
			float4 _EffectTex2_ST;
			sampler2D _GrabTexture;
			fixed _XShift;
			fixed _YShift;
			half _Intensity;

			inline half2 distortion(half2 rg)
			{
				return (2*rg - 1);
			}
			
			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
				o.uv = v.uv;
				o.uv0 = TRANSFORM_TEX(v.uv, _MainTex);
				o.uv1 = TRANSFORM_TEX(v.uv + float2(_Time.y / 5, _Time.y / 5), _EffectTex1);
				o.uv2 = TRANSFORM_TEX(v.uv - float2(_Time.y / 5, _Time.y / 5), _EffectTex2);
				return o;
			}
			
			fixed4 frag (v2f i) : COLOR
			{
				half2 uv = 0.1 * distortion(tex2D(_EffectTex1, i.uv1).rg);
				uv = 0.1 * distortion(tex2D(_EffectTex2, i.uv2 + uv).rg);
				return tex2D(_MainTex, i.uv0 + uv);
			}
			ENDCG
		}
	}
}
