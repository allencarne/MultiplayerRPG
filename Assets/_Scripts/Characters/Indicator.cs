using UnityEngine;

public class Indicator : MonoBehaviour
{
    [Header("Aimer")]
    public Transform Aimer;

    [Header("Skill Indicator")]
    string indicatorType = null;
    GameObject indicator;
    public Vector2 LastGroundPosition { get; private set; }

    [Header("Range Indicator")]
    [SerializeField] GameObject RangeIndicatorPrefab;
    GameObject rangeIndicatorInstance;
    SpriteRenderer rangeIndicatorRenderer;

    public void InstantiateIndicator(GameObject prefab, string type)
    {
        if (indicator != null && indicatorType != type)
        {
            Destroy(indicator);
            indicator = null;
        }

        if (indicator == null)
        {
            indicator = Instantiate(prefab, transform.position, Aimer.rotation, transform);
            indicatorType = type;
        }
        else
        {
            indicator.transform.rotation = Aimer.rotation;
        }
    }

    public void InstantiateIndicator(GameObject prefab, string type, Vector2 worldPosition)
    {
        if (indicator != null && indicatorType != type)
        {
            Destroy(indicator);
            indicator = null;
        }

        if (indicator == null)
        {
            // For ground-targeting we don't parent to the player; place at world position.
            indicator = Instantiate(prefab, worldPosition, Quaternion.identity, null);
            indicatorType = type;
        }
        else
        {
            indicator.transform.position = worldPosition;
        }
    }

    public void DestroyIndicator(string type)
    {
        if (indicator != null && indicatorType == type)
        {
            Destroy(indicator);
            indicator = null;
            indicatorType = null;
        }

        HideRangeIndicator();
    }

    public void DestroyAllIndicators()
    {
        DestroyIndicator("Offensive");
        DestroyIndicator("Mobility");
        DestroyIndicator("Defensive");
        DestroyIndicator("Utility");
        DestroyIndicator("Ultimate");
    }

    public void HandleAbilityIndicator(SkillData data, string indicatorName, bool isHeld, PlayerInputHandler input)
    {
        if (isHeld)
        {
            if (data.TargetingMode == SkillData.Targeting.Ground)
            {
                Vector2 worldPos = input.cameraInstance != null
                    ? (Vector2)input.cameraInstance.ScreenToWorldPoint(UnityEngine.Input.mousePosition)
                    : input.MousePosition;

                Vector2 clampedPos = ClampToRange(worldPos, data.SkillRange);

                LastGroundPosition = clampedPos;
                ShowRangeIndicator(data.SkillRange);
                InstantiateIndicator(data.IndicatorPrefab, indicatorName, clampedPos);
            }
            else
            {
                InstantiateIndicator(data.IndicatorPrefab,indicatorName);
            }
        }
        else
        {
            DestroyIndicator(indicatorName);
        }
    }

    Vector2 ClampToRange(Vector2 worldPos, float range)
    {
        Vector2 origin = transform.position;
        Vector2 toTarget = worldPos - origin;

        if (toTarget.sqrMagnitude > range * range)
        {
            toTarget = toTarget.normalized * range;
        }

        return origin + toTarget;
    }

    void ShowRangeIndicator(float range)
    {
        if (rangeIndicatorInstance == null)
        {
            rangeIndicatorInstance = Instantiate(RangeIndicatorPrefab, transform);
            rangeIndicatorInstance.transform.localPosition = Vector3.zero;
            rangeIndicatorRenderer = rangeIndicatorInstance.GetComponent<SpriteRenderer>();
        }

        rangeIndicatorInstance.SetActive(true);

        float nativeDiameter = rangeIndicatorRenderer.sprite.bounds.size.x;
        float scaleFactor = (range * 2f) / nativeDiameter;
        rangeIndicatorInstance.transform.localScale = new Vector3(scaleFactor, scaleFactor, 1f);
    }

    void HideRangeIndicator()
    {
        if (rangeIndicatorInstance != null)
        {
            rangeIndicatorInstance.SetActive(false);
        }
    }
}
