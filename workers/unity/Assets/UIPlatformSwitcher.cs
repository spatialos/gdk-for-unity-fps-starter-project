//#define FORCE_MOBILE

using UnityEngine;

public class UIPlatformSwitcher : MonoBehaviour
{
    public GameObject DesktopUI;
    public GameObject MobileUI;

    private void Awake()
    {
#if (UNITY_IOS || UNITY_ANDROID || FORCE_MOBILE)
        MobileUI.SetActive(true);
        DesktopUI.SetActive(false);
#else
        MobileUI.SetActive(false);
        DesktopUI.SetActive(true);
#endif
    }
}
