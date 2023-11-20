Shader "Custom/DesaturatedTint"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Color ("Tint Color", Color) = (1, 0, 0, 1) // 기본 색상은 빨강 (RGBA)
        _Desaturation ("Desaturation", Range(0, 1)) = 0.5
    }
 
    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" }
        LOD 100
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
 
            struct appdata_t
            {
                float4 vertex : POSITION;
                float2 texcoord : TEXCOORD0;
            };
 
            struct v2f
            {
                float2 texcoord : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };
 
            sampler2D _MainTex;
            float4 _Color;
            half _Desaturation;
 
            v2f vert (appdata_t v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.texcoord = v.texcoord;
                return o;
            }
 
            half4 frag (v2f i) : SV_Target
            {
                half4 texColor = tex2D(_MainTex, i.texcoord);
                half luminance = dot(texColor.rgb, half3(0.3, 0.59, 0.11));
                half4 desaturatedColor = lerp(texColor, half4(luminance, luminance, luminance, texColor.a), _Desaturation);
                half4 tintedColor = lerp(desaturatedColor, texColor * _Color, _Desaturation);
                return tintedColor;
            }
            ENDCG
        }
    }
}