using UnityEngine;

public class PlayerPassive
{
    public PassiveData passiveData;
    int passiveIndex;

    public PlayerPassive(PassiveData data, int index)
    {
        passiveData = data;
        passiveIndex = index;
    }

    public virtual void StartPassive(PlayerStateMachine owner)
    {

    }

    public virtual void UpdatePassive(PlayerStateMachine owner)
    {

    }
    public virtual void FixedUpdatePassive(PlayerStateMachine owner)
    {

    }

    public virtual void EndPassive(PlayerStateMachine owner)
    {

    }
}
