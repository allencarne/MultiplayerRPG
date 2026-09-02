using System;
using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Objects/Skill/Passive Triggers/Health Percentage")]
public class HealthPercentTrigger : PassiveTrigger
{
    [Range(0f, 1f)] public float HealthPercentThreshold = 0.3f;

    public override Action Subscribe(PlayerStateMachine owner, Action onTriggered)
    {
        void Handler(float previous, float current)
        {
            float max = owner.PlayerStats.net_TotalHP.Value;
            if (max <= 0) return;

            float prevPct = previous / max;
            float currPct = current / max;

            // Only fire on the crossing, not every tick while below threshold
            if (prevPct > HealthPercentThreshold && currPct <= HealthPercentThreshold)
            {
                Debug.Log($"[PassiveTrigger] LowHealthTrigger fired on {owner.name} ({currPct:P0} HP)");
                onTriggered();
            }
        }

        owner.PlayerStats.net_CurrentHP.OnValueChanged += Handler;
        Debug.Log($"[PassiveTrigger] LowHealthTrigger subscribed on {owner.name}");

        return () =>
        {
            owner.PlayerStats.net_CurrentHP.OnValueChanged -= Handler;
            Debug.Log($"[PassiveTrigger] LowHealthTrigger unsubscribed on {owner.name}");
        };
    }
}
