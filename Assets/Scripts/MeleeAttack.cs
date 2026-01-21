using UnityEngine;
using System.Collections;

public class MeleeAttack : IAttack
{
    private MonoBehaviour coroutineRunner;
    private float delay;
    public MeleeAttack(float delay, MonoBehaviour runner)
    {
        this.delay = delay; // attack delay in seconds, this make the fight fairer
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
        if (target != null && target.IsAlive)
            target.TakeDamage(attacker.atk);
    }
}
