/*
https://www.notion.so/MVC-2edf98e4af3c80c5998ee57016ca5401
*/
using UnityEngine;

public class Unit : MonoBehaviour
{
    [Header("Stats")]
    public float maxHp = 100;
    public float hp;
    public float atk = 10;
    public float moveSpeed = 4f;
    public float attackRange = 1.5f;
    public float personalRadius = 0.5f;

    [Header("Runtime")]
    public float cooldown;
    public Team team;

    [HideInInspector]
    public Vector3 faceDirection = Vector3.right;
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
        UnitController.Instance.OnUnitDeath(this);
        Destroy(gameObject);
    }

    // 🔹 每一幀由 Manager 呼叫（不是自己 Update）
    public void Tick()
    {
        if (!IsAlive) return;

        if (cooldown > 0)
            cooldown -= Time.deltaTime;

        // 每幀重新找目標
        currentTarget = UnitController.Instance.FindNearestEnemy(this);

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
        //Flip transform according to movement direction
        if (dir.x != 0)
            faceDirection = new Vector3(Mathf.Sign(dir.x), 0, 0);
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
