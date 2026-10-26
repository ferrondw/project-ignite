Shader "Simple CRT"
{
    Properties
    {
        [HideInInspector] _MainTex ("Camera Image", 2D) = "white" {}

        _WhiteNoiseOnOff ("White Noise", Float) = 0
        _WhiteNoiseStrength ("White Noise Strength", Range(0, 1)) = 0.08

        _ScanlineOnOff ("Scanlines", Float) = 1
        _ScanlineStrength ("Scanline Strength", Range(0, 0.2)) = 0.04
        _ScanlineFrequency ("Scanline Frequency", Range(100, 1200)) = 800

        _MonochormeOnOff ("Monochrome", Float) = 0

        _ScreenJumpLevel ("Screen Jump", Range(0, 1)) = 0

        _FlickeringOnOff ("Flickering", Float) = 1
        _FlickeringStrength ("Flickering Strength", Range(0, 0.2)) = 0.025
        _FlickeringCycle ("Flickering Cycle", Range(1, 100)) = 20

        _SlippageOnOff ("Slippage", Float) = 1
        _SlippageStrength ("Slippage Strength", Range(0, 0.1)) = 0.015
        _SlippageInterval ("Slippage Interval", Range(10, 300)) = 100
        _SlippageScrollSpeed ("Slippage Speed", Range(0, 20)) = 5
        _SlippageNoiseOnOff ("Slippage Bands", Float) = 1
        _SlippageSize ("Slippage Size", Range(0.1, 8)) = 2

        _ChromaticAberrationOnOff ("Chromatic Aberration", Float) = 1
        _ChromaticAberrationStrength ("Chromatic Strength", Range(0, 0.02)) = 0.002

        _MultipleGhostOnOff ("Ghosting", Float) = 1
        _MultipleGhostStrength ("Ghosting Strength", Range(0, 0.02)) = 0.003

        _DecalTex ("Decal Texture", 2D) = "white" {}
        _DecalTexOnOff ("Decal On/Off", Float) = 0
        _DecalTexPos ("Decal Position", Vector) = (0, 0, 0, 0)
        _DecalTexScale ("Decal Scale", Vector) = (1, 1, 0, 0)
        _DecalStrength ("Decal Strength", Range(0, 1)) = 1

        _FilmDirtTex ("Film Dirt Texture", 2D) = "gray" {}
        _FilmDirtOnOff ("Film Dirt", Float) = 0
        _FilmDirtStrength ("Film Dirt Strength", Range(0, 1)) = 0.25
        _FilmDirtSpeed ("Film Dirt Speed", Range(0, 10)) = 1

        _LetterBoxOnOff ("Letterbox", Float) = 0
        _LetterBoxType ("Letterbox Type (0 Black, 1 Blur)", Float) = 0
        _LetterBoxSize ("Letterbox Size", Range(0, 0.2)) = 0.08
    }

    SubShader
    {
        Tags
        {
            "RenderType" = "Opaque"
            "Queue" = "Overlay"
        }

        Cull Off
        ZWrite Off
        ZTest Always

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D _MainTex;
            float4 _MainTex_TexelSize;

            sampler2D _DecalTex;
            sampler2D _FilmDirtTex;

            float _WhiteNoiseOnOff;
            float _WhiteNoiseStrength;

            float _ScanlineOnOff;
            float _ScanlineStrength;
            float _ScanlineFrequency;

            float _MonochormeOnOff;
            float _ScreenJumpLevel;

            float _FlickeringOnOff;
            float _FlickeringStrength;
            float _FlickeringCycle;

            float _SlippageOnOff;
            float _SlippageStrength;
            float _SlippageInterval;
            float _SlippageScrollSpeed;
            float _SlippageNoiseOnOff;
            float _SlippageSize;

            float _ChromaticAberrationOnOff;
            float _ChromaticAberrationStrength;

            float _MultipleGhostOnOff;
            float _MultipleGhostStrength;

            float _DecalTexOnOff;
            float4 _DecalTexPos;
            float4 _DecalTexScale;
            float _DecalStrength;

            float _FilmDirtOnOff;
            float _FilmDirtStrength;
            float _FilmDirtSpeed;

            float _LetterBoxOnOff;
            float _LetterBoxType;
            float _LetterBoxSize;

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            float Hash21(float2 p)
            {
                p = frac(p * float2(123.34, 456.21));
                p += dot(p, p + 45.32);
                return frac(p.x * p.y);
            }

            fixed4 frag(v2f i) : SV_Target
            {
                float2 uv = i.uv;
                float time = _Time.y;

                // Screen jump
                uv.y = frac(uv.y + _ScreenJumpLevel);

                // Horizontal slippage bands
                float pattern = sin(
                    i.uv.y * _SlippageInterval
                    + time * _SlippageScrollSpeed
                );

                float slippageMask = pow(
                    abs(pattern),
                    max(_SlippageSize, 0.1)
                );

                float stepMask = step(pattern, 0.0);

                float slippageOffset =
                    _SlippageOnOff *
                    _SlippageNoiseOnOff *
                    _SlippageStrength *
                    slippageMask *
                    stepMask;

                uv.x += slippageOffset;

                // Flickering
                float flickerPattern = pow(
                    abs(sin(i.uv.y * _FlickeringCycle + time)),
                    10.0
                );

                float flickerNoise = Hash21(
                    float2(floor(time * 20.0), 7.13)
                );

                float flicker = 1.0 -
                    _FlickeringOnOff *
                    _FlickeringStrength *
                    flickerPattern *
                    flickerNoise;

                // Chromatic aberration
                float chroma =
                    _ChromaticAberrationStrength *
                    _ChromaticAberrationOnOff;

                float red = tex2D(
                    _MainTex,
                    uv - float2(chroma, 0)
                ).r;

                float green = tex2D(_MainTex, uv).g;

                float blue = tex2D(
                    _MainTex,
                    uv + float2(chroma, 0)
                ).b;

                fixed4 color = fixed4(red, green, blue, 1);

                // Multiple ghost images
                float ghostAmount =
                    _MultipleGhostOnOff *
                    _MultipleGhostStrength;

                fixed4 ghost1 = tex2D(
                    _MainTex,
                    uv - float2(ghostAmount, 0)
                );

                fixed4 ghost2 = tex2D(
                    _MainTex,
                    uv - float2(ghostAmount * 2.0, 0)
                );

                color = lerp(
                    color,
                    color * 0.8 + ghost1 * 0.15 + ghost2 * 0.05,
                    saturate(_MultipleGhostOnOff)
                );

                // Film dirt
                if (_FilmDirtOnOff > 0.5)
                {
                    float2 dirtUV = uv * 2.0 +
                        float2(time * _FilmDirtSpeed, time * 0.37);

                    float dirt1 = tex2D(
                        _FilmDirtTex, dirtUV * 0.5
                    ).r;

                    float dirt2 = tex2D(
                        _FilmDirtTex, dirtUV * 1.7
                    ).g;

                    float dirt3 = tex2D(
                        _FilmDirtTex, dirtUV * 3.1
                    ).b;

                    float dirt = max(
                        max(dirt1, dirt2),
                        dirt3
                    );

                    float dirtMask = smoothstep(0.45, 0.85, dirt);

                    color.rgb *= 1.0 -
                        dirtMask * _FilmDirtStrength;
                }

                // Decal overlay
                if (_DecalTexOnOff > 0.5)
                {
                    float2 decalScale = max(
                        abs(_DecalTexScale.xy),
                        float2(0.001, 0.001)
                    );

                    float2 decalUV =
                        (uv - _DecalTexPos.xy) / decalScale;

                    fixed4 decal = tex2D(_DecalTex, decalUV);

                    color.rgb = lerp(
                        color.rgb,
                        decal.rgb,
                        saturate(decal.a * _DecalStrength)
                    );
                }

                // Scanlines
                float scanline = sin(
                    (i.uv.y + time * 0.02) *
                    _ScanlineFrequency
                );

                color.rgb -=
                    scanline *
                    _ScanlineStrength *
                    _ScanlineOnOff;

                // Flicker
                color.rgb *= flicker;

                // White noise overlay
                if (_WhiteNoiseOnOff > 0.5)
                {
                    float noise = Hash21(
                        floor(i.uv * _ScreenParams.xy)
                        + floor(time * 30.0)
                    );

                    color.rgb = lerp(
                        color.rgb,
                        noise.xxx,
                        saturate(_WhiteNoiseStrength)
                    );
                }

                // Monochrome
                if (_MonochormeOnOff > 0.5)
                {
                    float gray = dot(
                        color.rgb,
                        float3(0.299, 0.587, 0.114)
                    );

                    color.rgb = gray.xxx;
                }

                // Letterbox: top and bottom bars
                if (_LetterBoxOnOff > 0.5)
                {
                    float edge = min(uv.y, 1.0 - uv.y);

                    if (edge < _LetterBoxSize)
                    {
                        if (_LetterBoxType < 0.5)
                        {
                            color.rgb = 0;
                        }
                        else
                        {
                            float2 texel =
                                _MainTex_TexelSize.xy * 4.0;

                            fixed4 blur = tex2D(_MainTex, uv);

                            blur += tex2D(
                                _MainTex, uv + float2(texel.x, 0)
                            );

                            blur += tex2D(
                                _MainTex, uv - float2(texel.x, 0)
                            );

                            blur += tex2D(
                                _MainTex, uv + float2(0, texel.y)
                            );

                            blur += tex2D(
                                _MainTex, uv - float2(0, texel.y)
                            );

                            color.rgb = blur.rgb / 5.0;
                        }
                    }
                }

                // Keep output within a valid color range
                color.rgb = saturate(color.rgb);
                color.a = 1.0;

                return color;
            }
            ENDCG
        }
    }

    Fallback Off
}