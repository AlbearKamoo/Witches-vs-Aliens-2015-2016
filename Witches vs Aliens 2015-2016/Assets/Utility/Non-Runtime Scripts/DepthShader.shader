Shader "Unlit/DepthShader"
{
	Properties
	{
	}
	SubShader
	{
		Tags {"RenderType"="Opaque"  }
 
		ZWrite On
		Cull Off

        Pass
        {
            CGPROGRAM
            #pragma target 3.0
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
			};

			struct v2f
			{
				float4 vertex : SV_POSITION;
				float4 projPos : TEXCOORD1; //Screen position of pos
			};

			sampler2D _CameraDepthTexture; //Depth Texture

			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
				o.projPos = ComputeScreenPos(o.vertex);
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				//Get the distance to the camera from the depth buffer for this point
                float sceneZ = LinearEyeDepth (tex2Dproj(_CameraDepthTexture,
                                                         UNITY_PROJ_COORD(i.projPos)).r) / 50;

				return fixed4(sceneZ, sceneZ, sceneZ, 1);
			}
			ENDCG
		}
	}
}