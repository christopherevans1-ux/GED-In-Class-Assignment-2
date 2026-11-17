Shader "Custom/CRTEffect"
{
    Properties
    {
        _ScanlineIntensity ("Scanline Intensity", Range(0, 1)) = 0.3
        _ScanlineSpeed ("Scanline Speed", Range(0, 2)) = 0.3
        _CurvatureAmount ("Screen Curvature", Range(0, 0.3)) = 0.12
        _VignetteAmount ("Vignette", Range(0, 4)) = 1.8
        _VignetteRoundness ("Vignette Roundness", Range(1, 20)) = 8.0
        _ChromaticAberration ("Chromatic Aberration", Range(0, 0.006)) = 0.002
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

            float _ScanlineIntensity;
            float _ScanlineSpeed;
            float _CurvatureAmount;
            float _VignetteAmount;
            float _VignetteRoundness;
            float _ChromaticAberration;

            float2 CurveUV(float2 uv)
            {
                uv = uv * 2.0 - 1.0;
                float2 offset = abs(uv.yx) * _CurvatureAmount;
                uv = uv + uv * offset * offset;
                return uv * 0.5 + 0.5;
            }

            half4 frag(Varyings input) : SV_Target
            {
                float2 uv = CurveUV(input.texcoord);

                // Black outside curved screen edges
                if (uv.x < 0 || uv.x > 1 || uv.y < 0 || uv.y > 1)
                    return half4(0, 0, 0, 1);

                // Chromatic aberration
                float2 ca = float2(_ChromaticAberration, 0);
                half r = SAMPLE_TEXTURE2D(_BlitTexture, sampler_LinearClamp, uv + ca).r;
                half g = SAMPLE_TEXTURE2D(_BlitTexture, sampler_LinearClamp, uv).g;
                half b = SAMPLE_TEXTURE2D(_BlitTexture, sampler_LinearClamp, uv - ca).b;
                half4 col = half4(r, g, b, 1);

                // Moving scanlines
                float scanlineFreq = max(2.0, _ScreenParams.y / 270.0);
                float scanline = fmod(input.positionCS.y + (_Time.y * _ScanlineSpeed * 100.0), scanlineFreq);
                col.rgb *= scanline > (scanlineFreq - 1.0) ? 1.0 : (1.0 - _ScanlineIntensity);

                // Rounded corner vignette
                float2 vigUV = uv * (1.0 - uv);
                float vignette = pow(vigUV.x * vigUV.y * _VignetteRoundness, _VignetteAmount);
                vignette = saturate(vignette);
                col.rgb *= vignette;

                return col;
            }
            ENDHLSL
        }
    }
}