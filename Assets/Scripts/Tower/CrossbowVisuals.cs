using System.Collections;
using UnityEngine;

public class CrossbowVisuals : MonoBehaviour
{
    private ObjectPoolManager objectPool;

    [Header("Attack Visuals")]
    [SerializeField] private GameObject onHitFx;
    [SerializeField] private LineRenderer attackVisuals;
    [SerializeField] private float attackVisualsDuration = 0.1f;
    private Vector3 hitPoint;

    [Header("Gloing Visuals")]
    [SerializeField] private MeshRenderer meshRenderer;
    private Material material;
    [Space]
    private float currenIntensity;
    [SerializeField] private float maxIntensity = 150;
    [Space]
    [SerializeField] private Color startColor;
    [SerializeField] private Color endColor;

    [Header("Rotor Visuals")]
    [SerializeField] private Transform rotor;
    [SerializeField] private Transform rotorUnloaded;
    [SerializeField] private Transform rotorLoaded;

    [Header("Front Glow String")]
    [SerializeField] private LineRenderer frontString_L;
    [SerializeField] private LineRenderer frontString_R;
    [Space]
    [SerializeField] private Transform frontStartPoint_L;
    [SerializeField] private Transform frontStartPoint_R;
    [SerializeField] private Transform frontEndPoint_L;
    [SerializeField] private Transform frontEndPoint_R;

    [Header("Back Glow String")]
    [SerializeField] private LineRenderer backString_L;
    [SerializeField] private LineRenderer backString_R;
    [Space]
    [SerializeField] private Transform backStartPoint_L;
    [SerializeField] private Transform backStartPoint_R;
    [SerializeField] private Transform backEndPoint_L;
    [SerializeField] private Transform backEndPoint_R;

    [SerializeField] private LineRenderer[] lineRenderers;


    private void Awake()
    {
        material = new Material(meshRenderer.material);
        meshRenderer.material = material;

        UpdateMaterialOnLineRenderer();
        StartCoroutine(ChangeEmission(1));
    }
    private void Start()
    {
        objectPool = ObjectPoolManager.Instance;
    }

    private void UpdateMaterialOnLineRenderer()
    {
        foreach (var lr in lineRenderers)
        {
            lr.material = material;
        }
    }

    private void Update()
    {
        UpdateEmisionColor();
        UpdateStrings();
        UpdateAttackVisualsIfNeeded();
    }

    public void CreateOnHitFx(Vector3 hitPoint) => objectPool.Get(onHitFx, hitPoint, Random.rotation);

    private void UpdateAttackVisualsIfNeeded()
    {
        if (attackVisuals.enabled && hitPoint != Vector3.zero)
        {
            attackVisuals.SetPosition(1, hitPoint);
        }
    }

    private void UpdateStrings()
    {
        UpdateStringVisuals(frontString_L, frontStartPoint_L, frontEndPoint_L);
        UpdateStringVisuals(frontString_R, frontStartPoint_R, frontEndPoint_R);
        UpdateStringVisuals(backString_L, backStartPoint_L, backEndPoint_L);
        UpdateStringVisuals(backString_R, backStartPoint_R, backEndPoint_R);
    }

    private void UpdateEmisionColor()
    {
        Color emissionColor = Color.Lerp(startColor, endColor, currenIntensity / maxIntensity);

        emissionColor = emissionColor * Mathf.LinearToGammaSpace(currenIntensity);

        material.SetColor("_EmissionColor", emissionColor);
    }

    public void PlayReloadVFX(float duration)
    {
        float newDuration = duration / 2;

        StartCoroutine(ChangeEmission(newDuration));
        StartCoroutine(UpdateRotorPosition(newDuration));

    }

    public void PlayAttackVFX(Vector3 startPoint, Vector3 endPoint)
    {
        StartCoroutine(VFXCoroutine(startPoint, endPoint));
    }

    private IEnumerator VFXCoroutine(Vector3 startPoint, Vector3 endPoint)
    {
        hitPoint = endPoint;

        attackVisuals.enabled = true;
        attackVisuals.SetPosition(0, startPoint);
        attackVisuals.SetPosition(1, endPoint);

        yield return new WaitForSeconds(attackVisualsDuration);
        attackVisuals.enabled = false;
    }

    private IEnumerator ChangeEmission(float duration)
    {
        float startTime = Time.time;
        float startItensity = 0;

        while (Time.time - startTime < duration)
        {
            float tValue = (Time.time - startTime) / duration;
            currenIntensity = Mathf.Lerp(startItensity, maxIntensity, tValue);
            yield return null;
        }

        currenIntensity = maxIntensity;
    }

    private IEnumerator UpdateRotorPosition(float duration)
    {
        float startTime = Time.time;

        while ((Time.time - startTime) < duration)
        {
            float tValue = (Time.time - startTime) / duration;
            rotor.position = Vector3.Lerp(rotorUnloaded.position, rotorLoaded.position, tValue);
            yield return null;
        }

        rotor.position = rotorLoaded.position;
    }

    private void UpdateStringVisuals(LineRenderer lineRenderer, Transform startPoint, Transform endPoint)
    {
        lineRenderer.SetPosition(0, startPoint.position);
        lineRenderer.SetPosition(1, endPoint.position);
    }
}