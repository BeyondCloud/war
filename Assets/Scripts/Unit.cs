using UnityEngine;

public class Unit : MonoBehaviour
{
    [HideInInspector]
    public float maxHp = 100;
    public float hp;
    public float atk = 10;
    public float moveSpeed = 4f;
    public float attackRange = 1.5f;
    public float personalRadius = 0.5f;

    [Header("Runtime")]
    public float cooldown;
    public Team team;

    Unit currentTarget;

    void Awake()
    {
        hp = maxHp;
    }

    public bool IsAlive => hp > 0;

    public void TakeDamage(float amount)
    {
        hp -= amount;
        if (hp <= 0)
            Die();
    }

    void Die()
    {
        BattleManager.Instance.OnUnitDeath(this);
        Destroy(gameObject);
    }

    // 🔹 每一幀由 Manager 呼叫（不是自己 Update）
    public void Tick()
    {
        if (!IsAlive) return;

        if (cooldown > 0)
            cooldown -= Time.deltaTime;

        // 每幀重新找目標
        currentTarget = BattleManager.Instance.FindNearestEnemy(this);

        if (!currentTarget) return;

        float dist = Vector3.Distance(transform.position, currentTarget.transform.position);

        if (dist <= attackRange)
            TryAttack(currentTarget);
        else
            MoveTowards(currentTarget.transform.position);
    }

    void MoveTowards(Vector3 target)
    {
        Vector3 dir = (target - transform.position).normalized;
        transform.position += dir * moveSpeed * Time.deltaTime;
    }

    void TryAttack(Unit target)
    {
        if (cooldown > 0) return;

        target.TakeDamage(atk);
        cooldown = 1.0f; // 秒
    }
}

public enum Team
{
    Blue,
    Red
}
