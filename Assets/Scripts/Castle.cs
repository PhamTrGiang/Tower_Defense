using UnityEngine;

public class Castle : MonoBehaviour
{
    private GameManager gameManager;

    private void OnTriggerEnter(Collider other)
    {
        Enemy enemy = other.GetComponent<Enemy>();
        if (enemy)
        {
            other.GetComponent<Enemy>().RemoveEnemy();

            if (gameManager == null)
                gameManager = FindAnyObjectByType<GameManager>();

            if (gameManager != null)
                gameManager.UpdateHp(-1);
        }
    }
}
