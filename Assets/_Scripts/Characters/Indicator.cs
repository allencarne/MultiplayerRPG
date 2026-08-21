using UnityEngine;

public class Indicator : MonoBehaviour
{
    public Transform Aimer;

    [Header("Indicator")]
    string indicatorType = null;
    GameObject indicator;

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
                Vector2 worldPos = input.cameraInstance != null ? (Vector2)input.cameraInstance.ScreenToWorldPoint(UnityEngine.Input.mousePosition): input.MousePosition;

                InstantiateIndicator(data.IndicatorPrefab,indicatorName,worldPos);
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
}
