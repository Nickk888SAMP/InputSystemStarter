Shader "Custom/World Space UV" {
	Properties {
		_Color ("Main Color", Color) = (0.5,0.5,0.5,1)
		_MainTex ("Base (RGB)", 2D) = "white" {}
		//_Ramp ("Toon Ramp (RGB)", 2D) = "gray" {}
		_Scale ("Texture Scale", Float) = 1
	}

	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200
		
		CGPROGRAM
		#pragma surface surf Lambert //ToonRamp

		sampler2D _Ramp;
		sampler2D _MainTex;
		float4 _Color;
		float _Scale;

		// custom lighting function that uses a texture ramp based
		// on angle between light direction and normal
		#pragma lighting Lambert exclude_path:prepass //ToonRamp
		inline half4 LightingToonRamp (SurfaceOutput s, half3 lightDir, half atten)
		{
			#ifndef USING_DIRECTIONAL_LIGHT
			lightDir = normalize(lightDir);
			#endif
			
			half d = dot (s.Normal, lightDir)*0.5 + 0.5;
			half3 ramp = tex2D (_Ramp, float2(d,d)).rgb;
			
			half4 c;
			c.rgb = s.Albedo * _LightColor0.rgb * ramp * (atten);
			c.a = 0;
			return c;
		}
		
		struct Input {
			float3 worldNormal;
			float3 worldPos;
		};

		void surf (Input IN, inout SurfaceOutput o) {
			float2 UV;// = IN.worldPos.xz; // top
			half4 c;// = tex2D(_MainTex, UV * _Scale) * _Color;


			if(abs(IN.worldNormal.x)>0.5) {
			UV = IN.worldPos.yz; // side
			c = tex2D(_MainTex, UV* _Scale); // use WALLSIDE texture
			} else if(abs(IN.worldNormal.z)>0.5) {
			UV = IN.worldPos.xy; // front
			c = tex2D(_MainTex, UV* _Scale); // use WALL texture
			} else {
			UV = IN.worldPos.xz; // top
			c = tex2D(_MainTex, UV* _Scale); // use FLR texture
			}
			o.Albedo = c.rgb * _Color;
			o.Alpha = c.a;
		}
		ENDCG

	} 

	Fallback "Diffuse"
}