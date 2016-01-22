Shader "Particles/AddedAlpha"
{
	Properties
	{
		_MainTex("MainTexture", 2D) = "white" {}
		[Toggle(USE_COLOR)] _USES_COLOR ("Use vertex color", Float) = 1
	}
	SubShader
	{
		Tags { "RenderType"="Transparent" "IgnoreProjector"="True" "Queue"="Transparent"  "PreviewType"="Plane"}
		Blend SrcAlpha One
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

			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
				o.uv = v.uv;
				#if USE_COLOR
				o.color = i.color;
				#endif
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				fixed4 col = tex2D(_MainTex, i.uv);
				#if USE_COLOR
				col = col * i.color;
				#endif
				return col;
			}
			ENDCG
		}
	}
}