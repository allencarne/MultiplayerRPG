using Unity.Netcode;
using UnityEngine;

public class NPCInteract : NetworkBehaviour
{
    [TextArea(3,8)] public string[] Dialogue;
}
