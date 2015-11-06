Shader "Unlit/FogVisibility"
{
	Properties
	{
		_MainTex("Main Render Texture", 2D) = "white" {}
		_WitchPos1("WitchPos1", Vector) = (10,10,0,0)
		_WitchPos2("WitchPos2", Vector) = (10,10,0,0)
		_WitchPos3("WitchPos3", Vector) = (10,10,0,0)
		_PuckPos("PuckPos", Vector) = (0,0,0,0)
		_RangeMin("Visibility Range Min", Float) = 1
		_RangeMax("Visibility Range Max", Float) = 2
	}
	SubShader
	{
		Tags { "RenderType"="Transparent" "IgnoreProjector"="True" "Queue"="Transparent+1"  "PreviewType"="Plane"}
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
				float4 vertex_world : TEXCOORD1;
			};

			half2 _WitchPos1;
			half2 _WitchPos2;
			half2 _WitchPos3;
			half2 _PuckPos;
			sampler2D _MainTex;
			float4 _MainTex_ST;
			half _RangeMin;
			half _RangeMax;

			float2 toScreen(float2 notScreen) //converts from[-1,1] to [0,1] for a float2
			{
				return (notScreen + 1) / 2;
			}

			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
				o.uv = v.uv;
				o.vertex_world = mul(_Object2World, v.vertex);
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				fixed4 col = tex2D(_MainTex, i.uv);
				half dist = min(min(distance(i.vertex_world, _WitchPos1), distance(i.vertex_world, _WitchPos2)), min(distance(i.vertex_world, _WitchPos3), distance(i.vertex_world, _PuckPos)));
				col.a = (col.r + 0.6666) * saturate((dist - _RangeMin)/(_RangeMax - _RangeMin));
					
				return col;
			}
			ENDCG
		}
	}
}