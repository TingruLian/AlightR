Shader "Custom/SemanticMask"
{
    Properties
    {
        _MainTex ("_MainTex", 2D) = "white" {}
        _SemanticTex("_SemanticTex", 2D) = "red" {}
    }
	SubShader
    {
        Tags {"Queue" = "Transparent-1"}	

        HLSLINCLUDE
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
        ENDHLSL

        Pass
        {
            Tags {"IgnoreProjector"="True" "RenderType"="Transparent"}
            Blend SrcAlpha OneMinusSrcAlpha

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float3 texcoord : TEXCOORD1;
                float4 vertex : SV_POSITION;

            };

            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);

            TEXTURE2D(_SemanticTex);
            SAMPLER(sampler_SemanticTex);

            CBUFFER_START(UnityPerMaterial)
                float4 _MainTex_ST;
                float4 _SemanticTex_ST;
            CBUFFER_END

            float4x4 _SemanticMat;

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = TransformObjectToHClip(v.vertex.xyz);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);

                            //we need to adjust our image to the correct rotation and aspect.
                o.texcoord = mul(_SemanticMat, float4(v.uv, 1.0f, 1.0f)).xyz;

                return o;
            }

            float4 _Color;
            float4 _Color2;

            float4 frag(v2f i) : SV_Target
            {
                //convert coordinate space
                float2 semanticUV = float2(i.texcoord.x / i.texcoord.z, i.texcoord.y / i.texcoord.z);
                float4 semanticCol = SAMPLE_TEXTURE2D(_SemanticTex, sampler_SemanticTex, semanticUV);
                float4 mainCol = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.uv);
                
                return float4(mainCol.r, mainCol.g, mainCol.b, semanticCol.r * mainCol.a);
                //return float4(1.0, 1.0, 1.0, 0.5);

            }
            ENDHLSL
        }
  }
}