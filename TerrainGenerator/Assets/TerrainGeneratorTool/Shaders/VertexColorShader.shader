Shader "Custom/VertexColorShader"
{

	Properties{
		vMultiplier("vMultiplier", float) = 0
		_MinHeight("Min Height", Range(0,1000)) = 0
		_MaxHeight("Max Height", Range(0,100000)) = 300
		_ColorLow("Color Low", Color) = (0.3, 0.5, 0.2, 0.1)
		_ColorMid("Color Mid", Color) = (0.45, 0.45, 0.45, 1)
		_ColorHigh("Color High", Color) = (0.9, 0.9, 0.9, 1)
		_Shininess("Shininess", Range(0.01, 1)) = 0
		_Texture("Texture", 2D) = "white" {}
		_SpecMap("Specular Map", 2D) = "white" {}
	}
		SubShader{
			Tags { "RenderType" = "Opaque" }


			LOD 200

			CGPROGRAM
			#pragma surface surf Lambert
			#pragma vertex vert
			#pragma target 3.0
			float vMultiplier;

			float _MinHeight;
			float _MaxHeight;
			float4 _Texture_ST;
			sampler2D _Texture;
			sampler2D _SpecMap;
			float4 _ColorLow;
			float4 _ColorMid;
			float4 _ColorHigh;
			float _Shininess;

			struct Input {
				float4 vertColor;
				float2 uv_MainTex;
				float3 worldPos;
			};

			void vert(inout appdata_full vertex, out Input surfInput) {
				UNITY_INITIALIZE_OUTPUT(Input, surfInput);
				vertex.vertex.y *= vMultiplier;
				surfInput.vertColor = vertex.color;


			}

			void surf(Input IN, inout SurfaceOutput output) {
				// calculate height ratio from vertex height
					float height = IN.worldPos.y * vMultiplier;
					float heightRatio = (height - _MinHeight) / (_MaxHeight - _MinHeight);
					heightRatio = clamp(heightRatio, 0.0, 1.0);

					// set surface color based on height ratio
					if (heightRatio < 0.23) {
						output.Albedo = lerp(_ColorLow.rgb, _ColorMid.rgb, heightRatio * 2);
						
					}
					else if (heightRatio < 0.76) {
						output.Albedo = lerp(_ColorMid.rgb, _ColorHigh.rgb, (heightRatio - 0.33) * 3);
					}
					else {
						output.Albedo = _ColorHigh.rgb;
					}
			}
			ENDCG
	}
		FallBack "Diffuse"
}
