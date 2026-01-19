/*
https://www.notion.so/MVC-2edf98e4af3c80c5998ee57016ca5401
*/
using UnityEngine;
using UnityEngine.AI;
// [RequireComponent(typeof(NavMeshAgent))]
// [RequireComponent(typeof(LineRenderer))]

public class Unit : MonoBehaviour
{
    [Header("Stats")]
    public float maxHp = 100;
    public float hp;
    public float atk = 10;
    public float attackRange = 0.5f;

    [Header("Runtime")]
    public float cooldown;
    public Team team;
    
    [HideInInspector]
    public Vector3 faceDirection = Vector3.right;
    [HideInInspector]
    public Unit currentTarget;
    
    private NavMeshAgent agent;
    private LineRenderer lineRenderer;
    [SerializeField]
    private Animator unitAnimator;
    void Awake()
    {
        hp = maxHp;
        agent = GetComponent<NavMeshAgent>();
        // Ensure the agent is configured correctly for 2D/3D hybrid or standard 3D usage
        agent.stoppingDistance = attackRange * 0.8f;
        agent.updateRotation = false; // We handle rotation manually
        
    }
    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.startWidth = 0.15f;
        lineRenderer.endWidth = 0.15f;
        lineRenderer.positionCount = 0;
    }
    void DrawPath()
    {
        if(team == Team.Blue)
            lineRenderer.material.color = Color.blue;
        else
            lineRenderer.material.color = Color.red;
        lineRenderer.positionCount = agent.path.corners.Length;
        lineRenderer.SetPosition(0,transform.position);
        if (agent.path.corners.Length < 2 )
        {
            return;
        }
        for (int i = 1; i < agent.path.corners.Length; i++)
        {
            Vector3 pointPosition = new Vector3(agent.path.corners[i].x, agent.path.corners[i].y, agent.path.corners[i].z);
            lineRenderer.SetPosition(i, pointPosition);
        }
    }
    public void Init(Team team)
    {
        this.team = team;
    }
    public bool IsAlive => hp > 0;

    public void TakeDamage(float amount)
    {
        unitAnimator.SetTrigger("hurt");
        hp -= amount;
        if (hp <= 0)
            Die();
    }

    void Die()
    {
        UnitController.Instance.OnUnitDeath(this);
        Destroy(gameObject);
    }
    private Unit AssignTarget()
    {
        return UnitController.Instance.FindNearestEnemy(this);
    }

    float GetPathLength(NavMeshPath path)
    {
        float length = 0.0f;
        if (path.corners.Length < 2) return length;

        for (int i = 0; i < path.corners.Length - 1; i++)
        {
            length += Vector3.Distance(path.corners[i], path.corners[i + 1]);
        }
        return length;
    }
    // 🔹 每一幀由 Manager 呼叫（不是自己 Update）
    public void Tick()
    {
        unitAnimator.SetFloat("speed", agent.velocity.magnitude);
        if (agent.hasPath)
        {
            DrawPath();
        }
        if (!IsAlive) return;
        
        if (cooldown > 0)
            cooldown -= Time.deltaTime;
        currentTarget = AssignTarget();
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
