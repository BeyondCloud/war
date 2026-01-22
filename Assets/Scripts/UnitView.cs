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
    public Transform unitSpriteTransform;
    
    private LineRenderer lineRenderer;
    private Vector3 targetLineOffset = new Vector3(0, 0, 0.5f);
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
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.startWidth = 0.15f;
        lineRenderer.endWidth = 0.15f;
        lineRenderer.positionCount = 0;
        lineRenderer.SetWidth(0.2f, 0.01f);
    }
    void DrawPath()
    {
        if(unit.team == Team.Blue)
        {
            lineRenderer.startColor = Color.blue;
            lineRenderer.endColor = Color.blue;
        }
        else
        {
            lineRenderer.startColor = Color.red;
            lineRenderer.endColor = Color.red;
        }
        
        if (unit.currentTarget)
        {
            lineRenderer.positionCount = 2;
            if (unit.team == Team.Red)
            {
                lineRenderer.SetPosition(0, unit.transform.position + targetLineOffset);
                lineRenderer.SetPosition(1, unit.currentTarget.transform.position + targetLineOffset);
            }
            else
            {
                lineRenderer.SetPosition(0, unit.transform.position);
                lineRenderer.SetPosition(1, unit.currentTarget.transform.position);
            }
        }
        else
        {
            lineRenderer.positionCount = 0;
        }
    }
    void Update()
    {
        updateHpBar();
        updateFacingDirection();
        DrawPath();
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
            var scale = unitSpriteTransform.localScale;
            scale.x = Mathf.Abs(scale.x) * unit.faceDirection.x;
            unitSpriteTransform.localScale = scale;
        }
    }
}