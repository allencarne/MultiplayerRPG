using UnityEngine;

[System.Serializable]
public class SpawnEffect
{
    public GameObject Prefab;
    public int Amount = 1;
    public float Force;
    public float Duration;
    public float SpreadAngle;
    public int RepeatAmount = 1;
    public float RepeatRate;

    [Header("On Hit")]
    [SerializeReference] public SpawnEffect[] OnTrigger_Spawn;
    [SerializeReference] public ApplyEffect[] OnTrigger_Apply;

    public void Execute(PlayerStateMachine owner)
    {

    }
}
