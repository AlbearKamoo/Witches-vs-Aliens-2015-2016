Shader "Unlit/VectoredMetaball"
{
	Properties
	{
		_MainTex("MainTexture", 2D) = "white" {}
		_XVel("Bumpmap X-velocity", Range(0,1)) = 0.5
		_YVel("Bumpmap Y-velocity", Range(0,1)) = 0.5
		_NumBoids("Num Boids (precision)", Float) = 4
	}
	SubShader
	{
		Tags { "RenderType"="Transparent" "IgnoreProjector"="True" "Queue"="Transparent"  "PreviewType"="Plane"}
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
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
				fixed3 vectoring : COLOR;
			};

			sampler2D _MainTex;
			fixed _XVel;
			fixed _YVel;
			half _NumBoids;

			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
				o.uv = v.uv;
				o.vectoring.z = 1/_NumBoids;
				o.vectoring.x = o.vectoring.z * _XVel;
				o.vectoring.y = o.vectoring.z * _YVel;
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				fixed col = tex2D(_MainTex, i.uv).r;

				//non-interpolated
				//return fixed4(i.vectoring.x, i.vectoring.y, i.vectoring.z / 2, col * col);

				//interpolated
				return col * fixed4(i.vectoring.x, i.vectoring.y, i.vectoring.z / 2, col);
			}
			ENDCG
		}
	}
}