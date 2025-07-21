using UnityEngine;

public class EnemySwarmVisuals : EnemyVisuals
{
    [Header("Visuals Variants")]
    [SerializeField] private GameObject[] vatiants;

    [Header("Bounce Settings")]
    [SerializeField] private AnimationCurve bounceCurve;
    [SerializeField] private float bounceSpeed;
    [SerializeField] private float minHeight;
    [SerializeField] private float maxHeight;
    private float bounceTimer;

    protected override void Awake()
    {
        ChooseVisualVariant();
        base.Awake();
    }

    protected override void Start()
    {
        base.Start();
    }

    protected override void Update()
    {
        base.Update();
        BounceEffect();
    }

    private void BounceEffect()
    {
        bounceTimer += Time.deltaTime * bounceSpeed;

        float bounceValue = bounceCurve.Evaluate(bounceTimer % 1);
        float bounceHight = Mathf.Lerp(minHeight, maxHeight, bounceValue);

        visuals.localPosition = new Vector3(visuals.localPosition.x, bounceHight, visuals.localPosition.z);
    }

    private void ChooseVisualVariant()
    {
        foreach (var option in vatiants)
        {
            option.SetActive(false);
        }

        int randomIndex = Random.Range(0, vatiants.Length);
        GameObject newVisual = vatiants[randomIndex];

        newVisual.SetActive(true);
        visuals = newVisual.transform;
    }
}
