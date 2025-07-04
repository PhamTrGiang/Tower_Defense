using UnityEngine;

public class Castle : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        Enemy enemy = other.GetComponent<Enemy>();
        if (enemy)
        {
            other.GetComponent<Enemy>().TakeDame(9999);
        }
    }
}
