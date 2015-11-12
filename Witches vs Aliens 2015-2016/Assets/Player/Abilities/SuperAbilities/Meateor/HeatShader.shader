Shader "Unlit/HeatShader"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_GradientTex("Gradient", 2D) = "white" {}
		_RelativeHeatTex ("RelativeHeatmap", 2D) = "white" {}
		_Cutoff ("Cutoff", Range(0,1)) = 1
	}
	SubShader
	{
		Tags { "RenderType"="Transparent" "IgnoreProjector"="True" "Queue"="Transparent"  "PreviewType"="Plane"}
		Blend One One
		Zwrite Off
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
				float4 vertex : POSITION;
			};

			sampler2D _MainTex;
			sampler2D _GradientTex;
			sampler2D _RelativeHeatTex;
		
			float4 _MainTex_ST;
			float _Cutoff;

			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
				o.uv = v.uv;
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				float heatValue = saturate(_Cutoff * tex2D(_RelativeHeatTex, i.uv).r);
				fixed4 heatColor = tex2D(_GradientTex, fixed2(heatValue ,0.5));
				fixed4 col = tex2D(_MainTex, i.uv);		
				return col * heatColor;
			}
			ENDCG
		}
	}
}