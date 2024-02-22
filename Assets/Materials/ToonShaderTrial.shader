// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

Shader "Custom/ToonShaderTrial"
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
    }
    SubShader
    {

        Tags { "RenderType"="Opaque" }
        LOD 200

        Pass
        {
            Tags {
                "LightMode" = "UniversalForward"
            }

            CGPROGRAM
            
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"
            #include "Lighting.cginc"

            sampler2D _MainTex;
            float4 _MainTex_ST;
            half _Glossiness;
            half _Metallic;
            fixed4 _Color1;
            fixed4 _Color2;
            fixed4 _Color3;
            float _ColorStep1;
            float _ColorStep2;
            float _Feather;

            struct a2v
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float4 texcoord : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv: TEXCOORD0;
                float3 worldNormal : TEXCOORD1;
                float3 worldPos : TEXCOORD2;
                float4 position : SV_POSITION;

            };

            v2f vert(a2v i){
                v2f o;
                o.position = UnityObjectToClipPos(i.vertex.xyz);
                o.worldNormal = UnityObjectToWorldNormal(i.normal);
                o.worldPos = mul(unity_ObjectToWorld, i.vertex).xyz;
                o.uv = TRANSFORM_TEX(i.texcoord, _MainTex);
                return o;
            }


            fixed4 frag(v2f u) : SV_Target {

                fixed3 ambient = UNITY_LIGHTMODEL_AMBIENT.xyz;
                fixed3 worldNormal = normalize(u.worldNormal);
                fixed3 worldLightDir = normalize(_WorldSpaceLightPos0.xyz);

                fixed3 albedo;
                fixed3 specular = 0;

                float f1 = _ColorStep1 + _Feather * _ColorStep2 - _Feather * _ColorStep1;
                float f2 = (1 - _Feather) * _ColorStep2;
              
                float cosine = dot(worldLightDir, worldNormal) * 0.5 + 0.5;
                if(cosine > _ColorStep1){
                    albedo = tex2D(_MainTex, u.uv).rgb * _Color1.rgb;
                }
                else if(cosine > _ColorStep2){
                    albedo = (tex2D(_MainTex, u.uv).rgb * _Color2.rgb * min(( _ColorStep1 - cosine ), (_ColorStep1 - f1)) + max(0, (cosine - f1)) * tex2D(_MainTex, u.uv).rgb * _Color1.rgb) / (_ColorStep1 - f1);
                }
                else{
                    albedo = (tex2D(_MainTex, u.uv).rgb * _Color3.rgb * min(( _ColorStep2 - cosine ), (_ColorStep2 - f2)) + max(0, (cosine - f2)) * tex2D(_MainTex, u.uv).rgb * _Color2.rgb) / (_ColorStep2 - f2);
                }

                // if(cosine > _ColorStep1){
                //     albedo = tex2D(_MainTex, u.uv).rgb * _Color1.rgb;
                // }
                // else if(cosine > _ColorStep2){
                //     albedo = tex2D(_MainTex, u.uv).rgb * _Color2.rgb;
                // }
                // else{
                //     albedo = tex2D(_MainTex, u.uv).rgb * _Color3.rgb;
                // }

                fixed3 diffuse = _LightColor0.rgb * albedo;

                fixed3 reflectDir = normalize(reflect(-worldLightDir, worldNormal));
                fixed3 viewDir = normalize(_WorldSpaceCameraPos.xyz - u.worldPos.xyz);
                // if(pow(saturate(dot(reflectDir, viewDir)), _Glossiness) > 0.9){
                //     specular = _LightColor0.rgb * _Metallic * 0.3;
                // }

                specular = _LightColor0.rgb * _Metallic * pow(saturate(dot(reflectDir, viewDir)), _Glossiness);

                return fixed4(specular + diffuse, 1.0);
            }

            ENDCG
        }
    }
    FallBack "Diffuse"
}
