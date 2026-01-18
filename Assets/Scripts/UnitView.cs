using UnityEngine;
using UnityEngine.UI;

public class UnitView : MonoBehaviour
{
    public Image hpFill;
    private Unit unit;

    void Awake()
    {
        unit = GetComponentInParent<Unit>();
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
        if (unit)
            hpFill.fillAmount = Mathf.Clamp01(unit.hp / unit.maxHp);
    }

    void LateUpdate()
    {
        if (Camera.main != null)
        {
            transform.rotation = Quaternion.LookRotation(transform.position - Camera.main.transform.position);
            transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x, 0, transform.rotation.eulerAngles.z);
        }

    }
}