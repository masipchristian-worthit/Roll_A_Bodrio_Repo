Shader "/PSXRetro/FullscreenURP" {
    Properties {
        _PixelSize ("Pixel size (px)", Float) = 4
        _DitherStrength ("Dither Strength", Range(0,1)) = 1
        _FogColor ("Fog Color", Color) = (0.3,0.25,0.2,1)
        _FogDensity ("Fog Density", Float) = 1.0
        _FogStart ("Fog Start (m)", Float) = 0.0
        _FogEnd ("Fog End (m)", Float) = 50.0
        _Contrast ("Contrast", Range(0,2)) = 1
        _Brightness ("Brightness", Range(-1,1)) = 0
    }

    SubShader {
        Tags { "RenderType"="Opaque" "Queue"="Overlay" }
        Pass {
            Name "PSXRetroPass"
            ZTest Always
            ZWrite Off

            HLSLPROGRAM
            #pragma vertex VertDefault
            #pragma fragment Frag
            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS

            // URP helpers
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"


            TEXTURE2D(_CameraColorTexture);
            SAMPLER(sampler_CameraColorTexture);
            TEXTURE2D(_CameraDepthTexture);
            SAMPLER(sampler_CameraDepthTexture);

            float _PixelSize;
            float _DitherStrength;
            float4 _FogColor;
            float _FogDensity;
            float _FogStart;
            float _FogEnd;
            float _Contrast;
            float _Brightness;

            struct Attributes {
                float4 positionOS : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct Varyings {
                float4 positionCS : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            Varyings VertDefault(Attributes v) {
                Varyings o;
                o.positionCS = TransformObjectToHClip(v.positionOS.xyz);
                o.uv = v.uv;
                return o;
            }

            // 4x4 Bayer matrix (values 0..15)
            uint bayer4x4[16] = {0,8,2,10,12,4,14,6,3,11,1,9,15,7,13,5};

            float OrderedDither(float2 fragPixelPos, float strength) {
                int2 ip = (int2)fragPixelPos;
                int bx = ip.x & 3;
                int by = ip.y & 3;
                int index = bx + by * 4;
                float t = (float)bayer4x4[index] / 16.0;
                return (t - 0.5) * strength;
            }

            float2 PixelizeUV(float2 uv) {
                float2 res = _ScreenParams.xy;
                float2 px = uv * res;
                float2 q = floor(px / max(1e-6, _PixelSize));
                float2 center = (q + 0.5) * _PixelSize;
                return center / res;
            }

            inline float SampleLinear01Depth(float2 uv){
                 float rawDepth = SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, sampler_CameraDepthTexture, uv);
                 // Reconstrucción lineal genérica para URP17
                 return rawDepth / (_ZBufferParams.x * rawDepth + _ZBufferParams.y);
            }


            float FogFactorFromDepth(float linear01Depth) {
                if (_FogEnd > _FogStart + 1e-5) {
                    float d = saturate((linear01Depth - _FogStart/_FogEnd) / ((_FogEnd - _FogStart)/_FogEnd));
                    return saturate(d * _FogDensity);
                }
                return 1 - exp(-linear01Depth * _FogDensity);
            }

            float3 TonemapCB(float3 col) {
                col = col * _Contrast + _Brightness;
                return saturate(col);
            }

            float4 Frag(Varyings i) : SV_Target {
                float2 pxUV = PixelizeUV(i.uv);
                float4 color = SAMPLE_TEXTURE2D(_CameraColorTexture, sampler_CameraColorTexture, pxUV);

                float2 fragCoord = i.positionCS.xy / i.positionCS.w * 0.5 + 0.5;
                fragCoord *= _ScreenParams.xy;

                float dither = OrderedDither(fragCoord, _DitherStrength);
                float lum = dot(color.rgb, float3(0.2126, 0.7152, 0.0722));
                lum = saturate(lum + dither);
                float3 finalRGB = color.rgb;
                if (lum > 1e-6) finalRGB *= lum / max(1e-6, dot(color.rgb, float3(0.2126,0.7152,0.0722)));

                finalRGB = TonemapCB(finalRGB);

                float linear01Depth = SampleLinear01Depth(i.uv);
                float fogFactor = FogFactorFromDepth(linear01Depth);
                finalRGB = lerp(finalRGB, _FogColor.rgb, fogFactor);

                return float4(finalRGB, color.a);
            }

            ENDHLSL
        }
    }
    Fallback Off
}
