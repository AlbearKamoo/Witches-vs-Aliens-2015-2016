Shader "Unlit/VectoredMetaballs"
{
	Properties
	{
		_MainTex("MainTexture", 2D) = "white" {}
		_SampleTex("Sampled Texture", 2D) = "white" {}
		_Cutoff("Cutoff", Range(0,1)) = 0.1
		_ScrollSpeed("ScrollSpeed", Float) = 0.1
		_NumXPixels("Number of X pixels", Float) = 2
		_NumYPixels("Number of Y pixels", Float) = 2
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
			fixed _Cutoff;
			half _ScrollSpeed;
			half _NumXPixels;
			half _NumYPixels;

			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
				o.uv = v.uv;

				return o;
			}

			fixed4 processVertex(v2f i, fixed XShift, fixed YShift)
			{
				half2 subPixelUV; //where are we in that pixel
				half2 pixelCenterUV; //the center of our pixel in image UV coordinates

				subPixelUV.x = modf((i.uv.x * _NumXPixels) + XShift, pixelCenterUV.x);
				pixelCenterUV.x /= _NumXPixels;


				subPixelUV.y = modf((i.uv.y * _NumYPixels) + YShift, pixelCenterUV.y);
				pixelCenterUV.y /= _NumYPixels;

				fixed4 col = tex2D(_MainTex, pixelCenterUV);


				half scaleFactor = 1 / col.b;

				half2 ColorVector;

				ColorVector.x = (col.x - col.z) * scaleFactor;
				ColorVector.y = (col.y - col.z) * scaleFactor;

				half hypotenuse = sqrt((ColorVector.x * ColorVector.x) + (ColorVector.y * ColorVector.y));

				half ColorSin = ColorVector.y / hypotenuse;
				half ColorCos = ColorVector.x / hypotenuse;

				//map from [0, 1] to [-1, 1]
				subPixelUV.x = 2 * subPixelUV.x - 1;
				subPixelUV.y = 2 * subPixelUV.y - 1;

				//rotate to face the color vector
				half2 rotatedSubPixelUV;
				rotatedSubPixelUV.x = subPixelUV.x * ColorCos + subPixelUV.y * ColorSin;
				rotatedSubPixelUV.y = -subPixelUV.x * ColorSin + subPixelUV.y * ColorCos;

				//map from [-1, 1] to [0, 1]
				rotatedSubPixelUV.x = (rotatedSubPixelUV.x + 1) / 2;
				rotatedSubPixelUV.y = (rotatedSubPixelUV.y + 1) / 2;

				//scroll
				rotatedSubPixelUV.x -= hypotenuse + frac(_ScrollSpeed * _Time.gg); // * hypotenuse / 1.41); // scale by colorVector's magnitude (1.41 is sqrt(1 + 1))

				fixed4 result = tex2D(_SampleTex, rotatedSubPixelUV);

				//scale result by distance from pixel center (there are multiple pixels; this ensures even overlap and smooth transition)

				fixed alphaScale = (1 - abs(subPixelUV.x)) * (1 - abs(subPixelUV.y));

				result *= 1-((1-alphaScale) * (1-alphaScale));

				return result;

				//Vector Debugging
				//half center = 1 / (2 * col.z);
				//return fixed4(col.x * center, col.y * center, col.z, col.a);
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				fixed4 result;
				result = processVertex(i, 0, 0);
				result += processVertex(i, 0, 0.5);
				result += processVertex(i, 0.5, 0.25);
				result += processVertex(i, 0.5, 0.75);

				fixed actualAlpha = tex2D(_MainTex, i.uv).a;
				fixed enabled = step(_Cutoff, actualAlpha);
				result.a *= enabled * saturate((actualAlpha - _Cutoff) / (1 - _Cutoff));
				//Shield-style alpha mapping //result.a *= enabled * (1 - saturate((actualAlpha - _Cutoff) / (1 - _Cutoff)) / 2);

				return result;
			}
			ENDCG
		}
	}
}