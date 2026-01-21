/*
https://www.notion.so/MVC-2edf98e4af3c80c5998ee57016ca5401
*/
using UnityEngine;
using UnityEngine.AI;
// [RequireComponent(typeof(NavMeshAgent))]
// [RequireComponent(typeof(LineRenderer))]

public enum AttackType
{
    Melee,
    Archer
}

public class Unit : MonoBehaviour
{
    [Header("Stats")]
    public float maxHp = 100;
    public float hp;
    public float atk = 10;
    public float attackRange = 0.5f;
    public float cooldown = 1.0f;

    [Header("Attack Settings")]
    public AttackType attackType = AttackType.Melee;
    [ConditionalHide("attackType", AttackType.Archer)]
    public float attackFlightSpeed = 10.0f; // 只有Archer類型時使用
    [ConditionalHide("attackType", AttackType.Archer)]
    public Sprite bulletSprite; // 子彈的樣式
    [ConditionalHide("attackType", AttackType.Archer)]
    public float arcOffset = 5.0f; // 弓箭飛行的Z軸偏移高度（0為直線飛行）

    [Header("Runtime")]

    private float cooldownTimer = 0f;
    [HideInInspector] public Team team;
    [HideInInspector] public Vector3 faceDirection = Vector3.right;
    [HideInInspector] public Unit currentTarget;
    
    private NavMeshAgent agent;
    private LineRenderer lineRenderer;
    [SerializeField] private Animator unitAnimator;
    private IAttack attackStrategy;
    
    private Vector3 offset = new Vector3(0, 0, 0.5f);
    void Awake()
    {
        hp = maxHp;
        agent = GetComponent<NavMeshAgent>();
        
        // 根據選擇的攻擊類型初始化攻擊策略
        switch (attackType)
        {
            case AttackType.Melee:
                attackStrategy = new MeleeAttack(0.1f, this);
                break;
            case AttackType.Archer:
                attackStrategy = new ArcherAttack(attackFlightSpeed, this, arcOffset);
                break;
        }
        // Ensure the agent is configured correctly for 2D/3D hybrid or standard 3D usage
        agent.stoppingDistance = attackRange * 0.8f;
        agent.updateRotation = false; // We handle rotation manually
        
        // 設置 NavMeshAgent 為確定性模式
        agent.avoidancePriority = 50; // 統一優先級, 沒有的話會有隨機性
        // agent.obstacleAvoidanceType = ObstacleAvoidanceType.NoObstacleAvoidance;
        
    }
    public float GetDistanceToTarget(Unit target)
    {
        return Vector3.Distance(transform.position, target.transform.position);
    }
    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.startWidth = 0.15f;
        lineRenderer.endWidth = 0.15f;
        lineRenderer.positionCount = 0;
        lineRenderer.SetWidth(0.2f, 0.01f);
    }
    void DrawPath()
    {
        if(team == Team.Blue)
        {
            lineRenderer.startColor = Color.blue;
            lineRenderer.endColor = Color.blue;
        }
        else
        {
            lineRenderer.startColor = Color.red;
            lineRenderer.endColor = Color.red;
        }
        
        if (currentTarget)
        {
            lineRenderer.positionCount = 2;
            if (team == Team.Red)
            {
                lineRenderer.SetPosition(0, transform.position + offset);
                lineRenderer.SetPosition(1, currentTarget.transform.position+ offset);
            }
            else
            {
                lineRenderer.SetPosition(0, transform.position);
                lineRenderer.SetPosition(1, currentTarget.transform.position);
            }
        }
        else
        {
            lineRenderer.positionCount = 0;
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

    // 🔹 每一幀由 Manager 呼叫（不是自己 Update）
    public void Tick()
    {
        unitAnimator.SetFloat("speed", agent.velocity.magnitude);
        DrawPath();
        if (!IsAlive) return;
        
        if (cooldownTimer > 0)
            cooldownTimer -= Time.deltaTime;
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
        if (cooldownTimer > 0) return;
        var dist = Vector3.Distance(transform.position, target.transform.position);
        attackStrategy.Attack(this, target);
        cooldownTimer = cooldown;
    }
}

public enum Team
{
    Blue,
    Red
}
