#pragma once

#include "../ShaderLibrary/UnityInput.hlsl"
#include "../ShaderLibrary/Common.hlsl"
#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/SpaceTransforms.hlsl"

struct Attributes
{
    float3 positionOS : POSITION;
    float4 color : COLOR;
};

struct Varyings
{
    float4 positionCS : SV_POSITION;
    float4 color : COLOR;
};

Varyings vert(Attributes input)
{
    Varyings output;

    output.positionCS = TransformObjectToHClip(input.positionOS.xyz);
    output.color = input.color;
    
    return output;
}

half4 frag(Varyings input) : SV_TARGET
{
    float dither = Dither(input.positionCS.xy * 0.5);
    clip(input.color.a - dither);
    return floor(input.color * 32) / 32.0;
}