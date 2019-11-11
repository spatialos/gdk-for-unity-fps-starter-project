using UnityEngine;

namespace Fps.UI
{
    [RequireComponent(typeof(MobileAnalogueControls))]
    public class MobileUI : MonoBehaviour, IMobileUI
    {
        // TODO Could be possible to use Unity's inspector actions/events to hook these up instead?
        // I.e. StandardButtons are responsible for setting up links to MobileUI

        public TouchscreenButton JumpButton;
        public TouchscreenButton AimButton;
        public TouchscreenButton FireButton;
        public TouchscreenButton MenuButton;
        public RectTransform LeftStickKnob;


        public float StickMoveDistance = 100;
        public float SprintDistanceBuffer = 30;

        public bool ShowHitboxes;


        public float MaxStickDistance => StickMoveDistance;
        public Vector2 LookDelta => analogueControls.LookDelta;
        public Vector2 MoveTotal => analogueControls.MoveTotal;
        public bool JumpPressed { get; private set; }
        public bool ShootPressed { get; private set; }
        public bool MenuPressed { get; private set; }
        public bool IsAiming { get; private set; }
        public bool ShootHeld { get; private set; }

        private MobileAnalogueControls analogueControls;

        private void OnValidate()
        {
            var buttons = GetComponentsInChildren<TouchscreenButton>();
            foreach (var button in buttons)
            {
                button.Hitbox.color = new Color(1, 0, 0, ShowHitboxes ? .3f : 0);
            }
        }

        private void Awake()
        {
            analogueControls = GetComponent<MobileAnalogueControls>();
        }

        private void Update()
        {
            if (MoveTotal.x < -MaxStickDistance)
            {
                var diff = MoveTotal.x + MaxStickDistance;
                analogueControls.AdjustMoveStartPosition(new Vector2(diff, 0));
            }
            else if (MoveTotal.x > MaxStickDistance)
            {
                var diff = MoveTotal.x - MaxStickDistance;
                analogueControls.AdjustMoveStartPosition(new Vector2(diff, 0));
            }

            if (MoveTotal.y < -MaxStickDistance)
            {
                var diff = MoveTotal.y + MaxStickDistance;
                analogueControls.AdjustMoveStartPosition(new Vector2(0, diff));
            }
            else if (MoveTotal.y > MaxStickDistance + SprintDistanceBuffer)
            {
                var diff = MoveTotal.y - (MaxStickDistance + SprintDistanceBuffer);
                analogueControls.AdjustMoveStartPosition(new Vector2(0, diff));
            }


            LeftStickKnob.localPosition = Vector3.ClampMagnitude(MoveTotal, MaxStickDistance);
        }

        private void LateUpdate()
        {
            JumpPressed = false;
            ShootPressed = false;
            MenuPressed = false;
        }

        private void OnEnable()
        {
            JumpButton.OnButtonDown += Jump;
            AimButton.OnButtonDown += ToggleAim;
            AimButton.OnButtonUp += ToggleAim;
            FireButton.OnButtonDown += StartFiring;
            FireButton.OnButtonUp += StopFiring;
            MenuButton.OnButtonDown += OpenMenu;
        }

        private void OnDisable()
        {
            JumpButton.OnButtonDown -= Jump;
            AimButton.OnButtonDown -= ToggleAim;
            AimButton.OnButtonUp -= ToggleAim;
            FireButton.OnButtonDown -= StartFiring;
            FireButton.OnButtonUp -= StopFiring;
            MenuButton.OnButtonDown -= OpenMenu;
        }


        private void Jump()
        {
            JumpPressed = true;
        }

        private void ToggleAim()
        {
            IsAiming = !IsAiming;
        }

        private void StartFiring()
        {
            ShootHeld = true;
        }

        private void StopFiring()
        {
            ShootHeld = false;
        }

        private void OpenMenu()
        {
            MenuPressed = true;
        }
    }
}
