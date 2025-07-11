using Unity.IO.LowLevel.Unsafe;
using Unity.VisualScripting;
using UnityEngine;

public class TowerPreview : MonoBehaviour
{
    private MeshRenderer[] meshRenderers;
    private TowerAttackRadirusDisplay attackRadiusDisplay;
    private Tower myTower;

    private float attackRange;

    private void Awake()
    {
        meshRenderers = GetComponentsInChildren<MeshRenderer>();
        myTower = GetComponent<Tower>();
        attackRadiusDisplay = transform.AddComponent<TowerAttackRadirusDisplay>();
        attackRange = myTower.GetAttackRange();

        MakeAllMeshTransperent();
        DestroyExtraComponents();
    }

    public void ShowPreview(bool showPreview, Vector3 position)
    {
        transform.position = position;
        attackRadiusDisplay.CreateCircle(showPreview,attackRange);
    }

    private void DestroyExtraComponents()
    {
        if (myTower != null)
        {
            CrossbowVisuals crossbowVisuals = GetComponent<CrossbowVisuals>();

            Destroy(crossbowVisuals);
            Destroy(myTower);
        }
    }

    private void MakeAllMeshTransperent()
    {
        Material previewMat = FindFirstObjectByType<BuildManager>().GetBuildPreviewMat();

        foreach (var mesh in meshRenderers)
        {
            mesh.material = previewMat;
        }
    }
}
