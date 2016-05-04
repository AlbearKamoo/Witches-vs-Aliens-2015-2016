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

						half JFADistance(half2 pos1, half2 pos2)
			{
				/*
				half2 delta = abs(pos1 - pos2);
				return max(delta.x, delta.y);
				*/
				/*
				half2 delta = pos1 - pos2;
				return max(abs(delta.x), max( abs(1.73205080757 * delta.y + delta.x) / 2, abs(1.73205080757 * delta.y - delta.x) / 2));
				//1.73205080757 = sqrt(3)
				*/
				return distance(pos1, pos2);
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				fixed4 col = tex2D(_JFATex, i.uv);
				fixed4 result = tex2D(_MainTex, col.xy);
				//fixed maxComponent = max(result.r, max(result.g, result.b));

				//result.rgb /= maxComponent;

				#if DISTANCE_FIELD
				half dist = frac(JFADistance(col.rg, i.uv) * 50);

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