Shader "Custom/arrowMoveShader"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        _Cutoff ("Alpha Cutoff", Range (0, 0.9)) = 0.5
        _ShadowColor ("Shadow Color", Color) = (0.2, 0.1, 0.15, 1.0)
    }
    SubShader
    {
        Tags { 
            "RenderType"="TransparentCutout"
            "RenderPipeline" = "UniversalPipeline"
            "Queue" = "AlphaTest"
            "IgnoreProjector" = "True"
            "LightMode" = "UniversalForward"
        }
        LOD 200

        HLSLINCLUDE
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Input.hlsl"            

            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);

            CBUFFER_START(UnityPerMaterial)
                float4 _MainTex_ST;
                half4 _Color;
                float _Cutoff;
                half4 _ShadowColor;
            CBUFFER_END

        ENDHLSL

        Pass {
            Blend SrcAlpha OneMinusSrcAlpha

            HLSLPROGRAM 

            #pragma vertex vert
            #pragma fragment frag

            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            struct IVnput
            {
                float2 uv_MainTex;
            };

            Varyings vert(Attributes IN)
            {                
                Varyings OUT;                
                OUT.positionHCS = TransformObjectToHClip(IN.positionOS.xyz);       
                OUT.uv = TRANSFORM_TEX(IN.uv, _MainTex);  
                return OUT;
            }

            half4 frag(Varyings IN) : SV_Target
            {
                float2 uvTimed = float2(IN.uv.x, IN.uv.y - _Time.y / 2.0);
                half4 albedo = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uvTimed);

                //alpha test
                clip(albedo.a - _Cutoff);
                half3 diffuse = _Color.rgb * albedo.rgb * _MainLightColor.rgb;

                return half4(diffuse, albedo.a);
            }
        
            ENDHLSL
        }

        Pass
        {
            Name "ShadowCast"

            Tags {"LightMode" = "ShadowCaster"}
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            struct Attributes {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct Varyings {
                float4 position : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            Varyings vert(Attributes IN) {
                Varyings OUT;
                OUT.position = TransformObjectToHClip(IN.vertex);
                OUT.uv = TRANSFORM_TEX(IN.uv, _MainTex);  
                return OUT;
            }

            half4 frag(Varyings IN) : SV_TARGET {
                float2 uvTimed = float2(IN.uv.x, IN.uv.y - _Time.y / 2.0);
                half4 albedo = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uvTimed);
                
                //alpha test
                clip(albedo.a - _Cutoff);
                return half4(_ShadowColor.rgb, 1.0);
            }
            ENDHLSL
        }
    }
    FallBack "Diffuse"
}
