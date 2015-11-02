Shader "Sprites/GlowPulseShader"
{
//v = max(0,sign(distance - _Value))
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_CullingMask ("TextureMask (black = cull)", 2D) = "white" {}
		_NoiseTex("TextureNoise", 2D) = "white" {}
		_PulsesPerSecond("PulsesPerSecond", Range(0,10)) = 1
		_ColorCyclesPerSecond("_ColorCyclesPerSecond", Range(0,2)) = 0.25
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
			sampler2D _CullingMask;
			sampler2D _NoiseTex;
			float4 _MainTex_ST;
			half _PulsesPerSecond;
			fixed _ColorCyclesPerSecond;

			float2 uvToVector(float2 uv)
			{
				return uv - 0.5;
			}

			float2 vectorToUV(float2 vect)
			{
				return vect + float2(0.5, 0.5);
			}

			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
				o.uv = v.uv;
				return o;
			}
			fixed exponentialTween(half input)
			{
				return 1/(1 * input + 1);
			}
			fixed4 timeToColor()
			{
				fixed red = saturate(2 * abs(((_ColorCyclesPerSecond * _Time.y + 0) % 1) - 0.5));
				fixed blue = saturate(2 * abs(((_ColorCyclesPerSecond * _Time.y - 0.33333) % 1) - 0.5));
				fixed green = saturate(2 * abs(((_ColorCyclesPerSecond * _Time.y + 0.33333) % 1) - 0.5));
				return fixed4(red, blue, green, 1);
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				fixed4 culling = tex2D(_CullingMask, i.uv);
				clip(culling.r - 0.01);
				half4 col = tex2D(_MainTex, i.uv);	
				fixed value = fmod(col.r + _PulsesPerSecond * _Time.y, 1);
				fixed expvalue = exponentialTween(value);
				float2 vect = uvToVector(i.uv);
				fixed4 img = tex2D(_NoiseTex, vectorToUV(vect - (normalize(vect) * (1 + 2 * fmod(_Time.y, 1)))));
				fixed4 resultColor = (expvalue + col.r) * timeToColor();
				resultColor.a = saturate(col.r - value/2 - img.r/2);
				return resultColor;
			}
			ENDCG
		}
	}
}