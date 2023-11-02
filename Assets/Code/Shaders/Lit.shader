Shader "Custom/Lit"
{
    Properties
    {
        _BaseColor ("Base Color", Color) = (0.8, 0.8, 0.8, 1)
        _SpecularColor ("Specular Color", Color) = (1, 1, 1, 0.0)
        _NoiseMap ("Noise Map [RGB]", 2D) = "gray" {}
        _NoiseScale ("Noise Scale", float) = 2
        _NoiseExp("Noise Exponent", float) = 2
        _NoiseContrast ("Noise Contrast", Range(-2.0, 2.0)) = 0.2
        [Toggle] _NoodleNoise("Noodle Noise", float) = 1.0
        [Toggle] _InvertNoise("Invert Noise", float) = 1.0
        _AffineUVs("Affine UVs", Range(0.0, 1.0)) = 0.0
    }

    SubShader
    {
        Pass
        {
            HLSLPROGRAM

            #pragma multi_compile_instancing

            #pragma vertex vert
            #pragma fragment frag

            #include "LitPass.hlsl"
            ENDHLSL
        }
    }
}