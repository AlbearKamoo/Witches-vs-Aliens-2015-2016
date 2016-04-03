//Highlights intersections with other objects
 
Shader "Custom/IntersectionHighlight"
{
    Properties
    {
        _RegularColor("Main Color", Color) = (1, 1, 1, .5) //Color when not intersecting
        _HighlightColor("Highlight Color", Color) = (1, 1, 1, .5) //Color when intersecting
        _HighlightThresholdMax("Highlight Threshold Max", Float) = 1 //Max difference for intersections
		_Radius("Radius", Float) = 1
    }
    SubShader
    {
        Tags { "Queue" = "Transparent" "RenderType"="Transparent"  }
 
        Pass
        {
            Blend SrcAlpha OneMinusSrcAlpha
            ZWrite Off
            Cull Off
 
            CGPROGRAM
            #pragma target 3.0
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
 
            uniform sampler2D _CameraDepthTexture; //Depth Texture
            uniform float4 _RegularColor;
            uniform float4 _HighlightColor;
            uniform float _HighlightThresholdMax;
			uniform float _Radius;
 
            struct v2f
            {
                float4 pos : SV_POSITION;
				float3 cameraVec : TEXCOORD0;
				float3 worldPos : TEXCOORD1;
                float4 projPos : TEXCOORD2; //Screen position of pos
            };
 
            v2f vert(appdata_base v)
            {
                v2f o;
                o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
				o.worldPos = mul (_Object2World, v.vertex);
                o.projPos = ComputeScreenPos(o.pos);
				o.cameraVec = normalize(WorldSpaceViewDir(v.vertex));
 
                return o;
            }
 
            half4 frag(v2f i) : COLOR
            {
                float4 finalColor = _RegularColor;
 
                //Get the distance to the camera from the depth buffer for this point
                float sceneZ = LinearEyeDepth (tex2Dproj(_CameraDepthTexture,
                                                         UNITY_PROJ_COORD(i.projPos)).r);
 
                //Actual distance to the camera
                float partZ = i.projPos.z;


				float diff = abs(partZ - sceneZ) / 10;
				/*

                //If the two are similar, then there is an object intersecting with our object
                float diff = distance(half3(0, 0, 0), i.worldPos + ((sceneZ - partZ) * i.cameraVec)); //distance in world space from center of sphere
				diff = abs(diff - _Radius); //distance in world space from surface of sphere
				diff /= _HighlightThresholdMax; //now scaled to our max distance
 
				*/

                if(diff <= 1)
                {
                    finalColor = lerp(_HighlightColor,
                                      _RegularColor,
                                      float4(diff, diff, diff, diff));
                }
 
                half4 c;
                c.r = finalColor.r;
                c.g = finalColor.g;
                c.b = finalColor.b;
                c.a = finalColor.a;
					
                return c;
            }
 
            ENDCG
        }
    }
    FallBack "VertexLit"
}