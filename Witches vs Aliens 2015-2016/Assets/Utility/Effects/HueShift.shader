Shader "Unlit/HueShift"
{
	Properties
	{
		_MainTex("MainTexture", 2D) = "white" {}
		_Shift("Hue Shift", Float) = 0.5
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
			float4 _MainTex_ST;
			half _Shift;

			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				fixed4 col = tex2D(_MainTex, i.uv);
				/*
				half max; //also the value
				half middle;
				half min;

				half offset;
				half color1;
				half color2;

				if(col.r > col.g)
				{
					if(col.r > col.b)
					{
						max = col.r;

						offset = 0;
						color1 = col.g;
						color2 = col.b;

						if(col.g > col.b)
						{
							middle = col.g;
							min = col.b;
						}
						else
						{
							middle = col.b;
							min = col.g;
						}
					}
					else
					{
						max = col.b;

						offset = 4;
						color1 = col.r;
						color2 = col.g;

						middle = col.r;
						min = col.g;
					}
				}
				else
				{
					if(col.g > col.b)
					{
						max = col.g;

						offset = 2;
						color1 = col.b;
						color1 = col.r;

						if(col.r > col.b)
						{
							middle = col.r;
							min = col.b;
						}
						else
						{
							middle = col.b;
							min = col.r;
						}
					}
					else
					{
						max = col.b;
						
						offset = 4;
						color1 = col.r;
						color2 = col.g;

						middle = col.g;
						min = col.r;
					}
				}

				float hue;
				//float value; //max is the value
				float saturation;

				if(max == 0)
				{
					hue = saturation = 0;
				}
				else
				{
					half delta = max - min;
					if(delta == 0)
					{
						hue = saturation = 0;
					}
					else
					{
						saturation = delta / max;
						hue = offset + ((color1 - color2) / delta);
						//hue = frac(hue / 6);
					}
				}
				*/

				float Epsilon = 1e-10;

				// Based on work by Sam Hocevar and Emil Persson
				float4 P = (col.g < col.b) ? float4(col.bg, -1.0, 2.0/3.0) : float4(col.gb, 0.0, -1.0/3.0);
				float4 Q = (col.r < P.x) ? float4(P.xyw, col.r) : float4(col.r, P.yzx);
				float C = Q.x - min(Q.w, Q.y);
				float hue = abs((Q.w - Q.y) / (6 * C + Epsilon) + Q.z);
				float value = Q.x;
				float saturation = C;// / (value + Epsilon);

				//hue shift
				hue = frac(hue + _Shift);

				col.r = abs(6 * hue - 3) - 1;
				col.g = 2 - abs(6 * hue - 2);
				col.b = 2 - abs(6 * hue - 4);
				saturate(col.rgb);

				col.rgb = lerp(fixed3(1, 1, 1), col.rgb, saturation);
				col.rgb *= value;

				return col;
			}
			ENDCG
		}
	}
}