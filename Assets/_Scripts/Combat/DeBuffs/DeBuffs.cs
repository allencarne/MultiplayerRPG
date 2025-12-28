using Unity.Netcode;

public class DeBuffs : NetworkBehaviour
{
    public Debuff_Slow slow;
    public Debuff_Weakness weakness;
    public Debuff_Impede impede;
    public Debuff_Vulnerability vulnerability;
    public Debuff_Exhaust exhaust;
    public Debuff_Bleed bleed;

    public void CleanseAllDebuffs()
    {
        slow?.CleanseSlow();
        weakness?.CleanseWeakness();
        impede?.CleanseImpede();
        vulnerability?.CleanseVulnerability();
        exhaust?.CleanseExhaust();
        bleed?.CleanseBleed();
    }
}