using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Animator))]
public class KillNotification : MonoBehaviour
{
    public float Lifetime;

    public Color LabelColor;
    public Color NameColor;

    [Header("Relegate options")] public float RelegateScale = .8f;
    public float RelegateDuration = .5f;
    public float RelegateYBuffer = 5;
    public bool RelegateUpward;

    private float relegateTime;
    private int relegateLevel;
    private float targetScale;
    private float targetYOffset;
    private bool isRelegating;

    private static string LabelColorHex;
    private static string NameColorHex;

    public Text KillTextObject;

    private void OnValidate()
    {
        LabelColorHex = ColorUtility.ToHtmlStringRGB(LabelColor);
        NameColorHex = ColorUtility.ToHtmlStringRGB(NameColor);
    }

    public Action OnDespawn;

    private string Label => $"<color=#{LabelColorHex}>You killed</color>";

    private float lifeRemaining;

    private Animator animator;
    private float startHeight;

    private bool despawnedExternally;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        startHeight = GetComponent<RectTransform>().rect.height;
    }

    public void Despawn()
    {
        despawnedExternally = true;
        lifeRemaining = 0;
    }

    public void OnEnable()
    {
        lifeRemaining = Lifetime;
        StartCoroutine(LifeTicker());
    }

    public void SetName(string name)
    {
        var Name = $"<color=#{NameColorHex}>{name}</color>";
        KillTextObject.text = $"{Label} {Name}";
    }

    private IEnumerator LifeTicker()
    {
        while (lifeRemaining > 0)
        {
            lifeRemaining -= Time.deltaTime;
            animator.SetFloat("LifeRemaining", lifeRemaining);
            yield return null;
        }

        animator.SetFloat("LifeRemaining", lifeRemaining);
    }

    // Destroy() is called from animation event
    private void Destroy()
    {
        Destroy(gameObject);
        if (despawnedExternally)
        {
            return;
        }

        OnDespawn();
    }


    public void Relegate()
    {
        relegateLevel++;

        targetScale = Mathf.Pow(RelegateScale, relegateLevel);
        targetYOffset = 0;

        for (var i = 0; i <= relegateLevel; i++)
        {
            targetYOffset += startHeight * Mathf.Pow(RelegateScale, i) * (i == 0 || i == relegateLevel ? .5f : 1f);
        }

        // When relegating, bump lifetime back up again so player can revel in their multikill glory. Stagger their
        // lifetimes based on relegation level so they don't all disappear at once.
        lifeRemaining = Lifetime - Lifetime * 0.1f * relegateLevel;

        if (isRelegating)
        {
            return;
        }

        StartCoroutine(DoRelegation());
    }

    private IEnumerator DoRelegation()
    {
        relegateTime = 0;
        while (relegateTime < RelegateDuration)
        {
            var t = relegateTime / RelegateDuration;

            relegateTime += Time.deltaTime;
            var setScaleTo = Vector3.Lerp(Vector3.one, Vector3.one * targetScale, t);
            var setPositionTo = Vector3.Lerp(Vector3.zero, Vector3.up * targetYOffset, t);
            transform.localScale = setScaleTo;
            transform.localPosition = -setPositionTo;
            yield return null;
        }

        transform.localScale = Vector3.one * targetScale;
        transform.localPosition =
            Vector3.up * (targetYOffset + RelegateYBuffer * relegateLevel) * (RelegateUpward ? 1 : -1);

        isRelegating = false;
    }
}
