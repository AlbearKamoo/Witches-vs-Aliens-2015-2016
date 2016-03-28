Shader "Custom/WitchPuckEffect"
{
	Properties
	{
		_MainTex("MainTexture", 2D) = "white" {}
		_NumXPixels("Number of X pixels", Float) = 2
		_NumYPixels("Number of Y pixels", Float) = 2
		_ASCIITex("ASCIILookupTable", 2D) = "white" {}
		_NumASCIIValues("Number of characters in the ASCII texture", Float) = 1
		_TimeScale("TimeScale", Float) = 1
		
	}
	SubShader
	{
		Tags { "RenderType"="Transparent" "IgnoreProjector"="True" "Queue"="Transparent"  "PreviewType"="Plane"}
		Blend SrcAlpha One
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
			half _NumXPixels;
			half _NumYPixels;
			sampler2D _ASCIITex;
			half _NumASCIIValues;
			half _TimeScale;

			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
				o.uv = v.uv;
				return o;
			}

			fixed4 calculateASCIIValue(float2 uv, float2 textureOffset)
			{
				half2 pixelatedUV; //which pixel we are in
				half2 subPixelUV; //where are we in that pixel
				half2 pixelCenterUV; //the center of our pixel in image UV coordinates

				subPixelUV.x = modf(uv.x * _NumXPixels, pixelatedUV.x);
				pixelCenterUV.x = (pixelatedUV.x + 0.5) / _NumXPixels;
				pixelatedUV.x /= _NumXPixels;


				subPixelUV.y = modf(uv.y * _NumYPixels, pixelatedUV.y);
				pixelCenterUV.y = (pixelatedUV.y + 0.5) / _NumYPixels;
				pixelatedUV.y /= _NumYPixels;

				fixed4 pixelCol = tex2D(_MainTex, pixelCenterUV + textureOffset);
				//fixed4 actualCol = tex2D(_MainTex, uv);

				half brightness = max(pixelCol.r, max(pixelCol.g, pixelCol.b)); //there might be a better scheme that uses weighting

				fixed actualMaxComponent = brightness;

				brightness = (floor((1 - brightness) * _NumASCIIValues) + subPixelUV.x) / _NumASCIIValues; //snap to a particular character in the ASCIITex, and add in the sub-pixel UV to get the UV.x coordinate for the ASCIITEX

				half2 ASCIITexUV = half2(brightness, subPixelUV.y);

				fixed4 returnValue = tex2D(_ASCIITex, ASCIITexUV);

				//fixed actualMaxComponent = max(actualCol.r, max(actualCol.g, actualCol.b));

				//return returnValue * actualCol / actualMaxComponent;

				return returnValue * pixelCol / actualMaxComponent;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
			    float2 sampleUV = i.uv;
				sampleUV.x += _Time.x;

				fixed4 result = calculateASCIIValue(sampleUV, _TimeScale * _Time.gg) / 2;

				sampleUV = i.uv;
				sampleUV.x += frac(-_Time.x);

				result += calculateASCIIValue(sampleUV, _TimeScale * -_Time.gg) / 2;

				return result;
			}
			ENDCG
		}
	}
}