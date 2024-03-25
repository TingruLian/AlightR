// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

Shader "Custom/ToonShader_Normal"
{
    Properties
    {
        _Color1 ("Color1", Color) = (1,1,1,1)
        _Color2 ("Color2", Color) = (1,1,1,1)
        _Color3 ("Color3", Color) = (1,1,1,1)
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        _NormalTex ("Normal Map", 2D) = "white" {}
        _BumpScale ("Bump Scale", Range(0.0, 5.0)) = 1.0
        _Glossiness ("Gloss", Range(0.1,8)) = 3
        _Metallic ("Metallic", Range(0,1)) = 0.0
        _ColorStep1("ColorStep1", Range(0,1)) = 0.6
        _ColorStep2("ColorStep2", Range(0,1)) = 0.3
        _Feather("Feather", Range(0.0001, 1)) = 0.0
    }
    SubShader
    {

        Tags { "RenderType"="Opaque"}

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
            sampler2D _NormalTex;
            float4 _MainTex_ST;
            float4 _NormalTex_ST;
            float _BumpScale;
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
                float4 tangent : TANGENT;
                float4 texcoord : TEXCOORD0;
            };

            struct v2f
            {
                float4 uv: TEXCOORD0;
                //float3 worldNormal : TEXCOORD1;
                //float3 worldPos : TEXCOORD2;
                float4 position : SV_POSITION;
                float4 TtoW0 : TEXCOORD1;
                float4 TtoW1 : TEXCOORD2;
                float4 TtoW2 : TEXCOORD3;
            };

            v2f vert(a2v i){
                v2f o;
                o.position = UnityObjectToClipPos(i.vertex.xyz);
                
                
                o.uv.xy = TRANSFORM_TEX(i.texcoord, _MainTex);
                o.uv.zw = i.texcoord.xy * _NormalTex_ST.xy + _NormalTex_ST.zw;

                float3 worldPos = mul(unity_ObjectToWorld, i.vertex).xyz;
                fixed3 worldNormal = UnityObjectToWorldNormal(i.normal);
                fixed3 worldTangent = UnityObjectToWorldDir(i.tangent.xyz);
                fixed3 worldBinormal = cross(worldNormal, worldTangent) * i.tangent.w;

                //compute the matrix that transform directions from tangent space to world space
                //put the world position in w component for optimization
                o.TtoW0 = float4(worldTangent.x, worldBinormal.x, worldNormal.x, worldPos.x);
                o.TtoW1 = float4(worldTangent.y, worldBinormal.y, worldNormal.y, worldPos.y);
                o.TtoW2 = float4(worldTangent.z, worldBinormal.z, worldNormal.z, worldPos.z);

                return o;
            }


            fixed4 frag(v2f u) : SV_Target {

                float3 worldPos = float3(u.TtoW0.w, u.TtoW1.w, u.TtoW2.w);

                fixed3 lightDir = normalize(UnityWorldSpaceLightDir(worldPos));
                fixed3 viewDir = normalize(UnityWorldSpaceViewDir(worldPos));

                //get the normal in tangent space
                fixed3 bump = UnpackNormal(tex2D(_NormalTex, u.uv.zw));
                bump.xy *= _BumpScale;
                bump.z = sqrt(1.0 - saturate(dot(bump.xy, bump.xy)));
                //transform the normal form tangent space to world space
                bump = normalize(half3(dot(u.TtoW0.xyz, bump), dot(u.TtoW1.xyz, bump), dot(u.TtoW2.xyz, bump)));

                fixed3 albedo;
                fixed3 specular = 0;

                float f1 = _ColorStep1 + _Feather * _ColorStep2 - _Feather * _ColorStep1;
                float f2 = (1 - _Feather) * _ColorStep2;
              
                float cosine = dot(lightDir, bump) * 0.5 + 0.5;
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
                // else{f
                //     albedo = tex2D(_MainTex, u.uv).rgb * _Color3.rgb;
                // }

                fixed3 diffuse = _LightColor0.rgb * albedo;
                fixed3 halfDir = normalize(lightDir + viewDir);
                // if(pow(saturate(dot(reflectDir, viewDir)), _Glossiness) > 0.9){
                //     specular = _LightColor0.rgb * _Metallic * 0.3;
                // }

                specular = _LightColor0.rgb * _Metallic * pow(max(0, dot(bump, halfDir)), _Glossiness);

                fixed3 ambient = UNITY_LIGHTMODEL_AMBIENT.xyz * albedo;

                return fixed4(ambient + specular + diffuse, 1.0);
            }

            ENDCG
        }
    }
    FallBack "Universal Render Pipeline/Lit"
}
