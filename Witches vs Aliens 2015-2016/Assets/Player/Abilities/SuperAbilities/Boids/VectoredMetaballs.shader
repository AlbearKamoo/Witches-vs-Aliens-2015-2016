Shader "Unlit/VectoredMetaballs"
{
	Properties
	{
		_MainTex("MainTexture", 2D) = "white" {}
		_SampleTex("Sampled Texture", 2D) = "white" {}
		_Cutoff("Cutoff", Range(0,1)) = 0.1
		_ScrollSpeed("ScrollSpeed", Float) = 0.1
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
			#pragma fragment frag alpha
			
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
			sampler2D _SampleTex;
			float4 _SampleTex_ST;
			fixed _Cutoff;
			half _ScrollSpeed;

			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
				o.uv = v.uv;
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				fixed4 col = tex2D(_MainTex, i.uv);
				fixed enabled = step(_Cutoff, col.a);
				col.a = enabled * (1 - saturate((col.a - _Cutoff) / (1 - _Cutoff)) / 2);

				//half scaleFactor = _ScrollSpeed / col.b;

				//i.uv_sample.x += (col.x - col.z) * scaleFactor * _Time.gg;
				//i.uv_sample.y += (col.y - col.z) * scaleFactor * _Time.gg;

				//col.rgb = tex2D(_SampleTex, i.uv_sample).rgb; //don't need the old col anymore
				//return col;

				//Vector Debugging
				half center = 1 / (2 * col.z);
				return fixed4(col.x * center, col.y * center, col.z, col.a);
			}
			ENDCG
		}
	}
}