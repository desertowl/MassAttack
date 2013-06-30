Shader "DOG/DiffuseNormalSpecEmis"
{
	Properties 
	{
_diffuse("_diffuse", 2D) = "black" {}
_emissive("_emissive", 2D) = "black" {}
_specular("_specular", 2D) = "black" {}
_normal("_normal", 2D) = "black" {}
_EmissiveColor("_EmissiveColor", Color) = (1,1,1,1)

	}
	
	SubShader 
	{
		Tags
		{
"Queue"="Geometry"
"IgnoreProjector"="False"
"RenderType"="Opaque"

		}

		
Cull Back
ZWrite On
ZTest LEqual
ColorMask RGBA
Fog{
}


		CGPROGRAM
#pragma surface surf BlinnPhongEditor  vertex:vert
#pragma target 2.0


sampler2D _diffuse;
sampler2D _emissive;
sampler2D _specular;
sampler2D _normal;
float4 _EmissiveColor;

			struct EditorSurfaceOutput {
				half3 Albedo;
				half3 Normal;
				half3 Emission;
				half3 Gloss;
				half Specular;
				half Alpha;
				half4 Custom;
			};
			
			inline half4 LightingBlinnPhongEditor_PrePass (EditorSurfaceOutput s, half4 light)
			{
half3 spec = light.a * s.Gloss;
half4 c;
c.rgb = (s.Albedo * light.rgb + light.rgb * spec);
c.a = s.Alpha;
return c;

			}

			inline half4 LightingBlinnPhongEditor (EditorSurfaceOutput s, half3 lightDir, half3 viewDir, half atten)
			{
				half3 h = normalize (lightDir + viewDir);
				
				half diff = max (0, dot ( lightDir, s.Normal ));
				
				float nh = max (0, dot (s.Normal, h));
				float spec = pow (nh, s.Specular*128.0);
				
				half4 res;
				res.rgb = _LightColor0.rgb * diff;
				res.w = spec * Luminance (_LightColor0.rgb);
				res *= atten * 2.0;

				return LightingBlinnPhongEditor_PrePass( s, res );
			}
			
			struct Input {
				float2 uv_diffuse;
float2 uv_normal;
float2 uv_emissive;
float2 uv_specular;

			};

			void vert (inout appdata_full v, out Input o)
			{
				UNITY_INITIALIZE_OUTPUT(Input, o);
				float4 VertexOutputMaster0_0_NoInput = float4(0,0,0,0);
				float4 VertexOutputMaster0_1_NoInput = float4(0,0,0,0);
				float4 VertexOutputMaster0_2_NoInput = float4(0,0,0,0);
				float4 VertexOutputMaster0_3_NoInput = float4(0,0,0,0);
			}
			

			void surf (Input IN, inout EditorSurfaceOutput o) {
				o.Normal = float3(0.0,0.0,1.0);
				o.Alpha = 1.0;
				o.Albedo = 0.0;
				o.Emission = 0.0;
				o.Gloss = 0.0;
				o.Specular = 0.0;
				o.Custom = 0.0;
				
float4 Tex2D0=tex2D(_diffuse,(IN.uv_diffuse.xyxy).xy);
float4 Tex2D2=tex2D(_normal,(IN.uv_normal.xyxy).xy);
float4 UnpackNormal0=float4(UnpackNormal(Tex2D2).xyz, 1.0);
float4 Tex2D4=tex2D(_emissive,(IN.uv_emissive.xyxy).xy);
float4 Multiply0=_EmissiveColor * Tex2D4;
float4 Tex2D3=tex2D(_specular,(IN.uv_specular.xyxy).xy);
float4 Divide0=Tex2D3 / float4( 25,25,25,25 );
float4 Master0_3_NoInput = float4(0,0,0,0);
float4 Master0_5_NoInput = float4(1,1,1,1);
float4 Master0_7_NoInput = float4(0,0,0,0);
float4 Master0_6_NoInput = float4(1,1,1,1);
o.Albedo = Tex2D0;
o.Normal = UnpackNormal0;
o.Emission = Multiply0;
o.Gloss = Divide0;

				o.Normal = normalize(o.Normal);
			}
		ENDCG
	}
	Fallback "Diffuse"
}