Shader "Unlit/AlphaMaskShader"
{
	Properties
	{
		_MainTex ("Color (RGB) Alpha (A)", 2D) = "white" {}
		_AlphaMap ("Additional Alpha Map (Greyscale)", 2D) = "white" {}
	}
	SubShader
	{
		Tags { "Queue"="Transparent" "RenderType"="Transparent" }
		LOD 100

		Pass
		{
			ZWrite Off
			Blend SrcAlpha OneMinusSrcAlpha
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			// make fog work
			#pragma multi_compile_fog
			
			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				UNITY_FOG_COORDS(1)
				float4 vertex : SV_POSITION;
			};

			sampler2D _MainTex;
			sampler2D _AlphaMap;
			float4 _MainTex_ST;
			
			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				UNITY_TRANSFER_FOG(o,o.vertex);
				return o;
			}
			
			float2 sinusoid (float2 x, float2 m, float2 M, float2 p) {
				float2 e   = M - m;
				float2 c = 3.1415 * 2.0 / p;
				return e / 2.0 * (1.0 + sin(x * c)) + m;
			}

			fixed4 frag (v2f i) : COLOR
			{
				// sample the texture
				fixed4 col = tex2D(_MainTex, i.uv);

				float time = _Time[1];

				float2 waterDisplacement =
				sinusoid
				(
					float2 (time, time),
					float2(-0.1, -0.1),
					float2(+0.1, +0.1),
				float2(10, 10)
				);

				fixed4 alpha = tex2D(_AlphaMap, i.uv + waterDisplacement);
				// apply fog
				UNITY_APPLY_FOG(i.fogCoord, col);	
				col.a = col.a * alpha.r;			
				return col;
			}

			
			ENDCG
		}
	}
}
