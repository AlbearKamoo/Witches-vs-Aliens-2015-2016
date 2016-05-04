Shader "Unlit/Shatter"
{
	Properties
	{
		_MainTex("Main Texture", 2D) = "bump" {}
		_StrengthMap("Strength Map", 2D) = "white" {}
		_Strength ("Strength", Float) = 1
		
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
				float2 uv : TEXCOORD0;
				float2 uv_world : TEXCOORD1;
			};

			sampler2D _MainTex;
			sampler2D _StrengthMap;
			sampler2D _GrabTexture;
			half _Strength;
			
			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
				o.uv = v.uv;
				o.uv_world = ComputeGrabScreenPos(o.vertex);
				return o;
			}
			
			fixed4 frag (v2f i) : COLOR
			{
				float2 displacement = tex2D(_MainTex, i.uv);
				displacement = 2 * (displacement - 0.5);
				//displacement.x *= 
				displacement *= tex2D(_StrengthMap, i.uv).r;
				half2 uv = i.uv_world + _Strength * displacement;

				fixed4 col = tex2D(_GrabTexture, uv);
				UNITY_OPAQUE_ALPHA(col.a);
				return col;
			}
			ENDCG
		}
	}
}
