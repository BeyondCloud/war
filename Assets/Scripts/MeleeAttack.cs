using UnityEngine;

public class MeleeAttack : IAttack
{
    public void Attack(Unit attacker, Unit target, float delay_sec = 0f)
    {
        target.TakeDamage(attacker.atk);
    }
}
