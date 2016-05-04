Shader "Unlit/IceShader"
{
	Properties
	{
		_MainTex("MainTexture", 2D) = "white" {}
		_NormalTex("Normal Map", 2D) = "white" {}
		_LightVector("Light Vector", Vector) = (0, 0, 1, 1)
		_Shine("Shine", Float) = 200
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
				float2 uv_normal : TEXCOORD1;
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;

			sampler2D _NormalTex;
			float4 _NormalTex_ST;

			fixed3 _LightVector;

			half _Shine;

			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				o.uv_normal = TRANSFORM_TEX(v.uv, _NormalTex);
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				fixed4 col = tex2D(_MainTex, i.uv);

				fixed3 normal;
				normal.xy = tex2D(_NormalTex, i.uv_normal).xy;
				normal.z = 1;
				normalize(normal);

				half diffuseIntensity = dot(normal, _LightVector);

				fixed3 reflection = normalize(2 * diffuseIntensity * normal + _LightVector);
				half specularIntensity = max(pow(dot(reflection, fixed3(0,0,1)), _Shine), 0);

				return fixed4(diffuseIntensity + specularIntensity, diffuseIntensity + specularIntensity, diffuseIntensity + specularIntensity, 1);
			}
			ENDCG
		}
	}
}