using UnityEngine;
using System.Collections.Generic;
/*
管理所有單位生命週期 (生成 死亡 清單 移動方式)
*/
public class BattleManager : MonoBehaviour
{
    public static BattleManager Instance;

    // ==================== 這裡加上 Prefab 屬性 ====================
    [Header("Unit Prefab")]

    public List<Unit> blueUnits = new();
    public List<Unit> redUnits = new();
    public GameObject dinoPrefab;
    public GameObject goblinPrefab;

    // Spatial Hashing
    private Dictionary<Vector2Int, List<Unit>> grid = new Dictionary<Vector2Int, List<Unit>>();
    private float gridCellSize = 2f; 

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        // 範例生成
        Spawn(Team.Blue, dinoPrefab, 20, new Vector3(-5, 0));
        Spawn(Team.Red, goblinPrefab, 20, new Vector3(5, 0, 0));
    }

    void Update()
    {
        foreach (var u in blueUnits)
            u.Tick();

        foreach (var u in redUnits)
            u.Tick();

        HandleSeparation(blueUnits);
        HandleSeparation(redUnits);
    }

    public Unit FindNearestEnemy(Unit self)
    {
        var enemies = self.team == Team.Blue ? redUnits : blueUnits;

        Unit best = null;
        float minDist = float.MaxValue;

        foreach (var e in enemies)
        {
            if (!e || !e.IsAlive) continue;

            float d = Vector3.Distance(self.transform.position, e.transform.position);
            if (d < minDist)
            {
                minDist = d;
                best = e;
            }
        }
        return best;
    }

    void HandleSeparation(List<Unit> units)
    {
        // 1. Clear and Build Grid
        grid.Clear();
        
        foreach (var u in units)
        {
            if (!u || !u.IsAlive) continue;
            
            Vector2Int key = new Vector2Int(
                Mathf.FloorToInt(u.transform.position.x / gridCellSize),
                Mathf.FloorToInt(u.transform.position.z / gridCellSize)
            );

            if (!grid.ContainsKey(key))
            {
                grid[key] = new List<Unit>();
            }
            grid[key].Add(u);
        }

        // 2. Optimized Collision Check
        foreach (var a in units)
        {
            if (!a || !a.IsAlive) continue;

            Vector2Int key = new Vector2Int(
                Mathf.FloorToInt(a.transform.position.x / gridCellSize),
                Mathf.FloorToInt(a.transform.position.z / gridCellSize)
            );

            // Check neighbors (3x3 area)
            for (int x = -1; x <= 1; x++)
            {
                for (int y = -1; y <= 1; y++)
                {
                    Vector2Int neighborKey = key + new Vector2Int(x, y);
                    if (grid.TryGetValue(neighborKey, out var neighbors))
                    {
                        foreach (var b in neighbors)
                        {
                            if (a == b) continue;

                            Vector3 diff = a.transform.position - b.transform.position;
                            float sqrDist = diff.sqrMagnitude;
                            float minDist = a.personalRadius + b.personalRadius;

                            // Using squared distance check to avoid Sqrt when not needed
                            if (sqrDist > 0 && sqrDist < minDist * minDist)
                            {
                                float dist = Mathf.Sqrt(sqrDist);
                                a.transform.position += diff / dist * (minDist - dist) * 0.5f;
                            }
                        }
                    }
                }
            }
        }
    }

    public void OnUnitDeath(Unit unit)
    {
        blueUnits.Remove(unit);
        redUnits.Remove(unit);
    }

    void Spawn(Team team, GameObject unitPrefab, int count, Vector3 center)
    {

        for (int i = 0; i < count; i++)
        {
            Vector3 pos = center + Random.insideUnitSphere*5f;
            pos.y = 0;

            // Keep prefab's authored rotation (Quaternion.identity would zero it out)
            var go = Instantiate(unitPrefab, pos, unitPrefab.transform.rotation);
            var u = go.GetComponent<Unit>();
            u.team = team;
            if (team == Team.Blue)
                blueUnits.Add(u);
            else
            {
                redUnits.Add(u);
            }
        }
    }

}
