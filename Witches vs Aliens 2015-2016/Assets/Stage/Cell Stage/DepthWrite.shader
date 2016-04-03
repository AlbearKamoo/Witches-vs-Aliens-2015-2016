Shader "Unlit/DepthWrite"
{
	Properties
	{
	}
	SubShader
	{
		Tags { "RenderType"="Transparent" "IgnoreProjector"="True" "PreviewType"="Plane"}
		Blend Zero One
		ZWrite On

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
			};

			struct v2f
			{
				float4 vertex : SV_POSITION;
			};

			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				return fixed4(0,0,0,0);
			}
			ENDCG
		}
	}
	Fallback "Diffuse"
}
