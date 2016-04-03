Shader "Custom/VoronoiCone"
{
	Properties
	{
		_MainTex("Gradient", 2D) = "white" {}
		_Color("Color", Color) = (1, 0, 1, 1)
		[Toggle(EDGE_HIGHLIGHTING)] _EDGE_HIGHLIGHTING ("Edge Highlighting", Float) = 1
	}
	SubShader
	{
		Tags { "RenderType"="Transparent" "IgnoreProjector"="True" "Queue"="Transparent"  "PreviewType"="Plane"}
		Blend SrcAlpha OneMinusSrcAlpha
		Lighting Off

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma shader_feature EDGE_HIGHLIGHTING
			
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
				float4 projPos : TEXCOORD2; //Screen position of pos
			};

			sampler2D _MainTex;
			#if EDGE_HIGHLIGHTING
			sampler2D _CameraDepthTexture; //Depth Texture
			#endif
			fixed3 _Color;

			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
				o.projPos = ComputeScreenPos(o.vertex);
				o.uv = v.uv;
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				fixed4 col = tex2D(_MainTex, i.uv);
				fixed4 result;

				#if EDGE_HIGHLIGHTING

				//Get the distance to the camera from the depth buffer for this point
                float sceneZ = LinearEyeDepth (tex2Dproj(_CameraDepthTexture,
                                                         UNITY_PROJ_COORD(i.projPos)).r);
 
                //Actual distance to the camera
                float partZ = i.projPos.z;

				#endif

				result.a = col.a;

				#if EDGE_HIGHLIGHTING

				if(abs(sceneZ - partZ) < 1) //if we're really close
				{
					result.rgb = fixed3(1, 1, 1);
				}
				else
				#endif
				{
					result.rgb = col.r * lerp(fixed3(1, 1, 1), _Color, 1-col.g);
				}

				result.rgb = col.r * lerp(fixed3(1, 1, 1), _Color, 1-col.g);

				

				return result;
			}
			ENDCG
		}
	}
}