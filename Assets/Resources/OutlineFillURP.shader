Shader "Universal Render Pipeline/OutlineFillURP"
{
    Properties
    {
        _OutlineColor("Outline Color", Color) = (1,1,0,1)
        _OutlineWidth("Width", Range(0,10)) = 4
    }

    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent+110" }

        Pass
        {
            Name "Outline"
            Tags { "LightMode"="UniversalForward" }

            Cull Front
            ZWrite Off
            ZTest Less

            Blend SrcAlpha OneMinusSrcAlpha

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            float4 _OutlineColor;
            float _OutlineWidth;

            struct Attributes
            {
                float4 positionOS : POSITION;
                float3 normalOS   : NORMAL;
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
            };

            Varyings vert(Attributes v)
            {
                Varyings o;

                float3 norm = normalize(v.normalOS);
                float3 offset = norm * _OutlineWidth * 0.01;

                float3 displacedPos = v.positionOS.xyz + offset;

                o.positionHCS = TransformObjectToHClip(displacedPos);
                return o;
            }

            half4 frag(Varyings i) : SV_Target
            {
                return _OutlineColor;
            }

            ENDHLSL
        }
    }
}