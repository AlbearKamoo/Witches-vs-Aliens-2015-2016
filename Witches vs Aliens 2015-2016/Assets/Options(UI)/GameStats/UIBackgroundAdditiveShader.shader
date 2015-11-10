Shader "Sprites/UIBackgroundAdditiveShader"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		[Toggle(USES_COLOR)] _USES_COLOR ("Uses the Color to tint", Float) = 0
		_ImageTex("Texture", 2D) = "white" {}
		_NoiseTex ("Noise Bumpmap", 2D) = "bump" {}
		_Overlay ("Overlay", 2D) = "white" {}
		_ScrollSpeed ("ScrollSpeed", Range(-1,1)) = 1
		_NoiseStrength ("NoiseStrength", Range(-1,1)) = 1
		_ImageStrength ("Strength", Range(0,1)) = 1
		_MainTexAlpha ("MainTexAlpha", Range(0,1)) = 1

		// required for UI.Mask
         _StencilComp ("Stencil Comparison", Float) = 8
         _Stencil ("Stencil ID", Float) = 0
         _StencilOp ("Stencil Operation", Float) = 0
         _StencilWriteMask ("Stencil Write Mask", Float) = 255
         _StencilReadMask ("Stencil Read Mask", Float) = 255
         _ColorMask ("Color Mask", Float) = 15
	}
	SubShader
	{
		Tags { "RenderType"="Transparent" "IgnoreProjector"="True" "Queue"="Transparent"  "PreviewType"="Plane"}
		Blend One One
		Zwrite Off
		Lighting Off

		// required for UI.Mask
         Stencil
         {
             Ref [_Stencil]
             Comp [_StencilComp]
             Pass [_StencilOp] 
             ReadMask [_StencilReadMask]
             WriteMask [_StencilWriteMask]
         }
          ColorMask [_ColorMask]

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma shader_feature USES_COLOR
			
			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
				fixed4 color : COLOR;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float2 uv2 : TEXCOORD1;
				float4 vertex : SV_POSITION;
				#ifdef USES_COLOR
				fixed4 color : COLOR;
				#endif
			};

			sampler2D _MainTex;
			sampler2D _ImageTex;
			sampler2D _NoiseTex;
			sampler2D _Overlay;
			float _ScrollSpeed;
		
			float4 _MainTex_ST;
			float _NoiseStrength;
			float _ImageStrength;
			float _MainTexAlpha;

			#ifdef USES_COLOR
			fixed4 _Color;
			#endif
			
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
				o.uv = v.uv;
				o.uv2 = o.uv + fixed2(_ScrollSpeed/10, _ScrollSpeed) * _Time.gg;
				#ifdef USES_COLOR
				o.color = v.color;
				#endif
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				fixed4 noise = tex2D(_NoiseTex, i.uv2);
				i.uv = PingPong(i.uv + _NoiseStrength * distortion(noise.rg), 1);
				fixed4 col = tex2D(_MainTex, i.uv);		
				fixed4 img = tex2D(_ImageTex, i.uv);

				img *= _ImageStrength * col.a;
				col *= col.a;
				img = _MainTexAlpha * col + img;
				img -= tex2D(_Overlay, i.uv + fixed2(0, _ScrollSpeed/3) * _Time.gg)/30;
				#ifdef USES_COLOR
				img *= i.color;
				#endif
				return img;
			}
			ENDCG
		}
	}
}