Shader "SoftLiu/Unlit/DarkenShader"
{
	Properties
	{
		//_MainTex("Base (RGB)", 2D) = "white" {}
		_RADIUSBUCEX("_RADIUSBUCEX", Range(0, 1)) = 0.1
		_RADIUSBUCEY("_RADIUSBUCEY", Range(0, 0.5)) = 0.05
		_OtherAlphValue("_OtherAlphValue", Range(0, 1)) = 0.1
		_CenterAlphValue("_CenterAlphValue", Range(0, 1)) = 0.1
	}
		SubShader
		{
			Tags
			{
				"Queue" = "Transparent"
			}
			LOD 100

			Pass
			{
				Blend SrcAlpha OneMinusSrcAlpha
				ZWrite off

				CGPROGRAM
				#pragma vertex vert
				#pragma fragment frag
			// make fog work
			#pragma multi_compile_fog

			#include "UnityCG.cginc"



			float _RADIUSBUCEX;
			float _RADIUSBUCEY;
			sampler2D _MainTex;
			float _OtherAlphValue;
			float _CenterAlphValue;

			struct appdata
			{
				float4 pos : POSITION;
				float2 uv : TEXCOORD;
				fixed4 col : COLOR;
			};

			struct v2f
			{
				float4 pos : SV_POSITION;
				float2 uv : TEXCOORD0;
				fixed4 col : COLOR;
				float2 radiusUV : TEXCOORD1;
			};
			v2f vert(appdata v)
			{
				v2f o;
				o.pos = UnityObjectToClipPos(v.pos);
				o.uv = v.uv;
				//将模型UV坐标原点置为中心原点,为了方便计算
				o.radiusUV = v.uv - float2(0.5, 0.5);
				o.col = v.col;
				return o;
			}

			fixed3 maxColorToGray(fixed4 col)
			{
				fixed maxV = max(max(col.r, col.g), col.b);
				return fixed3(maxV, maxV, maxV);
			}

			fixed4 frag(v2f i) :SV_Target
			{
				fixed4 col = i.col;

				if (abs(i.radiusUV.x) <= _RADIUSBUCEX && abs(i.radiusUV.y) <= _RADIUSBUCEY)
				{
					float len1 = length(float2(_RADIUSBUCEX, _RADIUSBUCEX) - float2(0.5, 0.5));
					float len2 = length(abs(i.radiusUV) - float2(0, 0));
					if (len2 >= len1)
					{
						col = tex2D(_MainTex, i.uv);
					}
					else
					{
						//if (length(abs(i.radiusUV) - float2(0.5 - _RADIUSBUCEX, 0.5 - _RADIUSBUCEX)) >= _RADIUSBUCEX)
						//{
						//	col = tex2D(_MainTex, i.uv);
						//}
						//else
						{
							col = tex2D(_MainTex, i.uv) * fixed4(1, 1, 1, _CenterAlphValue);
							//discard;
						}
					}
				}
				else
				{
					col = tex2D(_MainTex, i.uv);
				}

				//col = tex2D(_MainTex, i.uv) * fixed4(0,0,1,0.5);
				//col.rgb = maxColorToGray(col);
				return fixed4(col.rgb, col.a * _OtherAlphValue);
			}
			ENDCG
		}
		}
}
