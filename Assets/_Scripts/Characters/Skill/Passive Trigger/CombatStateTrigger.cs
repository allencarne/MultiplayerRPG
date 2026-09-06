using System;
using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Objects/Skill/Passive Triggers/Combat State")]
public class CombatStateTrigger : PassiveTrigger
{
    public enum TriggerMode
    {
        InCombat,
        OutOfCombat,
        Both
    }

    public TriggerMode TriggerWhen = TriggerMode.InCombat;

    public override Action Subscribe(StateMachine owner, Action onTriggered)
    {
        void Handler(bool inCombat)
        {
            switch (TriggerWhen)
            {
                case TriggerMode.InCombat: if (inCombat) onTriggered(); break;
                case TriggerMode.OutOfCombat: if (!inCombat) onTriggered(); break;
                case TriggerMode.Both: onTriggered(); break;
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
