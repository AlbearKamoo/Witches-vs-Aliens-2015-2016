Shader "Unlit/JFAVoronoiFinalPass"
{
	Properties
	{
		_MainTex("Main Texture", 2D) = "white" {}
		_JFATex("final JFA texture", 2D) = "white" {}
		[Toggle(DISTANCE_FIELD)] _DISTANCE_FIELD("distance field rendering", Float) = 0
	}
	SubShader
	{
		Tags { "RenderType"="Transparent" "IgnoreProjector"="True" "Queue"="Transparent"  "PreviewType"="Plane"}
		Blend One Zero
		ZWrite Off
		Lighting Off

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma shader_feature DISTANCE_FIELD
			
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
			float4 _MainTex_ST;
			sampler2D _JFATex;

			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
				o.uv = v.uv;
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				fixed4 col = tex2D(_JFATex, i.uv);
				fixed4 result = tex2D(_MainTex, col.xy);
				//fixed maxComponent = max(result.r, max(result.g, result.b));

				//result.rgb /= maxComponent;

				#if DISTANCE_FIELD
				half dist = frac(distance(col.rg, i.uv) * 50);

				dist = dist * dist;
				dist = dist * dist;

				return fixed4(dist, dist, dist, 1);
				#else
				result.a = 1;
				return result;
				#endif
			}
			ENDCG
		}
	}
}