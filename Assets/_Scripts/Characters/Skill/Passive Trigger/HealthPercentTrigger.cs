using System;
using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Objects/Skill/Passive Triggers/Health Percentage")]
public class HealthPercentTrigger : PassiveTrigger
{
    [Range(0f, 1f)] public float HealthPercentThreshold = 0.3f;

    public override Action Subscribe(StateMachine owner, Action onTriggered)
    {
        void Handler(float previous, float current)
        {
            float max = owner.Stats.net_TotalHP.Value;
            if (max <= 0) return;

            float prevPct = previous / max;
            float currPct = current / max;

            // Only fire on the crossing, not every tick while below threshold
            if (prevPct > HealthPercentThreshold && currPct <= HealthPercentThreshold)
            {
                onTriggered();
            }
        }

        owner.Stats.net_CurrentHP.OnValueChanged += Handler;

        void Unsubscribe()
        {
            owner.Stats.net_CurrentHP.OnValueChanged -= Handler;
        }

        return Unsubscribe;
    }
}
