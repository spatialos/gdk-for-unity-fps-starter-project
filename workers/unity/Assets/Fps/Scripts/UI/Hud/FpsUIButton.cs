using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Fps
{
    public class FpsUIButton : Button
    {
        public Image TargetFrame;
        public Image TargetText;
        public Image TargetFill;

        public Image[] TextOptions = new Image[2];

        // Define visual style of button.
        public bool DarkTheme;

        // Initialise colour values.
        private readonly Color32 lightFill = Color.white;
        private readonly Color32 darkFill = new Color32(69, 71, 71, 255);

        private readonly Color32 lightText = Color.white;
        private readonly Color32 darkText = new Color32(69, 71, 71, 255);

        private readonly Color32 hoverFill = new Color32(150, 150, 150, 255);

        private readonly Color32 disabledFill = new Color32(255, 255, 255, 100);
        private readonly Color32 disabledText = new Color32(69, 71, 71, 100);

        private bool pointerInButton;

        // Use this for initialization
        protected override void Awake()
        {
            OnDisable(); // Ensure initial state is greyed out if gameObject becomes visible but button is disabled
        }

        protected override void Start()
        {
            TargetText = TextOptions[0];
        }

        protected override void OnEnable()
        {
            if (pointerInButton)
            {
                HoverState();
            }
            else
            {
                NeutralState();
            }

            base.OnEnable();
        }

        protected override void OnDisable()
        {
            pointerInButton = false;

            if (TargetFill != null)
            {
                TargetFill.color = disabledFill;
            }

            if (TargetFrame != null)
            {
                TargetFrame.color = disabledFill;
            }

            if (TargetText != null)
            {
                TargetText.color = disabledText;
            }
        }

        public override void OnPointerDown(PointerEventData eventData)
        {
            // Pressed state
            if (interactable)
            {
                if (DarkTheme)
                {
                    if (TargetText != null)
                    {
                        TargetText.color = darkText;
                    }

                    if (TargetFill != null)
                    {
                        TargetFill.color = lightFill;
                    }
                }
                else
                {
                    if (TargetText != null)
                    {
                        TargetText.color = lightText;
                    }

                    if (TargetFill != null)
                    {
                        TargetFill.color = darkFill;
                    }
                }
            }
        }

        public override void OnPointerUp(PointerEventData eventData)
        {
            // If pointer up while hovered, return to hovered state
            if (interactable)
            {
                if (EventSystem.current.IsPointerOverGameObject())
                {
                    HoverState();
                }
                else
                {
                    NeutralState();
                }
            }
        }

        public override void OnPointerExit(PointerEventData eventData)
        {
            pointerInButton = false;
            if (interactable)
            {
                NeutralState();
            }
        }

        public override void OnPointerEnter(PointerEventData eventData)
        {
            pointerInButton = true;
            if (interactable)
            {
                HoverState();
            }
        }

        private void NeutralState()
        {
            if (DarkTheme)
            {
                if (TargetText != null)
                {
                    TargetText.color = lightText;
                }

                if (TargetFrame != null)
                {
                    TargetFrame.color = lightFill;
                }

                if (TargetFill != null)
                {
                    TargetFill.color = darkFill;
                }
            }
            else
            {
                if (TargetText != null)
                {
                    TargetText.color = darkText;
                }

                if (TargetFrame != null)
                {
                    TargetFrame.color = lightFill;
                }

                if (TargetFill != null)
                {
                    TargetFill.color = lightFill;
                }
            }
        }

        private void HoverState()
        {
            if (TargetFill != null)
            {
                TargetFill.color = hoverFill;
            }

            if (TargetText != null)
            {
                TargetText.color = lightText;
            }
        }

        // Used by animation events to change the visible text element on button.
        public void SetText(int index)
        {
            TargetText = TextOptions[index];
        }
    }
}
