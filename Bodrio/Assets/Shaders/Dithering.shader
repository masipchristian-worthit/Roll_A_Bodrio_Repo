Shader "PostEffect/Dithering"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        Pass
        {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata { float4 vertex : POSITION; float2 uv : TEXCOORD0; };
            struct v2f { float2 uv : TEXCOORD0; float4 vertex : SV_POSITION; };

            sampler2D _MainTex;

            float dither8x8[64] = {
                0, 48, 12, 60, 3, 51, 15, 63,
                32, 16, 44, 28, 35, 19, 47, 31,
                8, 56, 4, 52, 11, 59, 7, 55,
                40, 24, 36, 20, 43, 27, 39, 23,
                2, 50, 14, 62, 1, 49, 13, 61,
                34, 18, 46, 30, 33, 17, 45, 29,
                10, 58, 6, 54, 9, 57, 5, 53,
                42, 26, 38, 22, 41, 25, 37, 21
            };

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            half4 frag(v2f i) : SV_Target
            {
                half4 col = tex2D(_MainTex, i.uv);
                int x = int(i.uv.x * 8) % 8;
                int y = int(i.uv.y * 8) % 8;
                float threshold = dither8x8[y * 8 + x] / 64.0;
                col.rgb = step(threshold, col.rgb);
                return col;
            }
            ENDHLSL
        }
    }
}
