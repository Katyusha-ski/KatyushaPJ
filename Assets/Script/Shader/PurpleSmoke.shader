Shader "Custom/PurpleSmokeEthereal"
{
    Properties
    {
        _MainTex ("Noise Texture", 2D) = "white" {}
        _ColorA ("Color A (đậm)", Color) = (0.35, 0.05, 0.55, 1)
        _ColorB ("Color B (sáng)", Color) = (0.75, 0.4, 0.95, 1)
        _ColorCycleSpeed ("Color Cycle Speed", Range(0, 2)) = 0.3
        _MorphSpeed ("Morph Speed", Range(0, 5)) = 1
        _Density ("Density", Range(0, 5)) = 1.5
        _GlowIntensity ("Glow Intensity", Range(0, 3)) = 1.2
        _SwayAmount ("Sway Amount", Range(0, 1)) = 0.15
        _SwaySpeed ("Sway Speed", Range(0, 5)) = 1
        _SwayFrequency ("Sway Frequency", Range(0, 5)) = 1
    }

    SubShader
    {
        Tags
        {  "RenderType" = "Transparent"
           "Queue" = "Transparent"
        }
        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off
        Cull Off

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct MeshData
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct Interpolators
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float4 _ColorA;
            float4 _ColorB;
            float _ColorCycleSpeed;
            float _MorphSpeed;
            float _Density;
            float _GlowIntensity;
            float _SwayAmount;
            float _SwaySpeed;
            float _SwayFrequency;

            // Hàm random giả lập (pseudo-random), dùng làm "seed" để mỗi object
            // đung đưa lệch pha nhau, tránh cảm giác đồng bộ giả tạo khi có nhiều cụm khói
            float rand(float3 co)
            {
                return frac(sin(dot(co, float3(12.9898, 78.233, 45.164))) * 43758.5453);
            }

            Interpolators vert (MeshData v)
            {
                Interpolators o;

                // Lấy vị trí gốc (pivot) của object trong world space để làm seed ngẫu nhiên
                float3 pivotWorld = float3(unity_ObjectToWorld[0].w, unity_ObjectToWorld[1].w, unity_ObjectToWorld[2].w);
                float seed = rand(pivotWorld) * 6.28318; // 0 -> 2*PI

                // Đung đưa mạnh ở phần ngọn (uv.y cao), gần như đứng yên ở gốc (uv.y thấp)
                // -> giống chuyển động tán lá cây bị gió lay, gốc cố định, ngọn lắc
                float swayMask = v.uv.y;

                float swayX = sin(_Time.y * _SwaySpeed + seed) * _SwayAmount;
                float swayZ = cos(_Time.y * _SwaySpeed * 0.6 + seed * 1.7) * _SwayAmount * 0.5;

                // Thêm một lớp dao động tần số cao, biên độ nhỏ để chuyển động không đều tăm tắp,
                // tạo cảm giác "rung" tự nhiên hơn là lắc qua lại đều đặn
                swayX += sin(_Time.y * _SwaySpeed * _SwayFrequency * 2.3 + seed * 3.1) * _SwayAmount * 0.25;

                v.vertex.x += swayX * swayMask;
                v.vertex.z += swayZ * swayMask;

                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            fixed4 frag (Interpolators i) : SV_Target
            {
                float time = _Time.y * _MorphSpeed;

                // 3 lớp noise trôi theo hướng/tốc độ khác nhau -> khói cuộn huyền ảo, ít lặp khuôn mẫu
                float2 uv1 = i.uv + float2(time * 0.35, time * 0.25);
                float2 uv2 = i.uv - float2(time * 0.22, time * 0.4);
                float2 uv3 = i.uv * 1.7 + float2(-time * 0.15, time * 0.18);

                fixed noise1 = tex2D(_MainTex, uv1).r;
                fixed noise2 = tex2D(_MainTex, uv2).r;
                fixed noise3 = tex2D(_MainTex, uv3).r;

                float idleNoise = saturate(noise1 * noise2 + noise3 * 0.5) * _Density;

                // Nhịp "thở" tổng thể của đám khói
                float breath = sin(_Time.y * 1.3) * 0.2 + 0.8;

                // Chuyển màu dần dần qua lại giữa 2 sắc tím, dùng noise3 làm offset
                // để các vùng khói khác nhau lệch pha màu, tránh đổi màu đồng loạt cứng nhắc
                float colorPhase = sin(_Time.y * _ColorCycleSpeed + noise3 * 3.14159) * 0.5 + 0.5;
                fixed4 baseColor = lerp(_ColorA, _ColorB, colorPhase);

                // Glow nhẹ ở vùng noise đậm -> cảm giác phát sáng huyền ảo thay vì khói phẳng
                float glow = pow(idleNoise, 2) * _GlowIntensity;
                baseColor.rgb += glow * baseColor.rgb;

                fixed4 finalColor = baseColor;
                finalColor.a *= idleNoise * breath;

                // Làm mềm biên để khói tan dần ra mép, không bị cắt cứng
                float fadeX = smoothstep(0.0, 0.35, i.uv.x) * smoothstep(1.0, 0.65, i.uv.x);
                float fadeY = smoothstep(0.0, 0.35, i.uv.y) * smoothstep(1.0, 0.65, i.uv.y);
                finalColor.a *= (fadeX * fadeY);

                return finalColor;
            }
            ENDCG
        }
    }
    FallBack Off
}