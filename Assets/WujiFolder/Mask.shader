Shader "Custom/Mask"
{
	Properties
	{
		_ShadowColor ("Shadow Color", Color) = (0.35, 0.4, 0.45, 1.0)
	}
		SubShader
		{
      Tags 
      { 
        "RenderType" = "Transparent" 
        "Queue" = "Transparent-1"
        "RenderPipeline" = "UniversalPipeline"
      }

      Pass
      {
        Blend DstColor Zero, Zero One
        Cull Back
        ZTest LEqual
        ZWrite Off

        Tags
        {
          "LightMode" = "UniversalForward"
        }

        HLSLPROGRAM
        #pragma vertex vert
        #pragma fragment frag
  
        #pragma multi_compile _ _MAIN_LIGHT_SHADOWS
        #pragma multi_compile _ _MAIN_LIGHT_SHADOWS_CASCADE
        #pragma multi_compile _ _SHADOWS_SOFT
  
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

        CBUFFER_START(UnityPerMaterial)
          float4 _ShadowColor;
        CBUFFER_END
  
        struct Attributes
        {
          float4 positionOS : POSITION;  
        };
  
        struct Varyings
        {
          float4 positionCS : SV_POSITION;
          float3 positionWS : TEXCOORD0;
        };
  
        Varyings vert(Attributes IN)
        {
          Varyings OUT;
          OUT.positionCS = TransformObjectToHClip(IN.positionOS);
          OUT.positionWS = TransformObjectToWorld(IN.positionOS);  
          return OUT;
        }
  
        half4 frag(Varyings IN) : SV_TARGET {

          half4 color = half4(1,1,1,1);

          #ifdef _MAIN_LIGHT_SHADOWS
            VertexPositionInputs vertexInput = (VertexPositionInputs)0;
            vertexInput.positionWS = IN.positionWS;

            float4 shadowCoord = GetShadowCoord(vertexInput);
            half shadowAttenutation = MainLightRealtimeShadow(shadowCoord);
            color = lerp(half4(1,1,1,1), _ShadowColor, (1.0 - shadowAttenutation) * _ShadowColor.a); //lerp(x, y, s) = x*(1-s) + y*s 
          #endif

          return color;
        }
        ENDHLSL
      }
		
	}
}