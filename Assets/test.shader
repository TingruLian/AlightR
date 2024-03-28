// This shader fills the mesh shape with a color predefined in the code.
Shader"Custom/test"
{
   Properties
       {
           _Color1 ("Color1", Color) = (1,1,1,1)
           _Color2 ("Color2", Color) = (1,1,1,1)
           _Color3 ("Color3", Color) = (1,1,1,1)
           _MainTex ("Albedo (RGB)", 2D) = "white" {}
           _IceTex ("Ice Tex", 2D) = "white" {}
           _Glossiness ("Gloss", Range(0.1,8)) = 3
           _Metallic ("Metallic", Range(0,1)) = 0.0
           _ColorStep1("ColorStep1", Range(0,1)) = 0.6
           _ColorStep2("ColorStep2", Range(0,1)) = 0.3
           _Feather("Feather", Range(0.0001, 1)) = 0.0
           _IceScale("Ice Scale", Range(0.0, 1.0)) = 0
           _FresnelColor("Fresnel Color",Color) = (0.6, 0.9, 0.9, 1.0)
           _FresnelWidth("Fresnel Width",Float) = 1.5
       }

       SubShader
       {
           // Universal Pipeline tag is required. If Universal render pipeline is not set in the graphics settings
           // this Subshader will fail. One can add a subshader below or fallback to Standard built-in to make this
           // material work with both Universal Render Pipeline and Builtin Unity Pipeline
           Tags
           {
               "RenderType" = "Opaque"
               "RenderPipeline" = "UniversalPipeline"
               "UniversalMaterialType" = "Lit"
               "IgnoreProjector" = "True"
               "LightMode" = "UniversalForward"
           }

           // ------------------------------------------------------------------
           //  Forward pass. Shades all light in a single pass. GI + emission + Fog
           Pass
           {
               // Lightmode matches the ShaderPassName set in UniversalRenderPipeline.cs. SRPDefaultUnlit and passes with
               // no LightMode tag are also rendered by Universal Render Pipeline

               // -------------------------------------
               // Render State Commands
               //Blend[_SrcBlend][_DstBlend], [_SrcBlendAlpha][_DstBlendAlpha]
               //ZWrite[_ZWrite]
               //Cull[_Cull]
               //AlphaToMask[_AlphaToMask]

               HLSLPROGRAM
               //#pragma target 2.0

               //// -------------------------------------
               //// Shader Stages
               #pragma vertex vert
               #pragma fragment frag

               //// -------------------------------------
               //// Material Keywords
               //#pragma shader_feature_local _NORMALMAP
               //#pragma shader_feature_local _PARALLAXMAP
               //#pragma shader_feature_local _RECEIVE_SHADOWS_OFF
               //#pragma shader_feature_local _ _DETAIL_MULX2 _DETAIL_SCALED
               //#pragma shader_feature_local_fragment _SURFACE_TYPE_TRANSPARENT
               //#pragma shader_feature_local_fragment _ALPHATEST_ON
               //#pragma shader_feature_local_fragment _ _ALPHAPREMULTIPLY_ON _ALPHAMODULATE_ON
               //#pragma shader_feature_local_fragment _EMISSION
               //#pragma shader_feature_local_fragment _METALLICSPECGLOSSMAP
               //#pragma shader_feature_local_fragment _SMOOTHNESS_TEXTURE_ALBEDO_CHANNEL_A
               //#pragma shader_feature_local_fragment _OCCLUSIONMAP
               //#pragma shader_feature_local_fragment _SPECULARHIGHLIGHTS_OFF
               //#pragma shader_feature_local_fragment _ENVIRONMENTREFLECTIONS_OFF
               //#pragma shader_feature_local_fragment _SPECULAR_SETUP

               //// -------------------------------------
               //// Universal Pipeline keywords
               //#pragma multi_compile _ _MAIN_LIGHT_SHADOWS _MAIN_LIGHT_SHADOWS_CASCADE _MAIN_LIGHT_SHADOWS_SCREEN
               //#pragma multi_compile _ _ADDITIONAL_LIGHTS_VERTEX _ADDITIONAL_LIGHTS
               //#pragma multi_compile _ EVALUATE_SH_MIXED EVALUATE_SH_VERTEX
               //#pragma multi_compile_fragment _ _ADDITIONAL_LIGHT_SHADOWS
               //#pragma multi_compile_fragment _ _REFLECTION_PROBE_BLENDING
               //#pragma multi_compile_fragment _ _REFLECTION_PROBE_BOX_PROJECTION
               //#pragma multi_compile_fragment _ _SHADOWS_SOFT _SHADOWS_SOFT_LOW _SHADOWS_SOFT_MEDIUM _SHADOWS_SOFT_HIGH
               //#pragma multi_compile_fragment _ _SCREEN_SPACE_OCCLUSION
               //#pragma multi_compile_fragment _ _DBUFFER_MRT1 _DBUFFER_MRT2 _DBUFFER_MRT3
               //#pragma multi_compile_fragment _ _LIGHT_COOKIES
               //#pragma multi_compile _ _LIGHT_LAYERS
               //#pragma multi_compile _ _FORWARD_PLUS
               //#include_with_pragmas "Packages/com.unity.render-pipelines.core/ShaderLibrary/FoveatedRenderingKeywords.hlsl"
               //#include_with_pragmas "Packages/com.unity.render-pipelines.universal/ShaderLibrary/RenderingLayers.hlsl"


               //// -------------------------------------
               //// Unity defined keywords
               //#pragma multi_compile _ LIGHTMAP_SHADOW_MIXING
               //#pragma multi_compile _ SHADOWS_SHADOWMASK
               //#pragma multi_compile _ DIRLIGHTMAP_COMBINED
               //#pragma multi_compile _ LIGHTMAP_ON
               //#pragma multi_compile _ DYNAMICLIGHTMAP_ON
               //#pragma multi_compile_fragment _ LOD_FADE_CROSSFADE
               //#pragma multi_compile_fog
               //#pragma multi_compile_fragment _ DEBUG_DISPLAY

               ////--------------------------------------
               //// GPU Instancing
               //#pragma multi_compile_instancing
               //#pragma instancing_options renderinglayer
               //#include_with_pragmas "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DOTS.hlsl"

               //#include "Packages/com.unity.render-pipelines.universal/Shaders/LitInput.hlsl"
               //#include "Packages/com.unity.render-pipelines.universal/Shaders/LitForwardPass.hlsl"
               
               #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
struct Attributes
{
                // The positionOS variable contains the vertex positions in object
                // space.
   float4 positionOS : POSITION;
};

struct Varyings
{
                // The positions in this struct must have the SV_POSITION semantic.
   float4 positionHCS : SV_POSITION;
};

            // The vertex shader definition with properties defined in the Varyings 
            // structure. The type of the vert function must match the type (struct)
            // that it returns.
Varyings vert(Attributes IN)
{
                // Declaring the output object (OUT) with the Varyings struct.
   Varyings OUT;
                // The TransformObjectToHClip function transforms vertex positions
                // from object space to homogenous space
   OUT.positionHCS = TransformObjectToHClip(IN.positionOS.xyz);
                // Returning the output.
   return OUT;
}

            // The fragment shader definition.            
half4 frag() : SV_Target
{
                // Defining the color variable and returning it.
   half4 customColor;
   customColor = half4(0.5, 0, 0, 1);
   return customColor;
}
               ENDHLSL
           }
      }

}