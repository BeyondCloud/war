using UnityEngine;
using System.Collections.Generic;

public class BattleManager : MonoBehaviour
{
    public static BattleManager Instance;

    // ==================== 這裡加上 Prefab 屬性 ====================
    [Header("Unit Prefab")]
    public GameObject unitPrefab;   // 拖你的 Unit Prefab 進來

    [Header("Team Colors")]
    public Color blueColor = Color.blue;
    public Color redColor = Color.red;

    public List<Unit> blueUnits = new();
    public List<Unit> redUnits = new();

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        // 範例生成
        Spawn(Team.Blue, 5, new Vector3(-5, 0));
        Spawn(Team.Red, 4, new Vector3(5, 0, 0));
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
        foreach (var a in units)
            foreach (var b in units)
            {
                if (a == b) continue;

                Vector3 diff = a.transform.position - b.transform.position;
                float dist = diff.magnitude;
                float minDist = a.personalRadius + b.personalRadius;

                if (dist > 0 && dist < minDist)
                {
                    a.transform.position += diff.normalized * (minDist - dist) * 0.5f;
                }
            }
    }

    public void OnUnitDeath(Unit unit)
    {
        blueUnits.Remove(unit);
        redUnits.Remove(unit);
    }

    // ==================== Spawn 函數 ====================
    void Spawn(Team team, int count, Vector3 center)
    {

        for (int i = 0; i < count; i++)
        {
            Vector3 pos = center + Random.insideUnitSphere;
            pos.y = 0;

            // Keep prefab's authored rotation (Quaternion.identity would zero it out)
            var go = Instantiate(unitPrefab, pos, unitPrefab.transform.rotation);
            var u = go.GetComponent<Unit>();
            u.team = team;
            if (team == Team.Blue)
                blueUnits.Add(u);
            else
            {
                u.transform.localScale = new Vector3(-1, 1, 1); // Flip for red team
                redUnits.Add(u);
            }
        }
    }

}
