﻿Shader "MG/Effect"
{
	Properties
	{
		_RenderTex("Render Tex",2D)="white"{}
	}
	SubShader
	{
		Tags{"RenderType"="Transparent" "Queue"="Transparent"}
		pass
		{
			Blend SrcAlpha OneMinusSrcAlpha
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#include "unitycg.cginc"
			sampler2D _RenderTex;

			struct v2f
			{
				float4 pos:POSITION;
				float2 uv:TEXCOORD0;
			};

			v2f vert(appdata_base v)
			{
				v2f o;
				o.pos=UnityObjectToClipPos(v.vertex);
				o.uv=v.texcoord.xy;
				return o;
			}
			float4 frag(v2f i):COLOR
			{
				float4 texColor=tex2D(_RenderTex,i.uv);
				texColor.a=step(-(texColor.r+texColor.g+texColor.b),-1);
				return texColor;
			}
			ENDCG
		}
	}
	FallBack "Diffuse"
}