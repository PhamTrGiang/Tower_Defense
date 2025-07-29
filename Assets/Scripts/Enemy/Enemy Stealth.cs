using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyStealth : Enemy
{
    [Header("Stealth Enemy Details")]
    [SerializeField] private List<Enemy> enemiesToHide;
    [SerializeField] private float hideDuration = .5f;
    [SerializeField] private ParticleSystem smokeFx;
    private bool canHideEnemy = true;

    protected override void Awake()
    {
        base.Awake();

        InvokeRepeating(nameof(HideItSeft), .1f, hideDuration);
        InvokeRepeating(nameof(HideEnemies), .1f, hideDuration);
    }

    private void HideItSeft() => HideEnemy(hideDuration);

    private void HideEnemies()
    {
        if (canHideEnemy == false)
            return;

        foreach (Enemy enemy in enemiesToHide)
        {
            enemy.HideEnemy(hideDuration);
        }
    }

    public List<Enemy> GetEnemiesToHide() => enemiesToHide;
    public void EnableSmoke(bool enable)
    {
        if (smokeFx.isPlaying == false && enable)
            smokeFx.Play();
        else if (smokeFx.isPlaying == true && !enable)
            smokeFx.Stop();
    }

    protected override IEnumerator DisableHideCo(float duration)
    {
        EnableSmoke(false);
        canBeHidden = false;
        canHideEnemy = false;

        yield return new WaitForSeconds(duration);

        EnableSmoke(true);
        canBeHidden = true;
        canHideEnemy = true;
    }
}
