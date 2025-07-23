using UnityEngine;

public class SpiderLegReference : MonoBehaviour
{
    [SerializeField] private LayerMask whatIsGround;
    [SerializeField] private float contactPointRadius = .5f;

    public Vector3 ContactPoint()
    {
        if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit hitInfo, Mathf.Infinity, whatIsGround))
            return hitInfo.point;

        return transform.position;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;

        Gizmos.DrawLine(transform.position, ContactPoint());
        Gizmos.DrawWireSphere(ContactPoint(), contactPointRadius);
    }
}
