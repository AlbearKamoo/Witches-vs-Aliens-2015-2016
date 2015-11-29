Shader "Unlit/BackgroundMesh"
{
	Properties
	{
		_ImageTex("Texture", 2D) = "white" {}
	}
	SubShader
	{
		Tags { "RenderType"="Transparent" "IgnoreProjector"="True" "Queue"="Transparent+1"  "PreviewType"="Plane"}
		Blend Zero SrcColor
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
				float4 vertex : SV_POSITION;
				float2 uv : TEXCOORD0;
			};
			sampler2D _ImageTex;

			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
				o.uv = v.uv;
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				fixed4 img = tex2D(_ImageTex, i.uv);
				return img;
			}
			ENDCG
		}
	}
}