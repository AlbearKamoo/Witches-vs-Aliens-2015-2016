Shader "Custom/PixelationShader"
{
	Properties
	{
		_MainTex("MainTexture", 2D) = "white" {}
		_NumXPixels("Number of X pixels", Float) = 2
		_NumYPixels("Number of Y pixels", Float) = 2
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
			};

			sampler2D _MainTex;
			half _NumXPixels;
			half _NumYPixels;

			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
				o.uv = v.uv;
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				half2 pixelCenterUV; //the center of our pixel in image UV coordinates

				pixelCenterUV.x = (floor(i.uv.x * _NumXPixels) + 0.5) / _NumXPixels;

				pixelCenterUV.y = (floor(i.uv.y * _NumYPixels) + 0.5) / _NumYPixels;

				fixed4 col = tex2D(_MainTex, pixelCenterUV);
				return col;
			}
			ENDCG
		}
	}
}