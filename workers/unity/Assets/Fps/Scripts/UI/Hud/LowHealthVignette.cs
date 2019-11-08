using System;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

namespace Fps.UI
{
    [Serializable]
    [PostProcess(typeof(LowHealthVignetteProcessor), PostProcessEvent.AfterStack, "FPS/Low Health Vignette")]
    public sealed class LowHealthVignette : PostProcessEffectSettings
    {
        [Range(0f, 1f)] [Tooltip("Health as a decimal.")]
        public FloatParameter health = new FloatParameter { value = 0.5f };

        [Range(0f, 360f)] [Tooltip("Yaw of incoming damage, normalised.")]
        public FloatParameter damageYaw = new FloatParameter { value = 0f };

        [Range(0f, 32f)] [Tooltip("Arc angle of the damage effect.")]
        public FloatParameter damageFocus = new FloatParameter { value = 0f };

        [Range(0f, 1f)] [Tooltip("Strength of the damage effect.")]
        public FloatParameter damageIntensity = new FloatParameter { value = 0f };

        [Tooltip("Vignette Mask.")] public TextureParameter VignetteMask = new TextureParameter();
    }

    public sealed class LowHealthVignetteProcessor : PostProcessEffectRenderer<LowHealthVignette>
    {
        public override void Render(PostProcessRenderContext context)
        {
            if (settings.VignetteMask.value == null)
            {
                return;
            }

            var sheet = context.propertySheets.Get(Shader.Find("FPS/LowHealthVignette"));
            sheet.properties.SetFloat("_HEALTH_VALUE", settings.health);
            sheet.properties.SetFloat("_DAMAGE_YAW", settings.damageYaw);
            sheet.properties.SetFloat("_DAMAGE_FOCUS", settings.damageFocus);
            sheet.properties.SetFloat("_DAMAGE_INTENSITY", settings.damageIntensity);
            sheet.properties.SetTexture("_VignetteMask", settings.VignetteMask);
            context.command.BlitFullscreenTriangle(context.source, context.destination, sheet, 0);
        }
    }
}
