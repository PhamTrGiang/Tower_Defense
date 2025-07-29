using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class TowerPreview : MonoBehaviour
{
    private List<System.Type> compToKeep = new List<System.Type>();

    private MeshRenderer[] meshRenderers;
    private RadiusDisplay attackRadiusDisplay;
    private ForwardAttackDisplay forwardDisplay;

    private float attackRange;
    private bool towerAttackForward;

    public void SetUpTowerPrevier(GameObject towerToBuild)
    {
        Tower tower = towerToBuild.GetComponent<Tower>();

        attackRadiusDisplay = transform.AddComponent<RadiusDisplay>();
        meshRenderers = GetComponentsInChildren<MeshRenderer>();
        forwardDisplay = tower.GetComponent<ForwardAttackDisplay>();
        attackRange = tower.GetAttackRange();
        towerAttackForward = tower.towerAttackForward;

        SecureComponents();
        MakeAllMeshTransperent();
        DestroyExtraComponents();

        gameObject.SetActive(false);
    }

    public void ShowPreview(bool showPreview, Vector3 previewPosition)
    {
        transform.position = previewPosition;

        if (towerAttackForward == false)
            attackRadiusDisplay.CreateCircle(showPreview, attackRange);
        else
            forwardDisplay.CreateLine(showPreview, attackRange);

    }

    private void SecureComponents()
    {
        compToKeep.Add(typeof(Transform));
        compToKeep.Add(typeof(TowerPreview));
        compToKeep.Add(typeof(RadiusDisplay));
        compToKeep.Add(typeof(ForwardAttackDisplay));
        compToKeep.Add(typeof(LineRenderer));
    }

    private bool ComponentSecured(Component compToCheck)
    {
        return compToKeep.Contains(compToCheck.GetType());
    }

    private void DestroyExtraComponents()
    {
        Component[] components = GetComponents<Component>();

        foreach (var compToCheck in components)
        {
            if (ComponentSecured(compToCheck) == false)
                Destroy(compToCheck);
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
