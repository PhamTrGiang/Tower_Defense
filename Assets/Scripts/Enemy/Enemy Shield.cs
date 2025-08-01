using System.Collections;
using UnityEngine;

public class EnemyShield : MonoBehaviour
{
    [Header("Impact Details")]
    [SerializeField] private Material shieldMaterial;
    [SerializeField] private float defaultShieldGlow = 1;
    [SerializeField] private float impactShieldGlow = 3;
    [SerializeField] private float impactScaleMultiplier = .97f;
    [SerializeField] private float impactSpeed = .1f;
    // [SerializeField] private float impactResetDuration = .5f;

    private float defaultScale;
    private string shieldFresnelParameter = "_FresnelPower";
    private Coroutine currentCo;

    private void Start()
    {
        defaultScale = transform.localScale.x;
        shieldMaterial = Instantiate(shieldMaterial);
    }

    public void ActivateShieldImpact()
    {
        if (currentCo != null)
            StopCoroutine(currentCo);

        currentCo = StartCoroutine(ImpactCo());
    }

    private IEnumerator ImpactCo()
    {
        yield return StartCoroutine(ShieldChangeCo(impactShieldGlow, defaultScale * impactScaleMultiplier, impactSpeed));

        StartCoroutine(ShieldChangeCo(defaultShieldGlow, defaultScale, impactSpeed));

    }

    private IEnumerator ShieldChangeCo(float targetGlow, float targetScale, float duration)
    {
        float time = 0;
        float startGlow = shieldMaterial.GetFloat(shieldFresnelParameter);
        Vector3 initialScale = transform.localScale;
        Vector3 newTargetScale = new Vector3(targetScale, targetScale, targetScale);

        while (time < duration)
        {
            transform.localScale = Vector3.Lerp(initialScale, newTargetScale, time / duration);

            float newGlow = Mathf.Lerp(startGlow, targetGlow, time / duration);
            shieldMaterial.SetFloat(shieldFresnelParameter, newGlow);

            time += Time.deltaTime;
            yield return null;
        }

        transform.localScale = newTargetScale;
        shieldMaterial.SetFloat(shieldFresnelParameter, targetGlow);
    }
}
