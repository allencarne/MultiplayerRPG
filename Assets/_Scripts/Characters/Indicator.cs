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

    [Header("Stick Aiming (Gamepad / Mobile)")]
    [SerializeField] float stickDeadzone = 0.05f;

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

    public void HandleAbilityIndicator(ActiveSkillData data, string indicatorName, bool isHeld, PlayerInputHandler input, string controlScheme)
    {
        if (isHeld)
        {
            if (data.TargetingMode == ActiveSkillData.Targeting.Ground)
            {
                Vector2 targetPos = ComputeGroundTargetPosition(data, input, controlScheme);

                LastGroundPosition = targetPos;
                ShowRangeIndicator(data.SkillRange);
                InstantiateIndicator(data.IndicatorPrefab, indicatorName, targetPos);
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

    Vector2 ComputeGroundTargetPosition(ActiveSkillData data, PlayerInputHandler input, string controlScheme)
    {
        Vector2 origin = transform.position;

        bool useStick = Application.isMobilePlatform || controlScheme == "Gamepad";

        if (useStick)
        {
            Vector2 stickInput = Application.isMobilePlatform ? input.MoveInput : input.LookInput;

            if (stickInput.magnitude > stickDeadzone)
            {
                Vector2 direction = stickInput.normalized;
                float magnitude = Mathf.Clamp01(stickInput.magnitude);
                return origin + direction * (data.SkillRange * magnitude);
            }

            // No meaningful stick input — indicator sits at the player's feet.
            return origin;
        }
        else
        {
            Vector2 worldPos = input.cameraInstance != null ? (Vector2)input.cameraInstance.ScreenToWorldPoint(UnityEngine.Input.mousePosition) : input.MousePosition;
            return ClampToRange(worldPos, data.SkillRange);
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
