Shader "Unlit/HueBlended"
{
	Properties
	{
		_MainTex("MainTexture", 2D) = "white" {}
		[Toggle(USE_COLOR)] _USES_COLOR ("Use vertex color", Float) = 1
		_Color("Color", Color) = (1,1,1,1)
		_Cutoff("Cutoff", Range(0, 1)) = 0.5

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
		Blend SrcAlpha OneMinusSrcAlpha
		ZWrite Off
		Lighting Off

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma shader_feature USE_COLOR
			
			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
				#if USE_COLOR
				float4 color : COLOR;
				#endif
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
				#if USE_COLOR
				float4 color : COLOR;
				#endif
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;
			half _Cutoff;
			#if (USE_COLOR == false)
			fixed4 _Color;
			#endif

			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				#if USE_COLOR
				o.color = v.color;
				#endif
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				fixed4 col = tex2D(_MainTex, i.uv);
				#if USE_COLOR
				col.rgb = lerp(col.rgb, i.color.rgb, _Cutoff);
				#else
				col.rgb = lerp(col.rgb, _Color.rgb, _Cutoff);
				#endif
				return col;
			}
			ENDCG
		}
	}
}