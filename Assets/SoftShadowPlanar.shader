// 支持Plannar shadow的shader
// - no lighting
// - no per-material color
// - no lightmap support
// - lightprobe support

// tow pass with more draw-call
Shader "SoftShadowPlanar" {
	Properties {

		//如果是RGBA，则A通道代表metallic
		//如果是RGB, 则无Alpha通道
		_MainTex ("Base (RGB) Gross (A)", 2D) = "white" {}
		//贴图RGB的强度
		_MainTexInt ("Texture Intensity", float) = 0.5
		//_LightDir.xyz是灯光方向,  Range尽量都在[-1,1], 
		//y正号代表往前偏移，y负号往后偏移， y还有偏移强度的效果（越小偏移越大）
		//_LightDir.w是地面高度
		_LightDir("Light Directioin", Vector) = (1.69,1.42,1.23,0)
		//影子颜色
		_ShadowColor("Shadow Color", Color) = (0,0,0,1.0)
		//影子Alpha衰减
		//如果是_FadeByHeight,推荐值是 1/model_height（模型高度）
        _ShadowFalloff("Shadow Falloff", float) = 0.5
	}

	SubShader {
		Tags { "Queue"="Geometry" "RenderType"="Opaque" "LightMode"="ForwardBase" }
		LOD 100

		Pass {
			Blend off

			CGPROGRAM
				#pragma vertex vert
				#pragma fragment frag
				#pragma target 2.0
				#pragma multi_compile GRAY_OFF GRAY_ON
				#pragma multi_compile METALLIC_OFF METALLIC_ON

				//----不需要雾的支持----
				//#pragma multi_compile_fog

				#include "UnityCG.cginc"

				struct appdata_t {
					float4 vertex : POSITION;
					float2 texcoord : TEXCOORD0;
					float3 normal : NORMAL;
				
					UNITY_VERTEX_INPUT_INSTANCE_ID
				};

				struct v2f {
					float4 vertex : SV_POSITION;
					float2 texcoord : TEXCOORD0;
					
				#if METALLIC_ON
					fixed3 reflectDir : TEXCOORD1;
					fixed3 viewDir : TEXCOORD2;
				#endif
					fixed3 SHLighting : COLOR;

					//UNITY_FOG_COORDS(1)
					UNITY_VERTEX_OUTPUT_STEREO
				};

				sampler2D _MainTex;
				float4 _MainTex_ST;
				float _MainTexInt;

				float4 _LightDir;

				v2f vert (appdata_t v)
				{
					v2f o;
					UNITY_SETUP_INSTANCE_ID(v);
					UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
					o.vertex = UnityObjectToClipPos(v.vertex);
					o.texcoord = TRANSFORM_TEX(v.texcoord, _MainTex);

				#if GRAY_OFF
					//获得世界坐标中的normal
					half3 worldNormal = mul((float3x3)unity_ObjectToWorld, v.normal);
					//dynamic object light probe color
					o.SHLighting = ShadeSH9(float4(worldNormal,1));

					#if METALLIC_ON
					o.reflectDir=normalize(reflect(-normalize(_LightDir),normalize(worldNormal)));
					o.viewDir=normalize(_WorldSpaceCameraPos.xyz-mul(unity_ObjectToWorld,v.vertex).xyz);
					#endif
				#endif
					return o;
				}

				fixed4 frag (v2f i) : SV_Target
				{
					fixed4 col = tex2D(_MainTex, i.texcoord);
				#if GRAY_OFF
					//暂时没有环境漫反射
					//fixed3 ambient = UNITY_LIGHTMODEL_AMBIENT.xyz;

					#if METALLIC_ON

					//颜色=本体颜色*烘培probe颜色(亮部和暗部） + 基础颜色
					fixed3 diffuse = col.rgb  * i.SHLighting + col.rgb * _MainTexInt;


					col = fixed4(/*ambient + */ diffuse, 1.0);

					#else
					col.rgb = /*ambient + */ col.rgb  * i.SHLighting + col.rgb * _MainTexInt;
					#endif

				#endif

					#if GRAY_ON
					col.rgb = dot(col.rgb, fixed3(.222,.707,.071));
					#endif
					
					return col;
				}

				

			ENDCG
		}

		//阴影pass
		Pass
		{
			Name "Shadow"
			//用使用模板测试以保证alpha显示正确
			Stencil
			{
				Ref 0
				Comp equal
				Pass incrWrap
				Fail keep
				ZFail keep
			}

			//透明混合模式
			Blend SrcAlpha OneMinusSrcAlpha
			
			//关闭深度写入
			ZWrite off

			//深度稍微偏移防止阴影与地面穿插
			Offset -1 , 0

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			#include "UnityCG.cginc"

			#pragma multi_compile __ _UpperGround //地面一下部分怎么计算
			///*阴影远离圆心时变虚*/  /*阴影根据距离变虚*/
			#pragma multi_compile _FadeSurround _FadeByHeight 

			struct appdata
			{
				float4 vertex : POSITION;
			};

			struct v2f
			{
				float4 vertex : SV_POSITION;
				float4 color : COLOR;
			};

			float4 _LightDir;
			float4 _ShadowColor;
			float _ShadowFalloff;

			float3 ShadowProjectPos(float4 vertPos, float3 worldPos)
			{
				float3 shadowPos = float3(1.0,1.0,1.0);

				//灯光方向
				fixed3 lightDir = normalize(_LightDir.xyz);

				//阴影的世界空间坐标
				#if _UpperGround
				//低于地面的部分不做改变
				shadowPos.y = min(_LightDir.w, shadowPos.y);
				#else
				//低于地面的部分依然投影到w地面 
                shadowPos.y = _LightDir.w;
				#endif
				shadowPos.xz = worldPos.xz - lightDir.xz * max(0 , worldPos.y - _LightDir.w) / lightDir.y; 

				return shadowPos;
			}

			v2f vert (appdata v)
			{
				v2f o;
				//得到顶点的世界空间坐标
				float3 worldPos = mul(unity_ObjectToWorld , v.vertex).xyz;

				//得到阴影的世界空间坐标
				float3 shadowPos = ShadowProjectPos(v.vertex, worldPos);

				//转换到裁切空间
				o.vertex = UnityWorldToClipPos(shadowPos);

				//计算阴影衰减
				fixed falloff = 1;
				#if _FadeSurround
				//得到中心点世界坐标
				//unity_ObjectToWorld是对象坐标系到世界坐标系的变换矩阵
				//此矩阵模式为: | M3*3 T3*1 |
				//            | 01*3   1  |
				// M3*3 是3行3列 ，是选择和缩放，T3*1是平移           		
				float3 center = float3(unity_ObjectToWorld[0].w , _LightDir.w , unity_ObjectToWorld[2].w);
				falloff = 1 - saturate(distance(shadowPos , center) * _ShadowFalloff);
				#endif

				#if _FadeByHeight
				falloff = 1 - saturate(worldPos.y * _ShadowFalloff);
				#endif

				//阴影颜色
				o.color = _ShadowColor; 
				o.color.a *= falloff;

				return o;
			}

			fixed4 frag (v2f i) : SV_Target
			{
				return i.color;
			}
			ENDCG
		}
	}

}
