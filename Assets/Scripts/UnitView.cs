using UnityEngine;
using UnityEngine.UI;
/*
https://www.notion.so/MVC-2edf98e4af3c80c5998ee57016ca5401
*/
public class UnitView : MonoBehaviour
{
    public Image hpFill;
    public Canvas canvas;
    private Unit unit;
    

    // This get called after unit is initialized
    void Start()
    {
        unit = GetComponent<Unit>();
        if(unit.team == Team.Blue)
        {
            hpFill.color = Color.green;
        }
        else
        {
            hpFill.color = Color.red;
        }
    }

    void Update()
    {
        updateHpBar();
        updateFacingDirection();
   }

    void LateUpdate()
    {
        // 鎖定圖片旋轉，使其面向 Y 軸 (平躺)，且不隨 Agent 旋轉
        canvas.transform.rotation = Quaternion.Euler(90, 0, 0);
        // 鎖定Unit y 方向旋轉
    }
    
    void updateHpBar()
    {
        if (unit && hpFill)
        {
            float ratio = unit.maxHp > 0 ? Mathf.Clamp01(unit.hp / unit.maxHp) : 0f;
            hpFill.fillAmount = ratio;
        }
    }
    void updateFacingDirection()
    {
        if (unit)
        {
            var scale = unit.transform.localScale;
            scale.x = Mathf.Abs(scale.x) * unit.faceDirection.x;
            unit.transform.localScale = scale;
        }
    }
}