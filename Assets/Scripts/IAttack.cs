using UnityEngine;

public interface IAttack
{
    void Attack(Unit attacker, Unit target, float delay_sec = 0f);
}
