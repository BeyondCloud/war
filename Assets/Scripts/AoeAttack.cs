using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AoeAttack : IAttack
{
    private MonoBehaviour coroutineRunner;
    private float delay;
    private float aoeRadius;
    
    public AoeAttack(float delay, float aoeRadius, MonoBehaviour runner)
    {
        this.delay = delay;
        this.aoeRadius = aoeRadius;
        this.coroutineRunner = runner;
    }
    
    public void Attack(Unit attacker, Unit target)
    {
        coroutineRunner.StartCoroutine(AttackRoutine(attacker, target));
    }

    private IEnumerator AttackRoutine(Unit attacker, Unit target)
    {
        float elapsed = 0f;
        while (elapsed < this.delay)
        {
            elapsed += Time.deltaTime;
            yield return null;
        }
        
        // 尋找半徑範圍內的所有敵人
        List<Unit> enemiesInRange = UnitController.Instance.FindEnemiesInRadius(attacker, aoeRadius);
        
        // 對所有範圍內的敵人造成傷害
        foreach (Unit enemy in enemiesInRange)
        {
            if (enemy != null && enemy.IsAlive)
            {
                enemy.TakeDamage(attacker.atk);
            }
        }
    }
}
