Shader "Sprites/BlinkShader"
{
//v = max(0,sign(distance - _Value))
	Properties
	{
		[PerRendererData] _MainTex ("Texture", 2D) = "white" {}
		_CullingTex ("Greyscale", 2D) = "black" {}
		_DistortionTex ("Bumpmap", 2D) = "bump" {}
		_Cutoff ("Cutoff", Range(0,1)) = 0.5
		_Distortion ("Distortion", Range(0,1)) = 0
	}
	SubShader
	{
		Tags { "RenderType"="Opaque" "IgnoreProjector"="True" "Queue"="Transparent"  "PreviewType"="Plane"}

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
			sampler2D _CullingTex;
			sampler2D _DistortionTex;
			float4 _MainTex_ST;
			fixed  _Cutoff;
			fixed _Distortion;
			
			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
				o.uv = v.uv;
				return o;
			}

			half2 distortion(half2 rg)
			{
				return (2*rg - 1);
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				fixed4 cull = tex2D(_CullingTex, i.uv);

				clip(_Cutoff - cull.b); //cull if we don't meet cutoff

				half4 bump = tex2D(_DistortionTex, i.uv);
				half2 dist = distortion(bump.rg);
				i.uv += dist * _Distortion * (1-_Cutoff)/(_Cutoff - cull.b);
				fixed4 col = tex2D(_MainTex, i.uv);
				
				
				// apply fog				
				return col;
			}
			ENDCG
		}
	}
}
