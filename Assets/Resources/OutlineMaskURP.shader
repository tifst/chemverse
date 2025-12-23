Shader "Universal Render Pipeline/OutlineMaskURP"
{
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent+100" }
        Pass
        {
            Name "Mask"
            Tags { "LightMode"="UniversalForward" }
            ZWrite Off
            ZTest Always
            ColorMask 0

            Stencil
            {
                Ref 1
                Pass Replace
            }
        }
    }
}