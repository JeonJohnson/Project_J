﻿Shader "JGH/ARC_Shader"
{
	Properties
	{
		//기본적으로 값이 추가되면 그만큼 비어지는거

		[PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
		
		_Color ("Tint", Color) = (1,1,1,1)
        _PivotAngle("PivotAngle", Range(0, 360)) = 0 //시작 각도? 0 기준 right, 반시계방향

		_FovAngle("ArcAngle", Range(0,360)) = 90
	}

	SubShader	
	{
		Tags
		{ 
			"Queue"="Transparent" 
			"IgnoreProjector"="True" 
			"RenderType"="Transparent" 
			"PreviewType"="Plane"
			"CanUseSpriteAtlas"="True"
		}

		Cull Off
		Lighting Off
		ZWrite Off
		Blend One OneMinusSrcAlpha

		Pass
		{
		CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"
			
			struct appdata_t
			{
				float4 vertex   : POSITION;
				float4 color    : COLOR;
				float2 texcoord : TEXCOORD0;
			};

			struct v2f
			{
				float4 vertex   : SV_POSITION;
				fixed4 color    : COLOR;
				float2 texcoord  : TEXCOORD0;
			};
			
			fixed4 _Color;

			v2f vert(appdata_t IN)
			{
				v2f OUT;
				OUT.vertex = UnityObjectToClipPos(IN.vertex);
				OUT.texcoord = IN.texcoord;
				OUT.color = IN.color * _Color;
				return OUT;
			}

			sampler2D _MainTex;
			sampler2D _AlphaTex;
			float _AlphaSplitEnabled;

			float _PivotAngle;

			float _ArcAngle;
			float _Arc;
			

			fixed4 SampleSpriteTexture (float2 uv)
			{
				fixed4 color = tex2D (_MainTex, uv);

#if UNITY_TEXTURE_ALPHASPLIT_ALLOWED
				if (_AlphaSplitEnabled)
					color.a = tex2D (_AlphaTex, uv).r;
#endif //UNITY_TEXTURE_ALPHASPLIT_ALLOWED

				return color;
			}

			fixed4 frag(v2f IN) : SV_Target
			{
				fixed4 c = SampleSpriteTexture (IN.texcoord) * IN.color;
				c.rgb *= c.a;

				float _Angle = 270 - _PivotAngle;

				//draw start, draw end 
				_Arc = (360  - (_ArcAngle) )/2;
                float startAngle = _Angle - _Arc;
                float endAngle = _Angle + _Arc;

                // check offsets
                float offset0 = clamp(0, 360, startAngle + 360);
                float offset360 = clamp(0, 360, endAngle - 360);

                // convert uv to atan coordinates
                float2 atan2Coord = float2(lerp(-1, 1, IN.texcoord.x), lerp(-1, 1, IN.texcoord.y));
                float atanAngle = atan2(atan2Coord.y, atan2Coord.x) * 57.3; // angle in degrees
				// convert angle to 360 system
                if(atanAngle < 0) 
				{	
					atanAngle = 360 + atanAngle;
				}

                if(atanAngle >= startAngle && atanAngle <= endAngle) discard;
                if(atanAngle <= offset360) discard;
                if(atanAngle >= offset0) discard;

				return c;
			}
		ENDCG
		}
	}
}