using UnityEngine;
using UnityEngine.UI;
/*
https://www.notion.so/MVC-2edf98e4af3c80c5998ee57016ca5401
*/
public class UnitView : MonoBehaviour
{
    public Image hpFill;
    public Image unitImg;
    public Canvas canvas;
    private Unit unit;
    private Color originalHpColor;

    void Awake()
    {
        unit = GetComponentInParent<Unit>();
        if (hpFill != null)
            originalHpColor = hpFill.color;
        if (unit == null)
        {
            Debug.LogError("Failed to retrieve Unit component.");
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
        if (unitImg)
        {
            canvas.transform.rotation = Quaternion.Euler(90, 0, 0);
        }
    }
    
    void updateHpBar()
    {
        if (unit && hpFill)
        {
            float ratio = unit.maxHp > 0 ? Mathf.Clamp01(unit.hp / unit.maxHp) : 0f;
            hpFill.fillAmount = ratio;
            if (ratio < 0.3f)
                hpFill.color = Color.red;
            else if (ratio < 0.7f)
                hpFill.color = Color.yellow;
            else
                hpFill.color = originalHpColor;
        }
    }
    void updateFacingDirection()
    {
        if (unitImg && unit)
        {
            var scale = unitImg.transform.localScale;
            scale.x = Mathf.Abs(scale.x) * unit.faceDirection.x;
            unitImg.transform.localScale = scale;
        }
    }
}