using System;
using System.Collections;
using Unity.Netcode;
using UnityEngine;

public class DeBuffs : NetworkBehaviour
{
    public Debuff_Slow slow;
    public Debuff_Weakness weakness;
    public Debuff_Impede impede;
    public Debuff_Vulnerability vulnerability;
    public Debuff_Exhaust exhaust;
}