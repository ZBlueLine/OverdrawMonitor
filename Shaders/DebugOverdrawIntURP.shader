Shader "OverDrawMonitor" {
	Properties {
		[Header(Hardware settings)]
		[Enum(UnityEngine.Rendering.CullMode)] HARDWARE_CullMode ("Cull faces", Float) = 2
		[Enum(On, 1, Off, 0)] HARDWARE_ZWrite ("Depth write", Float) = 0
	}
	SubShader {
		Tags {
			"RenderPipeline"="UniversalPipeline"
			"RenderType"="Opaque"
			"Queue"="Geometry+50"
		}

		HLSLINCLUDE
		#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

		ENDHLSL

		Cull [HARDWARE_CullMode]
		ZWrite [HARDWARE_ZWrite]
		Blend One One

		Pass {
			Name "Unlit"

			HLSLPROGRAM
			#pragma vertex UnlitPassVertex
			#pragma fragment UnlitPassFragment

			// Structs
			struct Attributes {
				float4 positionOS	: POSITION;
			};

			struct Varyings {
				float4 positionCS 	: SV_POSITION;
			};

			// Vertex Shader
			Varyings UnlitPassVertex(Attributes IN) {
				Varyings OUT;

				VertexPositionInputs positionInputs = GetVertexPositionInputs(IN.positionOS.xyz);
				OUT.positionCS = positionInputs.positionCS;
				return OUT;
			}

			// Fragment Shader
			float4 UnlitPassFragment(Varyings IN) : SV_Target 
			{
				// 1 / 512 = 0.001953125; 1 / 1024 = 0.0009765625
				return 0.001953125;
			}
			ENDHLSL
		}
	}
}