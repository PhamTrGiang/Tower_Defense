using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class WaypointManager : MonoBehaviour
{
    [SerializeField] List<Transform> waypoints;

    public static WaypointManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    public Transform[] GetWaypoints() => waypoints.ToArray();

    private void OnDrawGizmosSelected()
    {
        waypoints.Clear();
        for (int i = 0; i < this.transform.childCount; i++)
        {
            waypoints.Add(this.transform.GetChild(i));
        }
    }
}
