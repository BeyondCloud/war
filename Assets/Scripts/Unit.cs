/*
https://www.notion.so/MVC-2edf98e4af3c80c5998ee57016ca5401
*/
using UnityEngine;
using UnityEngine.AI;

public class Unit : MonoBehaviour
{
    [Header("Stats")]
    public float maxHp = 100;
    public float hp;
    public float atk = 10;
    public float attackRange = 1.5f;

    [Header("Runtime")]
    public float cooldown;
    public Team team;

    [HideInInspector]
    public Vector3 faceDirection = Vector3.right;

    private NavMeshAgent agent;
    private Unit currentTarget;

    void Awake()
    {
        hp = maxHp;
        agent = GetComponent<NavMeshAgent>();
        // Ensure the agent is configured correctly for 2D/3D hybrid or standard 3D usage
        agent.stoppingDistance = attackRange * 0.8f;
    }
    public void Init(Team team)
    {
        Debug.Log(team.ToString() + " unit initialized.");
        this.team = team;
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

        if (!currentTarget)
        {
            // No target, stop moving
            if (agent.isOnNavMesh) agent.ResetPath();
            return;
        }

        float dist = Vector3.Distance(transform.position, currentTarget.transform.position);

        if (dist <= attackRange)
        {
            // Stop moving to attack
            if (agent.isOnNavMesh) agent.ResetPath();
            TryAttack(currentTarget);
        }
        else
        {
            MoveTowards(currentTarget.transform.position);
        }
    }

    void MoveTowards(Vector3 target)
    {
        if (agent.isOnNavMesh)
        {
            agent.SetDestination(target);

            // Optional: Face direction logic if needed for visuals using agent.velocity
            if (agent.velocity.x != 0)
                faceDirection = new Vector3(Mathf.Sign(agent.velocity.x), 0, 0);
        }
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
