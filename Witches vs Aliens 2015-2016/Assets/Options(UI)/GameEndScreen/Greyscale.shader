Shader "Unlit/Greyscale"
{
	Properties
	{
		[PerRendererData] _MainTex ("Texture", 2D) = "white" {}
		_Cutoff ("Cutoff", Range(0,1)) = 0.5
		_Value ("Value/Brightness", Range(0,1)) = 0.5
	}
	SubShader
	{
		Tags { "RenderType"="Opaque" "IgnoreProjector"="True" "Queue"="Transparent"  "PreviewType"="Plane"}
		ZWrite Off
		Lighting Off

		Pass
		{
			CGPROGRAM
			#pragma vertex vert_img
			#pragma fragment frag
			
			#include "UnityCG.cginc"

			sampler2D _MainTex;
			fixed _Cutoff;
			fixed _Value;
			
			fixed4 frag (v2f_img i) : SV_Target
			{
				fixed4 c = tex2D(_MainTex, i.uv);

				fixed lum = c.r*.3 + c.g*.59 + c.b*.11;
							
				return _Value * (lerp(fixed4(lum, lum, lum, 1), c, _Cutoff));
			}
			ENDCG
		}
	}
}