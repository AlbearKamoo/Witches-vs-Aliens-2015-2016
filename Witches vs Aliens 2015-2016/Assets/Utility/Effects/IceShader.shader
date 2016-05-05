Shader "Unlit/IceShader"
{
	Properties
	{
		_MainTex("MainTexture", 2D) = "white" {}
		_NormalTex1("Normal Map", 2D) = "white" {}
		_NormalTex2("Normal Map", 2D) = "white" {}
		_LightVector("Light Vector", Vector) = (0, 0, 1, 1)
		_Shine("Shine", Float) = 200
		_Color("Color", Color) = (1, 1, 1)
		_DistortionStrength("Distortion Strength", Float) = 0.1
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
				float2 uv_normal1 : TEXCOORD1;
				float2 uv_normal2 : TEXCOORD2;
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;

			sampler2D _NormalTex1;
			float4 _NormalTex1_ST;

			sampler2D _NormalTex2;
			float4 _NormalTex2_ST;

			fixed3 _LightVector;

			half _Shine;
			half _DistortionStrength;

			fixed4 _Color;

			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				o.uv_normal1 = TRANSFORM_TEX(v.uv, _NormalTex1) + float2(_Time.y / 40, _Time.y / 40);
				o.uv_normal2 = TRANSFORM_TEX(v.uv, _NormalTex2) - float2(_Time.y / 40, _Time.y / 40);
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				fixed3 normal;
				normal.xy = (1 + tex2D(_NormalTex1, i.uv_normal1).xy) * (1 + tex2D(_NormalTex2, i.uv_normal2).xy);
				normal.xy = (normal.xy - 2) / 2;
				normal.z = 1;

				fixed4 col = tex2D(_MainTex, i.uv + _DistortionStrength * normal.xy);

				normalize(normal);

				half diffuseIntensity = dot(normal, _LightVector) + 0.25;

				fixed3 reflection = normalize(2 * diffuseIntensity * normal - _LightVector);
				half specularIntensity = max(pow(dot(reflection, fixed3(0,0,1)), _Shine), 0);

				fixed4 lightingCol = _Color * diffuseIntensity;
				lightingCol += specularIntensity;



				return (lightingCol * (1 - col.a / 4)) + col / 4;
			}
			ENDCG
		}
	}
}