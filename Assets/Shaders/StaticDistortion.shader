Shader "Custom/StaticDistortion"
{
    Properties
    {
        _Intensity ("Intensity", Range(0, 1)) = 0
    }

    SubShader
    {
        Tags { "RenderType"="Opaque" "RenderPipeline"="UniversalPipeline" }
        ZWrite Off ZTest Always Cull Off

        Pass
        {
            HLSLPROGRAM
            #pragma vertex Vert
            #pragma fragment frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.core/Runtime/Utilities/Blit.hlsl"

            float _Intensity;

            float rand(float2 co)
            {
                return frac(sin(dot(co, float2(12.9898, 78.233))) * 43758.5453);
            }

            float rand2(float2 co)
            {
                return frac(sin(dot(co, float2(93.9898, 31.233))) * 19758.5453);
            }

            half4 frag(Varyings input) : SV_Target
            {
                float2 uv = input.texcoord;
                float t = _Intensity;

                if (t <= 0)
                    return SAMPLE_TEXTURE2D(_BlitTexture, sampler_LinearClamp, uv);

                // Horizontal scanline shift — rows of pixels jump left/right
                float lineNoise = rand(float2(floor(uv.y * 200.0), floor(_Time.y * 30.0)));
                float lineShift = (lineNoise - 0.5) * 0.08 * t;

                // Bigger block glitches at high intensity
                float blockNoise = rand(float2(floor(uv.y * 20.0), floor(_Time.y * 8.0)));
                float blockShift = 0;
                if (blockNoise > (1.0 - t * 0.6))
                    blockShift = (rand2(float2(floor(uv.y * 20.0), _Time.y)) - 0.5) * 0.15 * t;

                float2 distortedUV = uv + float2(lineShift + blockShift, 0);

                // RGB channel split — increases with intensity
                float chromaSplit = 0.008 * t * t;
                half r = SAMPLE_TEXTURE2D(_BlitTexture, sampler_LinearClamp, distortedUV + float2(chromaSplit, 0)).r;
                half g = SAMPLE_TEXTURE2D(_BlitTexture, sampler_LinearClamp, distortedUV).g;
                half b = SAMPLE_TEXTURE2D(_BlitTexture, sampler_LinearClamp, distortedUV - float2(chromaSplit, 0)).b;

                half4 col = half4(r, g, b, 1);

                // Noise grain overlay
                float grain = rand(uv + frac(_Time.y * 0.1)) * 0.15 * t;
                col.rgb += grain;

                // Vertical colour banding at high intensity
                if (t > 0.5)
                {
                    float bandT = (t - 0.5) * 2.0;
                    float band = rand(float2(floor(uv.y * 80.0), floor(_Time.y * 15.0)));
                    if (band > (1.0 - bandT * 0.3))
                    {
                        col.r = rand(float2(uv.y, _Time.y * 0.1)) * bandT;
                        col.g = rand(float2(uv.y, _Time.y * 0.2)) * bandT;
                        col.b = rand(float2(uv.y, _Time.y * 0.3)) * bandT;
                    }
                }

                return col;
            }
            ENDHLSL
        }
    }
}