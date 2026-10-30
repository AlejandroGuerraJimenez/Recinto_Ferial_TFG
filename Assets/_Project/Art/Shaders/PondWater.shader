Shader "Fairground/PondWater"
{
    Properties
    {
        _ShallowColor ("Shallow", Color) = (0.45, 0.84, 0.78, 0.48)
        _DeepColor ("Deep", Color) = (0.02, 0.2, 0.36, 0.9)
        _FoamColor ("Foam", Color) = (0.94, 0.98, 0.97, 1)
        _WaveScale ("Wave Scale", Float) = 5.5
        _WaveSpeed ("Wave Speed", Float) = 0.85
        _WaveHeight ("Wave Height", Range(0, 0.08)) = 0.018
        _GlintStrength ("Glint", Range(0, 2)) = 1.05
        _DiscRadius ("Disc Radius", Range(0.2, 0.5)) = 0.49
    }

    SubShader
    {
        Tags
        {
            "RenderType" = "Transparent"
            "Queue" = "Transparent"
            "RenderPipeline" = "UniversalPipeline"
            "IgnoreProjector" = "True"
        }

        Pass
        {
            Name "PondWater"
            Tags { "LightMode" = "UniversalForward" }

            Blend SrcAlpha OneMinusSrcAlpha
            ZWrite Off
            Cull Off

            HLSLPROGRAM
            #pragma target 3.0
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_fog

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

            CBUFFER_START(UnityPerMaterial)
                half4 _ShallowColor;
                half4 _DeepColor;
                half4 _FoamColor;
                float _WaveScale;
                float _WaveSpeed;
                float _WaveHeight;
                float _GlintStrength;
                float _DiscRadius;
            CBUFFER_END

            struct Attributes
            {
                float4 positionOS : POSITION;
                float3 normalOS : NORMAL;
                float2 uv : TEXCOORD0;
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float3 positionWS : TEXCOORD0;
                float3 normalWS : TEXCOORD1;
                float2 uv : TEXCOORD2;
                float fogFactor : TEXCOORD3;
            };

            float WaveHeight(float2 uv)
            {
                float t = _Time.y * _WaveSpeed;
                float a = sin((uv.x * _WaveScale + t) * 6.2831853);
                float b = sin((uv.y * _WaveScale * 0.85 - t * 0.8) * 6.2831853);
                float c = sin(((uv.x + uv.y) * _WaveScale * 1.7 + t * 1.3) * 3.14159265);
                return a * b * 0.65 + c * 0.35;
            }

            Varyings vert(Attributes input)
            {
                Varyings output;
                float2 centered = input.uv - 0.5;
                float radial = saturate(1.0 - length(centered) * 2.2);
                input.positionOS.y += WaveHeight(input.uv) * _WaveHeight * radial;

                VertexPositionInputs pos = GetVertexPositionInputs(input.positionOS.xyz);
                output.positionCS = pos.positionCS;
                output.positionWS = pos.positionWS;
                output.normalWS = TransformObjectToWorldNormal(input.normalOS);
                output.uv = input.uv;
                output.fogFactor = ComputeFogFactor(pos.positionCS.z);
                return output;
            }

            half4 frag(Varyings input) : SV_Target
            {
                float2 centered = input.uv - 0.5;
                float dist = length(centered);
                clip(_DiscRadius - dist);

                float t = _Time.y * _WaveSpeed;
                float h = WaveHeight(input.uv);
                float hx = WaveHeight(input.uv + float2(0.012, 0));
                float hy = WaveHeight(input.uv + float2(0, 0.012));
                float3 normalWS = normalize(input.normalWS + float3(h - hx, 0, h - hy) * 7.0);

                Light mainLight = GetMainLight();
                float3 lightDir = mainLight.direction;
                if (dot(lightDir, lightDir) < 0.001)
                    lightDir = float3(0, 1, 0);

                float3 viewDir = GetWorldSpaceNormalizeViewDir(input.positionWS);
                float ndotl = saturate(dot(normalWS, lightDir));
                float3 halfDir = normalize(lightDir + viewDir);
                float spec = pow(saturate(dot(normalWS, halfDir)), 42.0) * (0.35 + 0.65 * ndotl);
                float fresnel = pow(1.0 - saturate(dot(normalWS, viewDir)), 3.0);

                float shore = smoothstep(0.02, 0.4, dist);
                half3 water = lerp(_DeepColor.rgb, _ShallowColor.rgb, shore);
                water *= 0.62 + 0.38 * ndotl;
                water += mainLight.color.rgb * spec * _GlintStrength;
                water += fresnel * half3(0.7, 0.86, 0.92) * 0.5;

                float glint = pow(
                    saturate(sin(input.uv.x * 48.0 + t * 3.1) * sin(input.uv.y * 37.0 - t * 2.5)),
                    14.0);
                water += glint * _GlintStrength * 0.4;

                float edge = smoothstep(_DiscRadius - 0.055, _DiscRadius - 0.01, dist);
                float ripple = 0.6 + 0.4 * sin(dist * 80.0 - t * 4.0 + atan2(centered.y, centered.x) * 5.0);
                water = lerp(water, _FoamColor.rgb, edge * ripple);

                half alpha = lerp(_DeepColor.a, _ShallowColor.a, shore);
                alpha = saturate(alpha + fresnel * 0.25 + edge * 0.5);
                return half4(MixFog(water, input.fogFactor), alpha);
            }
            ENDHLSL
        }
    }

    FallBack Off
}
