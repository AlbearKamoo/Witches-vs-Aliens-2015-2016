Shader "Unlit/Twist"
{
	Properties
	{
		_Strength ("Strength", Float) = 1
		_MaxRadius ("Max Radius", Float) = 1
	}
	SubShader
	{
		Tags { "Queue"="Transparent" "PreviewType"="Plane"}
		LOD 100

		GrabPass { "_GrabTexture"}

		Pass
		{
			ZWrite On
			Blend One Zero
			Lighting Off
			Fog { Mode Off }
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				half2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float4 vertex : SV_POSITION;
				float4 center : TEXCOORD2;
				float4 vertexWorld : TEXCOORD3;
				float4 uv : TEXCOORD0;
				float4 uv_center : TEXCOORD1;
			};

			sampler2D _GrabTexture;
			half _Strength;
			half _MaxRadius;
			
			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
				o.center = mul(_Object2World, float4(0,0,0,1));
				o.vertexWorld = mul(_Object2World, v.vertex);
				o.uv = ComputeGrabScreenPos(o.vertex);
				o.uv_center = ComputeGrabScreenPos(mul(UNITY_MATRIX_MVP, float4(0,0,0,1)));
				return o;
			}
			
			fixed4 frag (v2f i) : COLOR
			{
				float4 displacement = i.vertexWorld - i.center;
				float dist = length(displacement);
				half2 uv;
				if(dist > _MaxRadius)
				{
				uv = i.uv;
				}
				else
				{
					displacement = i.uv - i.uv_center;
					dist = 0.5 + _MaxRadius - dist;
					float sinValue = sin(_Strength * dist);
					float cosValue = cos(_Strength * dist);
					uv.x = displacement.x * cosValue - displacement.y * sinValue;
					uv.y = displacement.x * sinValue + displacement.y * cosValue;
					uv += i.uv_center;
				}

				fixed4 col = tex2D(_GrabTexture, uv);
				UNITY_OPAQUE_ALPHA(col.a);
				return col;
			}
			ENDCG
		}
	}
}
