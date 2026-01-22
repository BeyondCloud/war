using System.Linq;
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

    // public List<Unit> blueUnits = new();
    // public List<Unit> redUnits = new();
    public List<Unit> blueUnits;
    public List<Unit> redUnits;
    
    [Header("Random Seed")]
    [SerializeField] private int randomSeed = 12345;
    [SerializeField] private bool useFixedSeed = true;
    [SerializeField] private Transform blueUnitsRoot;
    [SerializeField] private Transform redUnitsRoot;
    public GameObject dinoPrefab;
    public GameObject goblinPrefab;

    void Awake()
    {
        Instance = this;
        
        // 設置隨機種子以確保結果一致
        if (useFixedSeed)
        {
            Random.InitState(randomSeed);
            Debug.Log($"Ramdom Seed: {randomSeed}");
        }
        
        blueUnits = blueUnitsRoot.GetComponentsInChildren<Unit>().ToList();
        redUnits = redUnitsRoot.GetComponentsInChildren<Unit>().ToList();
        //set team
        foreach(var u in blueUnits)
            u.team = Team.Blue;

        foreach(var u in redUnits)
            u.team = Team.Red;
    }

    void Start()
    {
        // 範例生成
        // Spawn(Team.Blue, dinoPrefab, 10, new Vector3(-10, 0));
        // Spawn(Team.Red, goblinPrefab, 10, new Vector3(10, 0, 0));
    }

    void Update()
    {
        for (int i = blueUnits.Count - 1; i >= 0; i--)
        {
            if (i < blueUnits.Count)
                blueUnits[i].Tick();
        }

        for (int i = redUnits.Count - 1; i >= 0; i--)
        {
            if (i < redUnits.Count)
                redUnits[i].Tick();
        }
    }

    float zWeight = 1.0f; // Adjust this value to control Z-axis penalty

    public Unit FindNearestEnemy(Unit self)
    {
        var enemies = self.team == Team.Blue ? redUnits : blueUnits;

        Unit best = null;
        float minDist = float.MaxValue;

        foreach (var e in enemies)
        {

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
    
    public List<Unit> FindEnemiesInRadius(Unit self, float radius)
    {
        var enemies = self.team == Team.Blue ? redUnits : blueUnits;
        List<Unit> enemiesInRange = new List<Unit>();

        foreach (var e in enemies)
        {
            if (!e || !e.IsAlive) continue;

            float distance = Vector3.Distance(self.transform.position, e.transform.position);
            if (distance <= radius)
            {
                enemiesInRange.Add(e);
            }
        }
        return enemiesInRange;
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
