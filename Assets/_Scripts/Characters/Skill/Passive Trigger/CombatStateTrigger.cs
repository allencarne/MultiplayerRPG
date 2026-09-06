using System;
using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Objects/Skill/Passive Triggers/Combat State")]
public class CombatStateTrigger : PassiveTrigger
{
    public bool TriggerWhenInCombat = true;

    public override Action Subscribe(StateMachine owner, Action onTriggered)
    {
        void Handler(bool inCombat)
        {
            if (inCombat == TriggerWhenInCombat)
            {
                onTriggered();
            }
        }

        owner.Combat.OnCombatStateChanged.AddListener(Handler);

        void Unsubscribe()
        {
            owner.Combat.OnCombatStateChanged.RemoveListener(Handler);
        }

        return Unsubscribe;
    }
}
