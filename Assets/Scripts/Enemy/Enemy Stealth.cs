using System.Collections.Generic;
using UnityEngine;

public class EnemyStealth : Enemy
{
    [Header("Stealth Enemy Details")]
    [SerializeField] private List<Enemy> enemiesToHide;
    [SerializeField] private float hideDuration = .5f;
    [SerializeField] private ParticleSystem smokeFx;

    protected override void Awake()
    {
        base.Awake();

        InvokeRepeating(nameof(HideItSeft), .1f, hideDuration);
        InvokeRepeating(nameof(HideEnemies), .1f, hideDuration);
    }

    private void HideItSeft() => HideEnemy(hideDuration);

    private void HideEnemies()
    {
        foreach (Enemy enemy in enemiesToHide)
        {
            enemy.HideEnemy(hideDuration);
        }
    }

    public List<Enemy> GetEnemiesToHide() => enemiesToHide;
    public void EnableSmoke(bool enable)
    {
        if (enable)
        {
            if (smokeFx.isPlaying == false)
                smokeFx.Play();
            else
                smokeFx.Stop();
        }
    }
}
