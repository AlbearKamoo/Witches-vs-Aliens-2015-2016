Shader "Unlit/VectoredDrift"
{
	Properties
	{
		_MainTex("MainTexture", 2D) = "white" {}
		_PrevTex("Previous Texture", 2D) = "white" {}
		_NoiseTex("Noise Texture", 2D) = "white" {}
		_Cutoff("Cutoff", Range(0,1)) = 0.1
		_ScrollSpeed("ScrollSpeed", Float) = 0.1
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
				float2 uv_noise : TEXCOORD1;
				float4 vertex : SV_POSITION;
			};

			sampler2D _MainTex;
			sampler2D _PrevTex;
			sampler2D _NoiseTex;
			float4 _NoiseTex_ST;
			fixed _Cutoff;
			half _ScrollSpeed;
			half _NumXPixels;
			half _NumYPixels;

			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
				o.uv = v.uv;
				o.uv_noise = TRANSFORM_TEX(v.uv, _NoiseTex);
				o.uv_noise.x +=  + _Time.y * _ScrollSpeed;
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				fixed actualAlpha = tex2D(_MainTex, i.uv).a;
				fixed enabled = step(_Cutoff, actualAlpha);

				if(enabled == 0)
				{
					return fixed4(0, 0, 0, 0);
				}

				fixed4 col = tex2D(_MainTex, i.uv);

				half scaleFactor = 1 / col.b;

				half2 ColorVector;

				ColorVector.x = (col.x - col.z) * scaleFactor;
				ColorVector.y = (col.y - col.z) * scaleFactor;

				half4 result = tex2D(_NoiseTex, i.uv_noise);
				half4 previous = tex2D(_PrevTex, i.uv - ColorVector / 100);

				previous *= 2.05;
				previous += 0.01;
				result *= previous;

				half normalizedAlpha = saturate((actualAlpha - _Cutoff) / (1 - _Cutoff));
				result *= normalizedAlpha;

				//result.xy *= ColorVector + 1; //random-ish coloring
				
				//outline
				normalizedAlpha = 1 - normalizedAlpha;
				normalizedAlpha *= normalizedAlpha;
				result.rb += normalizedAlpha;

				return result;
			}
			ENDCG
		}
	}
}