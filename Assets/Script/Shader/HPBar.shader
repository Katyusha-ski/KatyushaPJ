Shader "UI/HPBar"
{
    Properties
    {
        _Health ("Health", Range(0, 1)) = 1
        _GlintTex ("Glint Texture", 2D) = "black" {}
        _GlintColor ("Glint Color", Color) = (1, 0.2, 1, 1)
        _GlintSpeedX ("Glint Speed X", Float) = 0.5
        _GlintSpeedY ("Glint Speed Y", Float) = 0.2

        _StencilComp ("Stencil Comparison", Float) = 8
        _Stencil ("Stencil ID", Float) = 0
        _StencilOp ("Stencil Operation", Float) = 0
        _StencilWriteMask ("Stencil Write Mask", Float) = 255
        _StencilReadMask ("Stencil Read Mask", Float) = 255
        _ColorMask ("Color Mask", Float) = 15
    }

    SubShader
    {
        Tags
        {
            "Queue"             = "Transparent"
            "IgnoreProjector"   = "True"
            "RenderType"        = "Transparent"
            "PreviewType"       = "Plane"
            "CanUseSpriteAtlas" = "True"
        }

        Stencil
        {
            Ref       [_Stencil]
            Comp      [_StencilComp]
            Pass      [_StencilOp]
            ReadMask  [_StencilReadMask]
            WriteMask [_StencilWriteMask]
        }

        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off
        ZTest [unity_GUIZTestMode]
        Cull Off
        ColorMask [_ColorMask]

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"
            #include "UnityUI.cginc"

            struct MeshData
            {
                float4 positionOS : POSITION;
                float2 uv         : TEXCOORD0;
                float4 color      : COLOR;
            };

            struct Interpolators
            {
                float4 positionHCS   : SV_POSITION;
                float2 uv            : TEXCOORD0;
                float4 color         : COLOR;
                float4 worldPosition : TEXCOORD1;
            };

            sampler2D _GlintTex;
            float _Health;
            half4 _GlintColor;
            float _GlintSpeedX;
            float _GlintSpeedY;
            float4 _ClipRect;

            Interpolators vert(MeshData IN)
            {
                Interpolators OUT;
                OUT.worldPosition = IN.positionOS;
                OUT.positionHCS   = UnityObjectToClipPos(IN.positionOS);
                OUT.uv            = IN.uv;
                OUT.color         = IN.color;
                return OUT;
            }

            float InverseLerp(float a, float b, float value)
            {
                return saturate((value - a) / max(b - a, 1e-5));
            }

            half4 frag(Interpolators IN) : SV_Target
            {
                if (!UnityGet2DClipping(IN.worldPosition.xy, _ClipRect))
                    discard;

                clip(_Health - IN.uv.x);

                float t = InverseLerp(0.2, 0.8, _Health);

                half3 red    = half3(1, 0, 0);
                half3 yellow = half3(1, 1, 0);
                half3 green  = half3(0, 1, 0);
                half t1 = saturate(t * 2.0);
                half t2 = saturate((t - 0.5) * 2.0);
                half3 healthColor = lerp(lerp(red, yellow, t1), green, t2);

                float2 scrolledUV  = IN.uv + float2(_GlintSpeedX, _GlintSpeedY) * _Time.y;
                float glintIntensity = tex2D(_GlintTex, scrolledUV).r;
                glintIntensity = saturate(pow(glintIntensity, 1.5) * 2.0);

                half3 finalColor = saturate(healthColor + _GlintColor.rgb * glintIntensity * 0.8);
                return half4(finalColor, 1.0) * IN.color;
            }

            ENDCG
        }
    }
}