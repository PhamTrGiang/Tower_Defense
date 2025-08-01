using System.Collections.Generic;
using UnityEngine;

public class EnemyVisuals : MonoBehaviour
{
    private ObjectPoolManager objectPool;

    [SerializeField] private GameObject onDeadFx;
    [SerializeField] private float onDeadFxScale = .5f;
    [Space]
    [SerializeField] protected Transform visuals;
    [SerializeField] private LayerMask whatIsGround;
    [SerializeField] private float verticalRotationSpeed;

    [Header("Transperent Details")]
    [SerializeField] private Material transperentMat;
    private List<Material> originalMat;
    private MeshRenderer[] myRenderers;

    protected virtual void Awake()
    {
        CollectDefaultMaterial();
    }

    protected virtual void Start()
    {
        objectPool = ObjectPoolManager.Instance;
    }

    protected virtual void Update()
    {
        AlighWithSlope();

        if (Input.GetKeyDown(KeyCode.X))
            MakeTransperent(true);

        if (Input.GetKeyDown(KeyCode.C))
            MakeTransperent(false);
    }

    public void CreateOnDeadFx()
    {
        GameObject newDeadVFX = objectPool.Get(onDeadFx, transform.position + new Vector3(0, .15f), Quaternion.identity);
        newDeadVFX.transform.localScale = new Vector3(onDeadFxScale, onDeadFxScale, onDeadFxScale);
    }

    public void MakeTransperent(bool transperent)
    {
        for (int i = 0; i < myRenderers.Length; i++)
        {
            Material materialToApply = transperent ? transperentMat : originalMat[i]; ;
            myRenderers[i].material = materialToApply;
        }
    }

    protected void CollectDefaultMaterial()
    {
        myRenderers = GetComponentsInChildren<MeshRenderer>();
        originalMat = new List<Material>();

        foreach (var renderer in myRenderers)
        {
            originalMat.Add(renderer.material);
        }
    }

    private void AlighWithSlope()
    {
        if (visuals == null) return;

        if (Physics.Raycast(visuals.position, Vector3.down, out RaycastHit hit, Mathf.Infinity, whatIsGround))
        {
            Quaternion targetRotation = Quaternion.FromToRotation(transform.up, hit.normal) * transform.rotation;
            visuals.rotation = Quaternion.Slerp(visuals.rotation, targetRotation, Time.deltaTime * verticalRotationSpeed);
        }
    }
}
