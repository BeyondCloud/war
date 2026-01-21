using UnityEngine;

public class MeleeAttack : IAttack
{
    public void Attack(Unit attacker, Unit target)
    {
        target.TakeDamage(attacker.atk);
    }
}
