using System;
using UnityEngine;

public abstract class PassiveTrigger : ScriptableObject
{
    public abstract Action Subscribe(StateMachine owner, Action onTriggered);
}
