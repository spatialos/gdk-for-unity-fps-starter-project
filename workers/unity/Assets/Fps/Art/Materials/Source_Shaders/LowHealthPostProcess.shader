
Shader "FPS/LowHealthVignette"
{
	HLSLINCLUDE

        #include "Packages/com.unity.postprocessing/PostProcessing/Shaders/StdLib.hlsl"

        TEXTURE2D_SAMPLER2D(_MainTex, sampler_MainTex);
        TEXTURE2D_SAMPLER2D(_VignetteMaterial, sampler_VignetteMaterial);
        TEXTURE2D_SAMPLER2D(_VignetteMask, sampler_VignetteMask);
        float _DAMAGE_YAW;
        float _DAMAGE_FOCUS;
        float _DAMAGE_INTENSITY;
        float _HEALTH_VALUE;

        float4 Frag(VaryingsDefault i) : SV_Target
        {
            float4 maskValue = SAMPLE_TEXTURE2D(_VignetteMask, sampler_VignetteMask, i.texcoord);
            float4 vignetteValue = SAMPLE_TEXTURE2D(_VignetteMaterial, sampler_VignetteMaterial, i.texcoord);
            float4 color = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.texcoord);

            float vignetteColour = pow((((vignetteValue.r+color.g)+color.b)*0.3),1.25);
            float health_saturation = pow(saturate(_HEALTH_VALUE),4.0);
            float angle = (fmod(_DAMAGE_YAW,360.0)/360.0);
            float3 emissive = lerp(lerp(lerp(float3(float2(vignetteColour,vignetteColour),vignetteColour),
			color.rgb,pow(health_saturation,0.15)),float3(1,0,0),
			pow(pow(maskValue.r,2.0),(health_saturation*19.25+0.75))),float3(1,0,0),
			pow((pow(maskValue.b,1.0)*pow(saturate((8.0*(_DAMAGE_INTENSITY*(1.0 - saturate((_DAMAGE_FOCUS*min(min(saturate(abs((maskValue.g-angle))),
			saturate(abs((maskValue.g-(angle+(-1.0)))))),saturate(abs((maskValue.g-(angle+1.0))))))))))),0.5)),1.5));

			float4 finalColor = color;
			finalColor.rgb = emissive;
            finalColor.rgb = lerp(color.rgb, finalColor.rgb, maskValue.xxx);
            return finalColor;
        }
    ENDHLSL

    SubShader
    {
        Cull Off ZWrite Off ZTest Always

        Pass
        {
            HLSLPROGRAM

                #pragma vertex VertDefault
                #pragma fragment Frag

            ENDHLSL
        }
    }
}
