Shader "DOG/SpaceFX"
{
	Properties 
	{
_stars("_stars", 2D) = "black" {}
_gas("_gas", 2D) = "black" {}
_noise("_noise", 2D) = "black" {}
_speed("_speed", Float) = 1
_foreground("_foreground", Color) = (1,1,1,1)

	}
	
	SubShader 
	{
		Tags
		{
"Queue"="Geometry"
"IgnoreProjector"="False"
"RenderType"="Opaque"

		}

		
Cull Off
ZWrite On
ZTest LEqual
ColorMask RGBA
Fog{
}


		CGPROGRAM
#pragma surface surf BlinnPhongEditor  vertex:vert
#pragma target 3.0


sampler2D _stars;
sampler2D _gas;
sampler2D _noise;
float _speed;
float4 _foreground;

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
				float2 uv_gas;
float2 uv_noise;
float2 uv_stars;
float4 color : COLOR;

			};

			void vert (inout appdata_full v, out Input o) {
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
				
float4 ASin0=asin(_speed.xxxx);
float4 Multiply4=ASin0 * _Time;
float4 UV_Pan1=float4((IN.uv_gas.xyxy).x + Multiply4.x,(IN.uv_gas.xyxy).y + Multiply4.x,(IN.uv_gas.xyxy).z,(IN.uv_gas.xyxy).w);
float4 Tex2D1=tex2D(_gas,UV_Pan1.xy);
float4 Multiply3=_foreground * Tex2D1;
float4 UV_Pan2=float4((IN.uv_noise.xyxy).x + _Time.x,(IN.uv_noise.xyxy).y + _Time.x,(IN.uv_noise.xyxy).z,(IN.uv_noise.xyxy).w);
float4 Tex2D2=tex2D(_noise,UV_Pan2.xy);
float4 Tex2D0=tex2D(_stars,(IN.uv_stars.xyxy).xy);
float4 Multiply1=Tex2D0 * IN.color;
float4 Multiply5=Tex2D2 * Multiply1;
float4 Add0=Multiply3 + Multiply5;
float4 Master0_1_NoInput = float4(0,0,1,1);
float4 Master0_2_NoInput = float4(0,0,0,0);
float4 Master0_3_NoInput = float4(0,0,0,0);
float4 Master0_4_NoInput = float4(0,0,0,0);
float4 Master0_5_NoInput = float4(1,1,1,1);
float4 Master0_7_NoInput = float4(0,0,0,0);
float4 Master0_6_NoInput = float4(1,1,1,1);
o.Albedo = Add0;

				o.Normal = normalize(o.Normal);
			}
		ENDCG
	}
	Fallback "Diffuse"
}