Shader "Custom/ShaderTest"
{
    Properties
    {
        [MainTexture] _BaseMap ("Noise Texture", 2D) = "white" {}
        [MainColor] _ColorA ("Color A (đậm)", Color) = (0.35, 0.05, 0.55, 1)
        [HDR] _ColorB ("Color B (sáng)", Color) = (0.75, 0.4, 0.95, 1)
        _ColorCycleSpeed("Color Cycle Speed", Range(0, 2)) = 0.3
        _GlowIntensity("Glow Intensity", Range(0, 3)) = 1.2
        [Float] _ScrollSpeed("Scroll Speed", Range(0, 10)) = 1
        [Float] _FadeRange("Fade Range", Range(0, 1)) = 0.2
        [Float] _Density("Density", Range(0, 5)) = 1
        _SwaySpeed("Sway Speed", Range(0, 5)) = 1
        _SwayFrequency ("Sway Frequency", Range(0, 5)) = 1
        _SwayAmount("Sway Amount", Range(0, 1)) = 0.15
    }

    SubShader
    {
        Tags 
        { 
            "RenderType" = "Transparent" 
            "RenderPipeline" = "UniversalPipeline"
            "Queue" = "Transparent"
        }

        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off

        Pass
        {
            HLSLPROGRAM

            #pragma vertex vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct MeshData
            {
                float4 positionOS : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct Interpolaters
            {
                float4 positionHCS : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            TEXTURE2D(_BaseMap);
            SAMPLER(sampler_BaseMap);


            CBUFFER_START(UnityPerMaterial)
                half4 _ColorA;
                half4 _ColorB;
                float _ColorCycleSpeed;
                float _GlowIntensity;
                float4 _BaseMap_ST;
                float _ScrollSpeed;
                float _FadeRange;
                float _Density;
                float _SwaySpeed;
                float _SwayFrequency;
                float _SwayAmount;  
            CBUFFER_END
            float rand(float3 co)
            {
                return frac(sin(dot(co, float3(12.9898, 78.233, 45.164))) * 43758.5453);
            }
            Interpolaters vert(MeshData IN)
            {
                Interpolaters OUT;
                
                float3 pivotWorld = float3(unity_ObjectToWorld[0].w, unity_ObjectToWorld[1].w, unity_ObjectToWorld[2].w);
                float seed = rand(pivotWorld) * 6.28318; // 0 -> 2*PI

                float swayMask = IN.uv.y;

                float swayX = sin(_Time.y * _SwaySpeed + seed);
                swayX += sin(_Time.y * _SwaySpeed * _SwayFrequency * 2.3 + seed * 3.1) * 0.25;

                IN.positionOS.x += swayX * swayMask * _SwayAmount;

                OUT.positionHCS = TransformObjectToHClip(IN.positionOS);
                OUT.uv = TRANSFORM_TEX(IN.uv, _BaseMap);
                return OUT;
            }

            

            half4 frag(Interpolaters IN) : SV_Target
            {
                
                // fade cắt dần đi mép ảnh
                float fadeX = smoothstep(0.0, _FadeRange, IN.uv.x) * smoothstep(1.0, 1.0 - _FadeRange, IN.uv.x);
                float fadeY = smoothstep(0.0, _FadeRange, IN.uv.y) * smoothstep(1.0, 1.0 - _FadeRange, IN.uv.y);
                float fade = fadeX * fadeY;

                // tạo UV động để texture di chuyển theo thời gian
                float2 animatedUV2 = IN.uv - float2(_Time.y * 0.3 * _ScrollSpeed, _Time.y * 0.3 * _ScrollSpeed);
                float2 animatedUV1 = IN.uv + float2(_Time.y * 0.1 * _ScrollSpeed, 0);
                float2 animatedUV3 = IN.uv * 1.25 + float2(-_Time.y * 0.15, _Time.y * 0.18);

                // lấy màu từ texture với UV động
                float noise1 = SAMPLE_TEXTURE2D(_BaseMap, sampler_BaseMap, animatedUV1).r;
                float noise2 = SAMPLE_TEXTURE2D(_BaseMap, sampler_BaseMap, animatedUV2).r;
                float noise3 = SAMPLE_TEXTURE2D(_BaseMap, sampler_BaseMap, animatedUV3).r;
                float idleNoise = saturate(noise1 + noise2 + noise3 ) * _Density;
                
                // chuyển màu dần dần qua lại giữa 2 sắc tím, dùng noise3 làm offset
                float colorPhase = sin(_Time.y * _ColorCycleSpeed + noise3 * 3.14159) * 0.5 + 0.5;
                half4 baseColor = lerp(_ColorA, _ColorB, colorPhase);

                // Glow nhẹ ở vùng noise đậm -> cảm giác phát sáng huyền ảo thay vì khói phẳng
                float glow = pow(idleNoise, 2) * _GlowIntensity;
                baseColor.rgb += glow * baseColor.rgb;

                // nhịp "thở" tổng thể của khói
                float breath = sin(_Time.y * 1.3) * 0.2 + 0.8;

                float4 finalColor = baseColor;
                finalColor.a *= idleNoise * breath * fade;

                return finalColor;
            }
            ENDHLSL
        }
    }
}
