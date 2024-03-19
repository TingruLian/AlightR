shader "Custom/Wireframe" {
	properties{
		   _Color("Color",Color) = (1.0,1.0,1.0,1.0)
		   _EdgeColor("Edge Color",Color) = (1.0,1.0,1.0,1.0)
		   _EdgeColor2("Edge Color",Color) = (1.0,1.0,1.0,1.0)
		   _Width("Width",Range(0,1)) = 0.2
	}
		SubShader{
			Tags {"Queue" = "Transparent" "IgnoreProjector" = "True"}
			Cull Front

			Pass {

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma target 3.0
			#include "UnityCG.cginc"

			struct a2v {
				half4 uv : TEXCOORD0;
				half4 vertex : POSITION;
			};

			struct v2f {
				half4 pos : SV_POSITION;
				half4 uv : TEXCOORD0;
			};

			fixed4 _Color;
			fixed4 _EdgeColor;
			float _Width;

			v2f vert(a2v v)
			{
				v2f o;
				o.uv = v.uv;
				o.pos = UnityObjectToClipPos(v.vertex);
				return o;
			}

			fixed4 frag(v2f i) : COLOR
			{
				fixed4 col;

				float LowX = step(_Width, i.uv.x);
				float LowY = step(_Width, i.uv.y);
				float HighX = step(i.uv.x, 1.0 - _Width);
				float HighY = step(i.uv.y, 1.0 - _Width);
				float num = LowX * LowY*HighX*HighY;
				col = lerp(_EdgeColor, _Color, num);

				clip((1-num)-0.1f);
				return col;
			}
			ENDCG
		}


			Cull Back
			Pass {
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"

			struct a2v {
				half4 uv : TEXCOORD0;
				half4 vertex : POSITION;

			};

			struct v2f {
				half4 pos : SV_POSITION;
				half4 uv : TEXCOORD0;

			};

			fixed4 _Color;
			fixed4 _EdgeColor2;
			float _Width;

			v2f vert(a2v v)
			{
				v2f o;
				o.uv = v.uv;
				o.pos = UnityObjectToClipPos(v.vertex);
				return o;
			}

			fixed4 frag(v2f i) : COLOR
			{
				fixed4 col;

				float LowX = step(_Width, i.uv.x);
				float LowY = step(_Width, i.uv.y);
				float HighX = step(i.uv.x, 1.0 - _Width);
				float HighY = step(i.uv.y, 1.0 - _Width);
				float num = LowX * LowY*HighX*HighY;
				col = lerp(_EdgeColor2, _Color, num);

				clip((1 - num) - 0.1f);
				return col;
			}
			ENDCG
		}
	}
	FallBack "Diffuse"
}
