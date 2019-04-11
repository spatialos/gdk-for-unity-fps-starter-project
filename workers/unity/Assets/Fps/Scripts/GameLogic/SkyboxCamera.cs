using UnityEngine;

namespace Fps
{
    public class SkyboxCamera : MonoBehaviour
    {
        private Camera myCamera;

        private void Awake()
        {
            myCamera = GetComponent<Camera>();
        }

        // Update is called once per frame
        private void LateUpdate()
        {
            var playerCamera = Camera.main;
            if (playerCamera == null)
            {
                return;
            }

            myCamera.transform.rotation = playerCamera.transform.rotation;
            myCamera.fieldOfView = playerCamera.fieldOfView;
        }
    }
}
