using UnityEngine;
using UnityEngine.UI;

public class UnitView : MonoBehaviour
{
    public Image hpFill;
    private Unit unit;
    private Color originalHpColor;

    void Awake()
    {
        unit = GetComponentInParent<Unit>();
        if (hpFill != null)
            originalHpColor = hpFill.color;
        if (unit != null)
        {
            Debug.Log("Successfully retrieved Unit component.");
        }
        else
        {
            Debug.LogError("Failed to retrieve Unit component.");
        }
    }

    void Update()
    {
        updateHpBar();
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
}