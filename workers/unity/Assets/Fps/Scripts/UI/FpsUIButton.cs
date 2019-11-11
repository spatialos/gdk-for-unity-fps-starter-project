using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Fps.UI
{
    public class FpsUIButton : Button
    {
        public Image TargetFrame;
        public Image TargetFill;
        public Image[] TextOptions = new Image[2];
        public bool IsDarkThemeEnabled;

        // Initialise colour values.
        private readonly Color32 lightColor = Color.white;
        private readonly Color32 darkColor = new Color32(69, 71, 71, 255);
        private readonly Color32 hoverColor = new Color32(150, 150, 150, 255);
        private readonly Color32 disabledFill = new Color32(255, 255, 255, 100);
        private readonly Color32 disabledText = new Color32(69, 71, 71, 100);

        private bool pointerInButton;
        private Image targetText;

#if UNITY_EDITOR
        protected override void OnValidate()
        {
            base.OnValidate();

            if (TargetFrame == null)
            {
                throw new NullReferenceException("Missing reference to target frame image.");
            }

            if (TargetFill == null)
            {
                throw new NullReferenceException("Missing reference to the target fill image.");
            }

            if (TextOptions == null || TextOptions.Length == 0)
            {
                throw new NullReferenceException("Missing reference to the text option images.");
            }
        }
#endif

        // Use this for initialization
        protected override void Awake()
        {
            targetText = TextOptions[0];
            DrawDisabledState();
        }

        protected override void OnEnable()
        {
            if (pointerInButton)
            {
                DrawHoverState();
            }
            else
            {
                DrawNeutralStrate();
            }

            base.OnEnable();
        }

        protected override void OnDisable()
        {
            DrawDisabledState();
        }

        public override void OnPointerDown(PointerEventData eventData)
        {
            // Pressed state
            if (interactable)
            {
                if (IsDarkThemeEnabled)
                {
                    targetText.color = lightColor;
                    TargetFill.color = lightColor;
                }
                else
                {
                    targetText.color = lightColor;
                    TargetFill.color = darkColor;
                }
            }
        }

        public override void OnPointerUp(PointerEventData eventData)
        {
            // If pointer up while hovered, return to hovered state
            if (!interactable)
            {
                return;
            }

            if (EventSystem.current.IsPointerOverGameObject())
            {
                DrawHoverState();
            }
            else
            {
                DrawNeutralStrate();
            }
        }

        public override void OnPointerExit(PointerEventData eventData)
        {
            pointerInButton = false;
            if (interactable)
            {
                DrawNeutralStrate();
            }
        }

        public override void OnPointerEnter(PointerEventData eventData)
        {
            pointerInButton = true;
            if (interactable)
            {
                DrawHoverState();
            }
        }

        private void DrawNeutralStrate()
        {
            if (IsDarkThemeEnabled)
            {
                targetText.color = lightColor;
                TargetFill.color = darkColor;
            }
            else
            {
                targetText.color = darkColor;
                TargetFill.color = lightColor;
            }

            TargetFrame.color = lightColor;
        }

        private void DrawHoverState()
        {
            TargetFill.color = hoverColor;
            targetText.color = lightColor;
        }

        private void DrawDisabledState()
        {
            pointerInButton = false;

            TargetFill.color = disabledFill;
            TargetFrame.color = disabledFill;
            targetText.color = disabledText;
        }
    }
}
