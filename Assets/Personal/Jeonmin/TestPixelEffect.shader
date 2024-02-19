Shader"Custom/PixelEffectShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" { }
        _PixelSize ("Pixel Size", Range(1, 1000)) = 1
    }

    SubShader
    {
        Tags { "Queue" = "Overlay" }
LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma exclude_renderers gles xbox360 ps3
            #pragma fragment frag
#include "UnityCG.cginc"

struct appdata
{
    float4 vertex : POSITION;
};

struct v2f
{
    float4 pos : POSITION;
    float2 uv : TEXCOORD0;
};

sampler2D _MainTex;
float _PixelSize;

v2f vert(appdata v)
{
    v2f o;
    o.pos = UnityObjectToClipPos(v.vertex);
    o.uv = v.vertex.xy;
    return o;
}

fixed4 frag(v2f i) : COLOR
{
    float2 uv = i.uv;
    float2 roundedUV = round(uv / _PixelSize) * _PixelSize;

                // Calculate distance from the rounded edge
    float distance = length(uv - roundedUV);
                
                // Apply pixel effect only to the border (within 1 pixel)
    fixed4 col = tex2D(_MainTex, uv);
    col.a = (1 - smoothstep(0.99, 1.01, distance));

    return col;
}
            ENDCG
        }
    }
}
