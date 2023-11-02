#pragma once

#define TEX(name) TEXTURE2D(name); SAMPLER(sampler ## name)
#define TEX_SAMPLE(name, uv) SAMPLE_TEXTURE2D(name, sampler ## name, uv)

static const float3 _Ambient = 0.2;

float Dither(uint2 uv)
{
    float ditherThresholds[16] =
    {
        1.0 / 17.0, 9.0 / 17.0, 3.0 / 17.0, 11.0 / 17.0,
        13.0 / 17.0, 5.0 / 17.0, 15.0 / 17.0, 7.0 / 17.0,
        4.0 / 17.0, 12.0 / 17.0, 2.0 / 17.0, 10.0 / 17.0,
        16.0 / 17.0, 8.0 / 17.0, 14.0 / 17.0, 6.0 / 17.0
    };
    uint index = (uint(uv.x) % 4) * 4 + uint(uv.y) % 4;
    return ditherThresholds[index];
}