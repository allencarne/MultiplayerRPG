using Unity.Netcode;

public class Buffs : NetworkBehaviour
{
    public Buff_Haste haste;
    public Buff_Might might;
    public Buff_Alacrity alacrity;
    public Buff_Protection protection;
    public Buff_Swiftness swiftness;

    public Buff_Phase phase;
    public Buff_Immune immune;
    public Buff_Immoveable immoveable;
    public Buff_Regeneration regeneration;

    public void PurgeAllDebuffs()
    {
        haste?.PurgeHaste();
        might?.PurgeMight();
        swiftness?.PurgeSwiftness();
        alacrity?.PurgeAlacrity();
        protection?.PurgeProtection();

        immoveable?.PurgeImmovable();
        immune?.PurgeImmune();
        phase?.PurgePhase();
    }
}