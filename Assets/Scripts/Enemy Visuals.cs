using UnityEngine;

public class EnemyVisuals : MonoBehaviour
{
    [SerializeField] private Transform visuals;
    [SerializeField] private LayerMask whatIsGround;
    [SerializeField] private float verticalRotationSpeed;

    private void Update()
    {
        AlighWithSlope();
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
