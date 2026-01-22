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
        yield return new WaitForSeconds(this.delay);
        if (target != null && target.IsAlive)
            target.TakeDamage(attacker.atk);
    }
}
