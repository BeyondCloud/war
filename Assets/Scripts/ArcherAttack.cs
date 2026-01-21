using UnityEngine;
using System.Collections;

public class ArcherAttack : IAttack
{
    private float attackFlightSpeed;
    private MonoBehaviour coroutineRunner;
    private float arcOffset; // Z軸偏移高度

    public ArcherAttack(float flightTime, MonoBehaviour runner, float arcOffset)
    {
        this.attackFlightSpeed = flightTime;
        this.coroutineRunner = runner;
        this.arcOffset = arcOffset;
    }

    public void Attack(Unit attacker, Unit target, float delay_sec = 0f)
    {
        coroutineRunner.StartCoroutine(ProjectileRoutine(attacker, target));
    }

    private IEnumerator ProjectileRoutine(Unit attacker, Unit target)
    {
        GameObject bullet = new GameObject("Bullet");
        // set bullet scale
        bullet.transform.localScale = new Vector3(5.0f, 10.0f, 5.0f);
        bullet.transform.position = attacker.transform.position;
        
        SpriteRenderer sr = bullet.AddComponent<SpriteRenderer>();
        sr.sprite = attacker.bulletSprite;
        sr.sortingOrder = 10;
        
        Vector3 startPos = attacker.transform.position;
        Vector3 targetPos = target.transform.position;
        float distance = Vector3.Distance(startPos, targetPos);
        float duration = distance / attackFlightSpeed;
        float elapsed = 0f;
        
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            if (target == null)
            {
                yield return null;
            }
            targetPos = target.transform.position;
            Vector3 basePos = Vector3.Lerp(startPos, targetPos, t);
            
            float arcHeight = arcOffset * Mathf.Sin(Mathf.PI * t);
            Vector3 currentPos = new Vector3(basePos.x, basePos.y, basePos.z + arcHeight);
            bullet.transform.position = currentPos;
            
            // Compute rotation to face movement direction
            float half_x = (targetPos.x - startPos.x) / 2;
            float angle_start = Mathf.Atan2(arcOffset, half_x) * Mathf.Rad2Deg - 90;
            float angle_end = Mathf.Atan2(-arcOffset, half_x) * Mathf.Rad2Deg - 90;
            float angle_current = Mathf.Lerp(angle_start, angle_end, t);
            bullet.transform.rotation = Quaternion.Euler(90f, 0f, angle_current);
            yield return null;
        }
        if (target != null && target.IsAlive)
        {
            target.TakeDamage(attacker.atk);
        }
        Object.Destroy(bullet);
    }
}
