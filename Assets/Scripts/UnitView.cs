using UnityEngine;
using UnityEngine.UI;

public class UnitView : MonoBehaviour
{
    public Image hpFill;
    public Image unitImg;
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