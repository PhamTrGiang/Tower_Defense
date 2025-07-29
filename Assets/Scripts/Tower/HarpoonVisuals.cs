using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class HarpoonVisuals : MonoBehaviour
{
    private ObjectPoolManager objectPool;

    [SerializeField] private Transform startPoint;
    [SerializeField] private Transform endPoint;
    [Space]
    [SerializeField] private GameObject linkPrefab;
    [SerializeField] private Transform linkParent;
    [SerializeField] private float linkDistance = .2f;
    [SerializeField] private int maxLinks = 100;

    private List<ProjectileHarpoonLink> links = new List<ProjectileHarpoonLink>();

    [Space]
    [SerializeField] private GameObject onElectrifyVfx;
    [SerializeField] private Vector3 vfxOffset;
    private GameObject currentVfx;

    private void Start()
    {
        InitializeLinks();
        objectPool = ObjectPoolManager.Instance;
    }

    private void Update()
    {
        if (endPoint == null)
            return;

        ActivateIfNeeded();
    }

    public void CreateElectrifyVfx(Transform targetTransform)
    {
        currentVfx = objectPool.Get(onElectrifyVfx, targetTransform.position + vfxOffset, Quaternion.identity, targetTransform);

    }

    private void DestroyElectrifyVfx()
    {
        if (currentVfx != null)
            objectPool.Remove(currentVfx);
    }

    public void EnableChainVisuals(bool enable, Transform newEndPoint = null)
    {
        if (enable)
        {
            endPoint = newEndPoint;
        }

        if (!enable)
        {
            endPoint = startPoint;

            DestroyElectrifyVfx();
        }
    }

    private void ActivateIfNeeded()
    {
        Vector3 direction = (endPoint.position - startPoint.position).normalized;
        float distance = Vector3.Distance(startPoint.position, endPoint.position);
        int activeLinksAmount = Mathf.Min(maxLinks, Mathf.CeilToInt(distance / linkDistance));

        for (int i = 0; i < links.Count; i++)
        {
            if (i < activeLinksAmount)
            {
                Vector3 newPosition = startPoint.position + direction * linkDistance * (i + 1);
                links[i].EnableLink(true, newPosition);
            }
            else
                links[i].EnableLink(false, Vector3.zero);

            if (i != links.Count - 1)
                links[i].UpdateLineRenderer(links[i], links[i + 1]);
        }
    }

    private void InitializeLinks()
    {
        for (int i = 0; i < maxLinks; i++)
        {
            ProjectileHarpoonLink newLink = Instantiate(linkPrefab, startPoint.position, Quaternion.identity, linkParent).GetComponent<ProjectileHarpoonLink>();
            links.Add(newLink);
        }
    }
}
