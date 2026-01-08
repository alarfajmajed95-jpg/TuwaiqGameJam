Shader "Unlit/FlagWave"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _WaveAmplitude ("Wave Amplitude", Range(0,1)) = 0.2
        _WaveFrequency ("Wave Frequency", Float) = 1.0
        _WaveSpeed ("Wave Speed", Float) = 1.0
        _WaveTop ("Wave Top", Range(0, 1)) = 0.3
        _FlagAnchor ("Flag Anchor", Range(-1,1)) = 0.0
        _DepthIntensity ("Depth Intensity", Range(0,3)) = 0.5
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        Cull Off

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float _WaveAmplitude;
            float _WaveFrequency;
            float _WaveSpeed;
            float _FlagAnchor;
            float _DepthIntensity;
            float _WaveTop;

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                float wave : TEXCOORD1;
            };

            v2f vert(appdata v)
            {
                v2f o;
                float3 pos = v.vertex.xyz;

                float x01 = saturate(pos.x + 0.5);
                float anchor = saturate(x01 + _FlagAnchor);
                float phase = pos.x * _WaveFrequency + _Time.y * _WaveSpeed;
                float wave = sin(phase) * _WaveAmplitude * anchor;

                pos.z += wave;
                pos.y += wave * _WaveTop;

                o.wave = wave;
                o.vertex = UnityObjectToClipPos(float4(pos,1));
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            float4 frag(v2f i) : SV_Target
            {
                float4 col = tex2D(_MainTex, i.uv);
                float brightness = saturate(1.0 - i.wave * _DepthIntensity);
                col.rgb *= brightness;
                return col;
            }
            ENDCG
        }
    }
}
