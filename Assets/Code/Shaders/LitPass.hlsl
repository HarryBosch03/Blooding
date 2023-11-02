#pragma once

#include "../ShaderLibrary/UnityInput.hlsl"
#include "../ShaderLibrary/Common.hlsl"
#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/SpaceTransforms.hlsl"

CBUFFER_START(UnityPerMaterial)
float4 _BaseColor;
float4 _SpecularColor;
TEX(_NoiseMap);
float _NoiseScale;
float _NoiseExp;
float _NoiseContrast;
float _NoodleNoise;
float _InvertNoise;
float _AffineUVs;
CBUFFER_END

struct Attributes
{
    float3 positionOS : POSITION;
    float3 normalOS : NORMAL;
    float2 uv : TEXCOORD0;
};

struct Varyings
{
    float4 positionCS : SV_POSITION;
    float3 positionWS : POSITION_WS;
    float3 normalWS : NORMAL_WS;
    float2 uv0 : TEXCOORD0;
    noperspective float2 uv1 : TEXCOORD1;
};

Varyings vert(Attributes input)
{
    Varyings output;

    output.positionWS = TransformObjectToWorld(input.positionOS.xyz);
    output.positionCS = TransformObjectToHClip(input.positionOS.xyz);
    output.normalWS = TransformObjectToWorldNormal(input.normalOS.xyz);
    output.uv0 = input.uv;
    output.uv1 = input.uv;
    
    return output;
}

half4 frag(Varyings input) : SV_TARGET
{
    float3 normal = normalize(input.normalWS);
    float3 light = normalize(_WorldSpaceLightPos0.xyz);
    float3 view = normalize(_WorldSpaceCameraPos - input.positionWS);
    float2 uv = lerp(input.uv0, input.uv1, _AffineUVs);

    float ndl = saturate(dot(normal, light));
    float ndv = saturate(dot(normal, view));
    
    float3 noise = TEX_SAMPLE(_NoiseMap, uv / _NoiseScale);
    noise = lerp(noise, abs(noise * 2.0 - 1.0), _NoodleNoise);
    noise = lerp(noise, 1.0 - noise, _InvertNoise);
    noise = pow(noise, _NoiseExp);
    
    float3 albedo = _BaseColor.rgb * lerp(1.0 - _NoiseContrast, 1.0 + _NoiseContrast, noise.r);

    float2 screenPos = round(input.positionCS.xy / 4.0) * 4.0 / _ScreenParams.xy;
    float2 specUV = (screenPos.x - screenPos.y / 8.0) / 4.0;
    float specular = saturate(pow(TEX_SAMPLE(_NoiseMap, specUV).r, 12.0) * 400.0);

    albedo += specular * _SpecularColor.rgb * _SpecularColor.a;// * pow(1 - ndv, 4);

    float3 final = 0.0;
    final += albedo * _Ambient;
    
    final += albedo * ndl * (1 - _Ambient);

    final = pow(final, 1 / 2.2);
    final = floor(final * 32) / 32.0;
    final = pow(final, 2.2);
    
    return float4(final, 1.0);
}