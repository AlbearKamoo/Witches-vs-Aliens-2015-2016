﻿Shader "Unlit/JFAVoronoi"
{
	Properties
	{
		_MainTex("MainTexture", 2D) = "white" {}
		_Width("Width in pixels", Float) = 1
		_Height("Height in pixels", Float) = 1
		_Distance("Distance in pixels, should be a power of 2", Float) = 1
		[Toggle(FIRST)] _FIRST ("First pass", Float) = 1
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
			#pragma shader_feature FIRST
			
			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				half2 centerUV : TEXCOORD0;
				half2 negativeUV : TEXCOORD1;
				half2 positiveUV : TEXCOORD2;
				float4 vertex : SV_POSITION;
			};

			sampler2D _MainTex;
			half _Width;
			half _Height;
			half _Distance;

			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
				o.centerUV = v.uv;
				half2 UVDistance = half2(_Distance / _Width, _Distance / _Height);
				o.negativeUV = o.centerUV - UVDistance;
				o.positiveUV = o.centerUV + UVDistance;
				return o;
			}

			bool validUV(half2 UV)
			{
				return UV.x >= 1 && UV.x <= 1 && UV.y >= 1 && UV.y <= 1;
			}

			#if FIRST
			half3 updatePoint(half3 previousPoint, half2 testUV)
			{
				fixed4 testCol = tex2D(_MainTex, testUV);
				testCol.a = step(0.1, testCol.a); //1 if actual seed point, zero otherwise
				if(validUV(testUV) && testCol.a > previousPoint.z) //we only need to check for valid seed values, since we call updatePoint in order from nearest to farthest distance
				{
					previousPoint.xy = testUV;
					previousPoint.z = testCol.a;
				}
				return previousPoint;
			}
			#else
			half4 updatePoint(half4 previousPoint, half2 currentUV, half2 testUV)
			{
				fixed4 testCol = tex2D(_MainTex, testUV);
				testCol.a = step(0.1, testCol.a); //1 if actual seed point, zero otherwise
				if(!validUV(testUV) || testCol.a != 1)
				{
					return previousPoint;
				}
				else
				{
					testCol.z = distance(currentUV, testCol.xy);
					if(testCol.z < previousPoint.z)
					{
						previousPoint.xyz = testCol.xyz;
						previousPoint.a = 1;
					}
				}
				return previousPoint;
			}
			#endif
			fixed4 frag (v2f i) : SV_Target
			{
				fixed4 testCol = tex2D(_MainTex, i.centerUV);
				
				#if FIRST
				half3 closestPoint;
				closestPoint.xy = i.centerUV;
				closestPoint.z = step(0.1, testCol.a); //1 if actual seed point, zero otherwise

				#else

				half4 closestPoint;
				if(step(0.1, testCol.a) == 1)
				{
					closestPoint.xy = testCol.xy;
					closestPoint.z = distance(i.centerUV, testCol.xy);
					closestPoint.a = 1;

				}
				else
				{
					closestPoint.z = 1; //z is distance; 1 is max distance, since there was no seed
					closestPoint.a = 0;
				}
				#endif

				//now check surrounding points

				#if FIRST
				closestPoint = updatePoint(closestPoint, half2(i.centerUV.x, i.negativeUV.y));
				closestPoint = updatePoint(closestPoint, half2(i.centerUV.x, i.positiveUV.y));

				closestPoint = updatePoint(closestPoint, half2(i.negativeUV.x, i.centerUV.y));
				closestPoint = updatePoint(closestPoint, half2(i.positiveUV.x, i.centerUV.y));

				closestPoint = updatePoint(closestPoint, i.negativeUV);
				closestPoint = updatePoint(closestPoint, i.positiveUV);

				closestPoint = updatePoint(closestPoint, half2(i.negativeUV.x, i.positiveUV.y));
				closestPoint = updatePoint(closestPoint, half2(i.positiveUV.x, i.negativeUV.y));

				#else

				closestPoint = updatePoint(closestPoint, i.centerUV, half2(i.centerUV.x, i.negativeUV.y));
				closestPoint = updatePoint(closestPoint, i.centerUV, half2(i.centerUV.x, i.positiveUV.y));

				closestPoint = updatePoint(closestPoint, i.centerUV, half2(i.negativeUV.x, i.centerUV.y));
				closestPoint = updatePoint(closestPoint, i.centerUV, half2(i.positiveUV.x, i.centerUV.y));

				closestPoint = updatePoint(closestPoint, i.centerUV, i.negativeUV);
				closestPoint = updatePoint(closestPoint, i.centerUV, i.positiveUV);

				closestPoint = updatePoint(closestPoint, i.centerUV, half2(i.negativeUV.x, i.positiveUV.y));
				closestPoint = updatePoint(closestPoint, i.centerUV, half2(i.positiveUV.x, i.negativeUV.y));
				#endif

				//now format the result

				#if FIRST
				//return fixed4(testCol.a, step(0.1, testCol.a), step(0.1, testCol.a), 1);
				return fixed4(closestPoint.x, closestPoint.y, closestPoint.z, closestPoint.z);
				#else
				return closestPoint;
				#endif
			}
			ENDCG
		}
	}
}