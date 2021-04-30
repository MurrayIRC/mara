Shader "Unlit/Textured" {
    Properties {
        _MainTex ("Texture", 2D) = "white" {}
        _Pattern ("Pattern", 2D) = "white" {}
        
        _Rock ("Rock", 2D) = "white" {}
    }
    SubShader {
        Tags { "RenderType"="Opaque" }

        Pass {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            #define TAU 6.28318530718

            struct ModelData {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct Interpolators {
                float4 vertex : SV_POSITION;
                float2 uv : TEXCOORD0;
                float3 worldPos : TEXCOORD1;
            };

            sampler2D _MainTex;
            sampler2D _Pattern;
            sampler2D _Rock;

            float GetWave(float coord) {
                float wave = cos((coord - _Time.y * 0.1) * TAU * 5) * 0.5 + 0.5;
                wave *= 1 - coord;
                return wave;
            }

            Interpolators vert (ModelData v) {
                Interpolators o;
                //o.worldPos = mul(unity_ObjectToWorld, v.vertex);
                o.worldPos = mul(UNITY_MATRIX_M, v.vertex); // object to world
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv; 
                //o.uv.x += _Time.y * 0.1; 
                return o;
            }

            fixed4 frag (Interpolators i) : SV_Target {
                float2 topDownProjection = i.worldPos.xz;
                fixed4 col = tex2D(_MainTex, topDownProjection);
                fixed4 rock = tex2D(_Rock, topDownProjection);
                float pattern = tex2D(_Pattern, i.uv);

                float4 finalColor = lerp(rock, col, pattern);

                return finalColor;
            }
            ENDCG
        }
    }
}
