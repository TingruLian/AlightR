Shader "Unlit/SemanticShader2"
{
    Properties
    {
        _MainTex ("_MainTex", 2D) = "white" {}
        _SemanticTex("_SemanticTex", 2D) = "red" {}
        _Color("_Color", Color) = (1,1,1,1)
        _Color2("_Color2", Color) = (1,1,1,1) 
        _BlendRate("_BlendRate", Range(0.0, 1.0)) = 0.1
    }
    SubShader
    {
        Tags {"Queue" = "Transparent" "IgnoreProjector" = "True" "RenderType" = "Transparent"}
        BlendOp RevSub, Add
        Blend OneMinusDstColor OneMinusSrcColor, SrcAlpha Zero
        // No culling or depth
        Cull Off ZWrite Off ZTest Always
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

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

            float4x4 _SemanticMat;

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;

                //we need to adjust our image to the correct rotation and aspect.
                o.texcoord = mul(_SemanticMat, float4(v.uv, 1.0f, 1.0f)).xyz;

                return o;
            }

            sampler2D _MainTex;
            sampler2D _SemanticTex;
            fixed4 _Color;
            fixed4 _Color2;
            float _BlendRate;

            fixed4 frag(v2f i) : SV_Target
            {
                //convert coordinate space
                float2 semanticUV = float2(i.texcoord.x / i.texcoord.z, i.texcoord.y / i.texcoord.z);
                float2 semanticUVleft = float2(i.texcoord.x + _BlendRate / i.texcoord.z, i.texcoord.y / i.texcoord.z);
                float2 semanticUVright = float2(i.texcoord.x - _BlendRate / i.texcoord.z, i.texcoord.y / i.texcoord.z);
                float2 semanticUVtop = float2(i.texcoord.x / i.texcoord.z, i.texcoord.y + _BlendRate / i.texcoord.z);
                float2 semanticUVbottom = float2(i.texcoord.x / i.texcoord.z, i.texcoord.y - _BlendRate / i.texcoord.z);

                float4 semanticCol = tex2D(_SemanticTex, semanticUV);
                float4 semanticColleft = tex2D(_SemanticTex, semanticUVleft);
                float4 semanticColright = tex2D(_SemanticTex, semanticUVright);
                float4 semanticColtop = tex2D(_SemanticTex, semanticUVtop);
                float4 semanticColbottom = tex2D(_SemanticTex, semanticUVbottom);

                float4 finalColor = (semanticColleft + semanticCol + semanticColright + semanticColtop + semanticColbottom) / 5.0;
   
                fixed scaler = finalColor.r;
                return fixed4(_Color.r * scaler + _Color2.r * (1.0 - scaler)
                           , _Color.g * scaler + _Color2.g * (1.0 - scaler)
                           ,_Color.b * scaler + _Color2.b * (1.0 - scaler)
                           ,_Color.a * scaler + _Color2.a * (1.0 - scaler));
            }
            ENDCG
        }
    }
}
