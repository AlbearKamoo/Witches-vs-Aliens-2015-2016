Shader "Unlit/BasicMetaballs"
{
	Properties
	{
		_MainTex("MainTexture", 2D) = "white" {}
		_Cutoff("Cutoff", Range(0,1)) = 0.1
	}
	SubShader
	{
		Tags { "RenderType"="Transparent" "IgnoreProjector"="True" "Queue"="Transparent"  "PreviewType"="Plane"}
		Blend SrcAlpha OneMinusSrcAlpha, One Zero
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
			fixed _Cutoff;

			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
				o.uv = v.uv;
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				fixed col = tex2D(_MainTex, i.uv).b;
				fixed enabled = step(_Cutoff, col);
				col = saturate(40 * (col - _Cutoff));

				return enabled * fixed4(1, 1, 1, 1 - col);
			}
			ENDCG
		}
	}
}