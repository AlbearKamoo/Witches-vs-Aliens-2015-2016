Shader "Unlit/UIImageShader"
{
	Properties
	{
		_ImageTex("Texture", 2D) = "white" {}
	}
	SubShader
	{
		Tags { "RenderType"="Transparent" "IgnoreProjector"="True" "Queue"="Transparent+1"  "PreviewType"="Plane"}
		Blend One One
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
			};

			struct v2f
			{
				float4 vertex : SV_POSITION;
				float2 uv_screen : TEXCOORD1;
			};
			sampler2D _ImageTex;
			
			float2 toScreen(float2 notScreen) //converts from[-1,1] to [0,1] for a float2
			{
				return (notScreen + 1) / 2;
			}

			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
				o.uv_screen = toScreen(o.vertex.xy / o.vertex.w);
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				fixed4 img = tex2D(_ImageTex, i.uv_screen);
					
				return img;
			}
			ENDCG
		}
	}
}