using UnityEngine;
using System.Collections.Generic;
/*
https://www.notion.so/MVC-2edf98e4af3c80c5998ee57016ca5401
*/
public class UnitController : MonoBehaviour
{
    public static UnitController Instance;

    // ==================== 這裡加上 Prefab 屬性 ====================
    [Header("Unit Prefab")]

    public List<Unit> blueUnits = new();
    public List<Unit> redUnits = new();
    public GameObject dinoPrefab;
    public GameObject goblinPrefab;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        // 範例生成
        Spawn(Team.Blue, dinoPrefab, 10, new Vector3(-10, 0));
        Spawn(Team.Red, goblinPrefab, 10, new Vector3(10, 0, 0));
    }

    void Update()
    {
        foreach (var u in blueUnits)
            u.Tick();

        foreach (var u in redUnits)
            u.Tick();
    }

    float zWeight = 1.0f; // Adjust this value to control Z-axis penalty

    public Unit FindNearestEnemy(Unit self)
    {
        var enemies = self.team == Team.Blue ? redUnits : blueUnits;

        Unit best = null;
        float minDist = float.MaxValue;

        foreach (var e in enemies)
        {
            if (!e || !e.IsAlive) continue;

            // Calculate the distance with a penalty on the Z-axis
            float d = Vector3.Distance(self.transform.position, e.transform.position);
            float zPenalty = Mathf.Abs(self.transform.position.z - e.transform.position.z) * zWeight; // zWeight is a factor you can define
            float totalDist = d + zPenalty;

            if (totalDist < minDist)
            {
                minDist = totalDist;
                best = e;
            }
        }
        return best;
    }
    public Unit FindLowHpEnemy(Unit self)
    {
        var enemies = self.team == Team.Blue ? redUnits : blueUnits;

        Unit best = null;
        float minHp = float.MaxValue;

        foreach (var e in enemies)
        {
            if (!e || !e.IsAlive) continue;

            if (e.hp < minHp)
            {
                minHp = e.hp;
                best = e;
            }
        }
        return best;
    }
    // Removed HandleSeparation logic since NavMeshAgents are now used.

    public void OnUnitDeath(Unit unit)
    {
        blueUnits.Remove(unit);
        redUnits.Remove(unit);
    }

    void Spawn(Team team, GameObject unitPrefab, int count, Vector3 center)
    {

        for (int i = 0; i < count; i++)
        {
            Vector3 pos = center + Random.insideUnitSphere * 5f;
            pos.y = 0;

            // Keep prefab's authored rotation (Quaternion.identity would zero it out)
            var go = Instantiate(unitPrefab, pos, Quaternion.identity);
            go.GetComponent<Unit>().Init(team);
            var u = go.GetComponent<Unit>();
            if (team == Team.Blue)
                blueUnits.Add(u);
            else
            {
                redUnits.Add(u);
            }
        }
    }

}
