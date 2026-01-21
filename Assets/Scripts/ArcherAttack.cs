using UnityEngine;
using System.Collections;

public class ArcherAttack : IAttack
{
    private float attackFlightSpeed;
    private MonoBehaviour coroutineRunner;

    public ArcherAttack(float flightTime, MonoBehaviour runner)
    {
        this.attackFlightSpeed = flightTime;
        this.coroutineRunner = runner;
    }

    public void Attack(Unit attacker, Unit target, float delay_sec = 0f)
    {
        // 启动协程来延迟伤害
        coroutineRunner.StartCoroutine(ProjectileRoutine(attacker, target, delay_sec));
    }

    private IEnumerator ProjectileRoutine(Unit attacker, Unit target, float delay_sec)
    {
        yield return new WaitForSeconds(delay_sec);
        
        // 检查目标是否还存活
        if (target != null)
        {
            target.TakeDamage(attacker.atk);
        }
    }
}
