Shader "URP/TilingNormalMappedSurface"
{
    Properties
    {
        _MainTex ("Base Texture", 2D) = "white" {}
        _BumpMap ("Normal Map", 2D) = "bump" {}
        _BumpScale ("Normal Strength", Range(0,2)) = 1.0
        _TileScale ("Tile Scale", Range(0.1, 50)) = 1.0
        _UseWorldSpace ("Use World Space", Float) = 1.0
        [Toggle] _UseTriplanar ("Use Triplanar Mapping", Float) = 0.0
        _TriplanarBlend ("Triplanar Blend", Range(0.1, 10)) = 1.0
        _Color ("Color Tint", Color) = (1,1,1,1)
        
        // PBR Properties
        _Metallic ("Metallic", Range(0,1)) = 0.0
        _Smoothness ("Smoothness", Range(0,1)) = 0.2
        
        // URP Required Properties
        [HideInInspector] _Surface("__surface", Float) = 0.0
        [HideInInspector] _Blend("__blend", Float) = 0.0
        [HideInInspector] _AlphaClip("__clip", Float) = 0.0
        [HideInInspector] _SrcBlend("__src", Float) = 1.0
        [HideInInspector] _DstBlend("__dst", Float) = 0.0
        [HideInInspector] _ZWrite("__zw", Float) = 1.0
        [HideInInspector] _Cull("__cull", Float) = 2.0
    }
    
    SubShader
    {
        Tags 
        { 
            "RenderType" = "Opaque"
            "RenderPipeline" = "UniversalPipeline"
            "Queue" = "Geometry"
        }
        LOD 300
        
        Pass
        {
            Name "ForwardLit"
            Tags { "LightMode" = "UniversalForward" }
            
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            
            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS
            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS_CASCADE
            #pragma multi_compile _ _ADDITIONAL_LIGHTS_VERTEX _ADDITIONAL_LIGHTS
            #pragma multi_compile_fragment _ _ADDITIONAL_LIGHT_SHADOWS
            #pragma multi_compile_fragment _ _SHADOWS_SOFT
            
            #pragma shader_feature _USETRIPLANAR_ON
            
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
            
            struct Attributes
            {
                float4 positionOS : POSITION;
                float3 normalOS : NORMAL;
                float4 tangentOS : TANGENT;
                float2 uv : TEXCOORD0;
            };
            
            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float3 positionWS : TEXCOORD0;
                float3 normalWS : TEXCOORD1;
                float4 tangentWS : TEXCOORD2;
                float2 uv : TEXCOORD3;
                float3 viewDirWS : TEXCOORD4;
            };
            
            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);
            TEXTURE2D(_BumpMap);
            SAMPLER(sampler_BumpMap);
            
            CBUFFER_START(UnityPerMaterial)
                float4 _MainTex_ST;
                float _BumpScale;
                float _TileScale;
                float _UseWorldSpace;
                float _UseTriplanar;
                float _TriplanarBlend;
                float4 _Color;
                float _Metallic;
                float _Smoothness;
            CBUFFER_END
            
            float4 TriplanarSample(TEXTURE2D_PARAM(tex, texSampler), float3 worldPos, float3 worldNormal, float scale)
            {
                float2 uvX = worldPos.zy * scale;
                float2 uvY = worldPos.xz * scale;
                float2 uvZ = worldPos.xy * scale;
                
                float4 texX = SAMPLE_TEXTURE2D(tex, texSampler, uvX);
                float4 texY = SAMPLE_TEXTURE2D(tex, texSampler, uvY);
                float4 texZ = SAMPLE_TEXTURE2D(tex, texSampler, uvZ);
                
                float3 blend = pow(abs(worldNormal), _TriplanarBlend);
                blend = blend / (blend.x + blend.y + blend.z);
                
                return texX * blend.x + texY * blend.y + texZ * blend.z;
            }
            
            float3 TriplanarNormalSample(TEXTURE2D_PARAM(normalMap, normalSampler), float3 worldPos, float3 worldNormal, float scale)
            {
                float3 normalX = UnpackNormal(SAMPLE_TEXTURE2D(normalMap, normalSampler, worldPos.zy * scale));
                float3 normalY = UnpackNormal(SAMPLE_TEXTURE2D(normalMap, normalSampler, worldPos.xz * scale));
                float3 normalZ = UnpackNormal(SAMPLE_TEXTURE2D(normalMap, normalSampler, worldPos.xy * scale));
                
                normalX = float3(normalX.xy + worldNormal.zy, abs(normalX.z) * worldNormal.x);
                normalY = float3(normalY.xy + worldNormal.xz, abs(normalY.z) * worldNormal.y);
                normalZ = float3(normalZ.xy + worldNormal.xy, abs(normalZ.z) * worldNormal.z);
                
                float3 blend = pow(abs(worldNormal), _TriplanarBlend);
                blend = blend / (blend.x + blend.y + blend.z);
                
                return normalize(normalX * blend.x + normalY * blend.y + normalZ * blend.z);
            }
            
            Varyings vert(Attributes input)
            {
                Varyings output = (Varyings)0;
                
                VertexPositionInputs vertexInput = GetVertexPositionInputs(input.positionOS.xyz);
                VertexNormalInputs normalInput = GetVertexNormalInputs(input.normalOS, input.tangentOS);
                
                output.positionCS = vertexInput.positionCS;
                output.positionWS = vertexInput.positionWS;
                output.normalWS = normalInput.normalWS;
                
                real sign = input.tangentOS.w * GetOddNegativeScale();
                output.tangentWS = float4(normalInput.tangentWS.xyz, sign);
                
                output.uv = TRANSFORM_TEX(input.uv, _MainTex);
                output.viewDirWS = GetWorldSpaceViewDir(vertexInput.positionWS);
                
                return output;
            }
            
            float4 frag(Varyings input) : SV_Target
            {
                float4 baseColor;
                float3 normalTS;
                
                #ifdef _USETRIPLANAR_ON
                    baseColor = TriplanarSample(TEXTURE2D_ARGS(_MainTex, sampler_MainTex), input.positionWS, input.normalWS, _TileScale) * _Color;
                    normalTS = TriplanarNormalSample(TEXTURE2D_ARGS(_BumpMap, sampler_BumpMap), input.positionWS, input.normalWS, _TileScale);
                    normalTS.xy *= _BumpScale;
                #else
                    float2 texCoords;
                    
                    if (_UseWorldSpace > 0.5)
                    {
                        texCoords = input.positionWS.xz * _TileScale;
                    }
                    else
                    {
                        texCoords = input.uv * _TileScale;
                    }
                    
                    baseColor = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, texCoords) * _Color;
                    normalTS = UnpackNormal(SAMPLE_TEXTURE2D(_BumpMap, sampler_BumpMap, texCoords));
                    normalTS.xy *= _BumpScale;
                #endif
                
                // Transform normal from tangent space to world space
                float3 bitangentWS = input.tangentWS.w * cross(input.normalWS, input.tangentWS.xyz);
                float3x3 tangentToWorld = float3x3(input.tangentWS.xyz, bitangentWS, input.normalWS);
                float3 normalWS = normalize(mul(normalTS, tangentToWorld));
                
                // Setup lighting input
                InputData inputData = (InputData)0;
                inputData.positionWS = input.positionWS;
                inputData.normalWS = normalWS;
                inputData.viewDirectionWS = SafeNormalize(input.viewDirWS);
                inputData.shadowCoord = TransformWorldToShadowCoord(input.positionWS);
                
                // Setup surface data with controllable metallic and smoothness
                SurfaceData surfaceData = (SurfaceData)0;
                surfaceData.albedo = baseColor.rgb;
                surfaceData.alpha = baseColor.a;
                surfaceData.metallic = _Metallic;
                surfaceData.smoothness = _Smoothness;
                surfaceData.normalTS = normalTS;
                surfaceData.emission = 0;
                surfaceData.occlusion = 1.0;
                
                return UniversalFragmentPBR(inputData, surfaceData);
            }
            ENDHLSL
        }
        
        Pass
        {
            Name "ShadowCaster"
            Tags { "LightMode" = "ShadowCaster" }
            
            ZWrite On
            ZTest LEqual
            ColorMask 0
            
            HLSLPROGRAM
            #pragma vertex ShadowPassVertex
            #pragma fragment ShadowPassFragment
            
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
            
            float3 _LightDirection;
            
            struct Attributes
            {
                float4 positionOS : POSITION;
                float3 normalOS : NORMAL;
            };
            
            struct Varyings
            {
                float4 positionCS : SV_POSITION;
            };
            
            float4 GetShadowPositionHClip(Attributes input)
            {
                float3 positionWS = TransformObjectToWorld(input.positionOS.xyz);
                float3 normalWS = TransformObjectToWorldNormal(input.normalOS);
                float4 positionCS = TransformWorldToHClip(ApplyShadowBias(positionWS, normalWS, _LightDirection));
                
                #if UNITY_REVERSED_Z
                    positionCS.z = min(positionCS.z, positionCS.w * UNITY_NEAR_CLIP_VALUE);
                #else
                    positionCS.z = max(positionCS.z, positionCS.w * UNITY_NEAR_CLIP_VALUE);
                #endif
                
                return positionCS;
            }
            
            Varyings ShadowPassVertex(Attributes input)
            {
                Varyings output;
                output.positionCS = GetShadowPositionHClip(input);
                return output;
            }
            
            half4 ShadowPassFragment(Varyings input) : SV_TARGET
            {
                return 0;
            }
            ENDHLSL
        }
        
        Pass
        {
            Name "DepthOnly"
            Tags { "LightMode" = "DepthOnly" }
            
            ZWrite On
            ColorMask 0
            
            HLSLPROGRAM
            #pragma vertex DepthOnlyVertex
            #pragma fragment DepthOnlyFragment
            
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            
            struct Attributes
            {
                float4 positionOS : POSITION;
            };
            
            struct Varyings
            {
                float4 positionCS : SV_POSITION;
            };
            
            Varyings DepthOnlyVertex(Attributes input)
            {
                Varyings output;
                output.positionCS = TransformObjectToHClip(input.positionOS.xyz);
                return output;
            }
            
            half4 DepthOnlyFragment(Varyings input) : SV_TARGET
            {
                return 0;
            }
            ENDHLSL
        }
    }
    
    FallBack "Hidden/Universal Render Pipeline/FallbackError"
}