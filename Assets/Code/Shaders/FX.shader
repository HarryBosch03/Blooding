Shader "Custom/FX"
{
    Properties
    {

    }
    SubShader
    {
        Pass
        {
            HLSLPROGRAM

            #pragma multi_compile_instancing

            #pragma vertex vert
            #pragma fragment frag

            #include "./FXPass.hlsl"
            ENDHLSL
        }
    }
}