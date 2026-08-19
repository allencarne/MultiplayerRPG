using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Objects/Skill Effects/SparkEffect")]
public class SparkEffect : SkillEffect
{
    [SerializeField] GameObject Spark;

    public override void Execute(PlayerStateMachine owner, SkillContext ctx)
    {
        // Spawn Spark Effect

        // Add RPC to PlayerStateMachine
    }
}
