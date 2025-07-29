using UnityEngine;

public class TowerFanRevealArea : MonoBehaviour
{
    private TowerFan tower;

    private void Awake()
    {
        tower = GetComponentInParent<TowerFan>();
    }

    private void OnTriggerEnter(Collider other)
    {
        Enemy enemy = other.GetComponent<Enemy>();

        if (enemy != null)
            tower.AddEnemyToReveal(enemy);
    }

    void OnTriggerExit(Collider other)
    {
        Enemy enemy = other.GetComponent<Enemy>();

        if (enemy != null)
            tower.RemoveEnemyToReveal(enemy);
    }
}
