Shader "Custom/ToonShaderHLSL"
{    
    Properties
    { 
        _Color1 ("Color1", Color) = (1,1,1,1)
        _Color2 ("Color2", Color) = (1,1,1,1)
        _Color3 ("Color3", Color) = (1,1,1,1)
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        _Glossiness ("Gloss", Range(0.1,8)) = 3
        _Metallic ("Metallic", Range(0,1)) = 0.0
        _ColorStep1("ColorStep1", Range(0,1)) = 0.6
        _ColorStep2("ColorStep2", Range(0,1)) = 0.3
        _Feather("Feather", Range(0.0001, 1)) = 0.0
        _RedScale("Red Scale", Range(0.0, 1.0)) = 0.0
    }

    SubShader
    {
        HLSLINCLUDE
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
        ENDHLSL
        
        Tags
        {
               "RenderType" = "Opaque"
               "RenderPipeline" = "UniversalPipeline"
               "UniversalMaterialType" = "Lit"
               "IgnoreProjector" = "True"
        }

        LOD 300

        Pass
        {
            Tags {"LightMode" = "UniversalForward"}

            HLSLPROGRAM            
            #pragma vertex vert            
            #pragma fragment frag

            struct Attributes
            {
                float4 positionOS : POSITION;
                half3 normal : NORMAL;
                float2 uv : TEXCOORD0;
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                half3 normal : TEXCOORD0;
                float3 worldPos : TEXCOORD2;
                float2 uv : TEXCOORD1;
            };

            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);

            CBUFFER_START(UnityPerMaterial)
                float4 _MainTex_ST;
                half _Glossiness;
                half _Metallic;
                half4 _Color1;
                half4 _Color2;
                half4 _Color3;
                float _ColorStep1;
                float _ColorStep2;
                float _Feather;
                float _RedScale;
            CBUFFER_END

            Varyings vert(Attributes IN)
            {                
                Varyings OUT;                
                OUT.positionHCS = TransformObjectToHClip(IN.positionOS.xyz);       
                OUT.normal = TransformObjectToWorldNormal(IN.normal);
                OUT.uv = TRANSFORM_TEX(IN.uv, _MainTex);  
                OUT.worldPos = TransformObjectToWorld(IN.positionOS).xyz;
                return OUT;
            }

            half4 frag(Varyings IN) : SV_Target
            {
                float3 lightDir = normalize(_MainLightPosition.xyz);
                float3 viewDir = normalize(GetWorldSpaceViewDir(IN.worldPos));
                float3 worldNormal = normalize(IN.normal);
                half3 albedo = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, IN.uv);
                half3 specular = 0;

                float f1 = _ColorStep1 + _Feather * _ColorStep2 - _Feather * _ColorStep1;
                float f2 = (1 - _Feather) * _ColorStep2;

                float cosine = dot(lightDir, worldNormal) * 0.5 + 0.5;
                if(cosine > _ColorStep1){
                    albedo *= _Color1.rgb;
                }
                else if(cosine > _ColorStep2){
                    albedo *= (_Color2.rgb * min(( _ColorStep1 - cosine ), (_ColorStep1 - f1)) + max(0, (cosine - f1)) * _Color1.rgb) / (_ColorStep1 - f1);
                }
                else{
                    albedo *= (_Color3.rgb * min(( _ColorStep2 - cosine ), (_ColorStep2 - f2)) + max(0, (cosine - f2)) * _Color2.rgb) / (_ColorStep2 - f2);
                }

                half3 albedo1 = _MainLightColor.rgb * albedo;
                half3 albedo2 = half3(0.3, 0, 0);
                half3 diffuse = _RedScale * (albedo2) + (1.0 - _RedScale) * albedo1;
                half3 halfDir = normalize(lightDir + viewDir);

                specular = _MainLightColor.rgb * _Metallic * pow(max(0, dot(worldNormal, halfDir)), _Glossiness);

                half3 ambient = UNITY_LIGHTMODEL_AMBIENT.xyz * albedo;

                return half4(ambient + diffuse + specular, 1.0);
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
            };

            struct Varyings {
                float4 position : SV_POSITION;
            };

            Varyings vert(Attributes IN) {
                Varyings OUT;
                OUT.position = TransformObjectToHClip(IN.vertex);
                return OUT;
            }

            half4 frag(Varyings IN) : SV_TARGET {
                return half4(0.0, 0.0, 0.0, 1.0);
            }
            ENDHLSL
        }
    }
}