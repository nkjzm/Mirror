Shader "Mirror/MaskedObject" 
{

Properties 
{
    _Mask("Mask", Int) = 1
    _Color ("Color", Color) = (1,1,1,1)
    _MainTex ("Albedo (RGB)", 2D) = "white" {}
    _Glossiness ("Smoothness", Range(0,1)) = 0.5
    _Metallic ("Metallic", Range(0,1)) = 0.0
}

SubShader 
{

Tags 
{ 
    "RenderType" = "Opaque" 
}

Stencil 
{
    Ref [_Mask]
    Comp Equal
}
    
CGPROGRAM

//#pragma surface surf Standard fullforwardshadows
#pragma surface surf NoLighting
#pragma target 3.0

fixed4 LightingNoLighting(SurfaceOutput s, fixed3 lightDir, fixed atten)
{
	fixed4 c;
	c.rgb = s.Albedo; 
	c.a = s.Alpha;
	return c;
}

sampler2D _MainTex;

struct Input 
{
    float2 uv_MainTex;
};

half _Glossiness;
half _Metallic;
fixed4 _Color;

void surf(Input IN, inout SurfaceOutput o) 
{
    fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color;
    o.Albedo = c.rgb * 0.7;
    o.Alpha = c.a;
}

ENDCG

}

}