Shader "Sprites/MindMergeShader"
{
	Properties
	{
		_MainTex("Alpha Mask", 2D) = "white" {}
		_NoiseTex("Noise Texture", 2D) = "white" {}
		_MainColor ("MainColor", Color) = (0,0,0,0)
		_AlphaNoiseSpeed ("Speed of Alpha Noise", Float) = 0.01
	}
	SubShader
	{
		Tags { "RenderType"="Transparent" "IgnoreProjector"="True" "Queue"="Transparent+1"  "PreviewType"="Plane"}
		Blend  SrcAlpha OneMinusSrcAlpha
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
			sampler2D _NoiseTex;
			float4 _MainTex_ST;
			float4 _MainColor;
			fixed _AlphaNoiseSpeed;

			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
				o.uv = v.uv;
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				fixed4 col = _MainColor;
				col.a = tex2D(_MainTex, i.uv).r;
				float2 noiseUV = i.uv;
				noiseUV.x += _AlphaNoiseSpeed * _Time.gg;
				fixed noiseAlpha = tex2D(_NoiseTex, noiseUV).r;
				col.a *= noiseAlpha;
				return col;
			}
			ENDCG
		}
	}
}