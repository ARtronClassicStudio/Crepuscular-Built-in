
Shader "Hidden/Crepuscular" 
{
	Properties { [HideInInspector] _MainTex("Main Texture",2D) = "white" {}}

	SubShader
	{
		Pass
		{
			CGPROGRAM
			#pragma vertex vert_img
			#pragma fragment frag
			
			#include "UnityCG.cginc"

			uniform sampler2D _MainTex;
			float3 _LightPos;
			float _NumSamples;
			float _Density;
			float _Weight;
			float _Decay;
			float _Exposure;
			float _IlluminationDecay;
			float4 _ColorRay;

			float4 frag(v2f_img i) : COLOR
			{
	
				float2 deltaTexCoord = (i.uv - _LightPos.xy) * (_LightPos.z < 0 ? -1 : 1);
				deltaTexCoord *= 1.0f / _NumSamples * _Density;
				float2 uv = i.uv;
				float3 color = tex2D(_MainTex,uv).xyz;

				for (int i = 0; i < (_LightPos.z < 0 ? 0 : _NumSamples * _LightPos.z); i++)
				{
						uv -= deltaTexCoord;
						float3 sample = tex2D(_MainTex, uv).xyz;
						sample *= _IlluminationDecay * (_Weight / _NumSamples);
						color += sample * _ColorRay.xyz;
						_IlluminationDecay *= _Decay;		
					
				}
				return float4(color * _Exposure, 1);
			}
			ENDCG
		}
	}
}
