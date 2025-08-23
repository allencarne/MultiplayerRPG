using System.Collections;
using Unity.Netcode;
using UnityEngine;

public class CrowdControl : NetworkBehaviour
{
    public CC_Disarm disarm;
    public CC_Silence silence;
    public CC_Immobilize immobilize;
    public CC_Stun stun;
    public CC_Incapacitate incapacitate;
    public CC_KnockBack knockBack;
    public CC_KnockUp knockUp;
    public CC_Pull pull;
    public bool IsInterrupted;
}
