Shader "Tutorial/VertexOffset" {
    Properties {
        _ColorA ("Color A", Color) = (1, 1, 1, 1)
        _ColorB ("Color B", Color) = (1, 1, 1, 1)
        _ColorStart ("Color Start", Range(0, 1)) = 0
        _ColorEnd ("Color End", Range(0, 1)) = 1
        _WaveAmplitude ("Wave Amplitude", Range(0, 0.2)) = 0.2
    }
    SubShader {
        // subshader tags
        Tags { 
            "RenderType"="Opaque"
            "Queue"="Geometry" 
        }

        Pass {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            #define TAU 6.28318530718

            float4 _ColorA;
            float4 _ColorB;
            float _ColorStart;
            float _ColorEnd;
            float _WaveAmplitude;

            struct MeshData {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float2 uv0 : TEXCOORD0;
            };

            struct Interpolators {
                float4 vertex : SV_POSITION;
                float3 normal : TEXCOORD0;
                float2 uv : TEXCOORD1;
            };

            float GetWave(float2 uv) {
                float2 uvsCentered = uv * 2 - 1;
                float radialDistance = length(uvsCentered);
                //return float4(radialDistance.xxx, 1);

                float wave = cos((radialDistance - _Time.y * 0.1) * TAU * 5) * 0.5 + 0.5;
                wave *= 1- radialDistance;
                return wave;
            }

            float InverseLerp(float a, float b, float v) {
                return (v - a) / (b - a);
            }

            Interpolators vert (MeshData v) {
                Interpolators o;

                v.vertex.y = GetWave(v.uv0) * _WaveAmplitude;

                o.vertex = UnityObjectToClipPos(v.vertex);
                o.normal = UnityObjectToWorldNormal(v.normal);
                o.uv = v.uv0;
                return o;
            }

            float4 frag (Interpolators i) : SV_Target {
                return GetWave(i.uv);
            }
            ENDCG
        }
    }
}
