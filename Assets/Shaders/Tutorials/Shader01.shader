Shader "Tutorial/Shader01" {
    Properties {
        _ColorA ("Color A", Color) = (1, 1, 1, 1)
        _ColorB ("Color B", Color) = (1, 1, 1, 1)
        _ColorStart ("Color Start", Range(0, 1)) = 0
        _ColorEnd ("Color End", Range(0, 1)) = 1
    }
    SubShader {
        // subshader tags
        Tags { 
            "RenderType"="Opaque"
            "Queue"="Transparent" 
        }

        Pass {
            // pass tags
            Cull Off
            ZWrite Off
            ZTest LEqual
            Blend One One       // additive, for light effects n shit
            //Blend DstColor Zero // multiply

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            #define TAU 6.28318530718

            float4 _ColorA;
            float4 _ColorB;
            float _ColorStart;
            float _ColorEnd;

            // automatically filled out by unity
            struct MeshData {
                float4 vertex : POSITION; // vertex position
                float3 normal : NORMAL;
                // float4 tangent : TANGENT;
                //float4 color : COLOR; // vertex color
                float2 uv0 : TEXCOORD0; // uv0 diffuse / normal map textures
                //float2 uv1 : TEXCOORD1; // uv1 lightmap coordinates
            };

            struct Interpolators { // Passes data from vertex shader to fragment shader
                float4 vertex : SV_POSITION; // clip-space position
                //float2 uv0 : TEXCOORD0; // does not refer to uv channels, could be anything  
                float3 normal : TEXCOORD0;
                float2 uv : TEXCOORD1;
            };

            Interpolators vert (MeshData v) {
                Interpolators o;
                o.vertex = UnityObjectToClipPos(v.vertex); // local space to clip-space
                o.normal = UnityObjectToWorldNormal(v.normal);
                o.uv = v.uv0; //(v.uv0 + _Offset) * _Scale; // passthrough
                return o;
            }

            // start with float, go down in precision later if performance is a concern.
            // float (32-bit float)
            // half  (16-bit float)
            // fixed (12-bit float (platform-specific, but it's lower))
            // float4 -> half4 -> fixed4
            // float4x4 -> half4x4 -> fixed4x4

            float InverseLerp(float a, float b, float v) {
                return (v - a) / (b - a);
            }

            float4 frag (Interpolators i) : SV_Target {
                //float t = abs(frac(i.uv.x * 5) * 2 - 1);
                //float t = cos(i.uv.x * 25);
    
                float xOffset = cos(i.uv.x * TAU * 8) * 0.01;
                float t = cos((i.uv.y + xOffset - _Time.y * 0.1) * TAU * 5) * 0.5 + 0.5;
                t *= 1 - i.uv.y;

                float topBottomRemover = (abs(i.normal.y) < 0.999);
                float waves = t * topBottomRemover;

                float4 gradient = lerp(_ColorA, _ColorB, i.uv.y);
                
                return gradient * waves;

                // saturate() is the same as Clamp01 in unity. shit name.
                //float t = saturate(InverseLerp(_ColorStart, _ColorEnd, i.uv.x));
                // frac = v - floor(v);
                // frac can show us if anything isn't clamped.
                // t = frac(t);
                //t = clamp(t, 0, 1);
                //float4 outColor = lerp(_ColorA, _ColorB, t);
                //return outColor;
            }
            ENDCG
        }
    }
}
