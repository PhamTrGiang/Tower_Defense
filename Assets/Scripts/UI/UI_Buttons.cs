using UnityEngine;
using UnityEngine.EventSystems;

public class UI_Buttons : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler
{
    private UI_Animator uiAnimator;
    private RectTransform myRect;

    [SerializeField] private float showcaseScale = 1.1f;
    [SerializeField] private float scaleDuration = .25f;

    private Coroutine scaleCoroutine;
    [Space]
    [SerializeField] private UI_TextBlinkEffect myTextBlinkEffect;

    private void Awake()
    {
        uiAnimator = GetComponentInParent<UI_Animator>();
        myRect = GetComponent<RectTransform>();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (scaleCoroutine != null)
            StopCoroutine(scaleCoroutine);

        scaleCoroutine = StartCoroutine(uiAnimator.ChangeScaleCo(myRect, showcaseScale, scaleDuration));

        if (myTextBlinkEffect != null)
            myTextBlinkEffect.EnableBlink(false);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (scaleCoroutine != null)
            StopCoroutine(scaleCoroutine);

        scaleCoroutine = StartCoroutine(uiAnimator.ChangeScaleCo(myRect, 1, scaleDuration));

        if (myTextBlinkEffect != null)
            myTextBlinkEffect.EnableBlink(true);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        myRect.localScale = new Vector3(1, 1, 1);
    }
}
