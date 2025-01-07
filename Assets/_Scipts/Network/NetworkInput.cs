using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

public class NetworkInput : NetworkBehaviour
{
    [SerializeField] PlayerInputHandler playerInputHandler;
    [SerializeField] PlayerInput playerInput;
    [SerializeField] PlayerStateMachine playerStateMachine;

    private void Awake()
    {
        playerInputHandler.enabled = false;
        playerInput.enabled = false;
        //playerStateMachine.enabled = false;
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        if (IsOwner)
        {
            playerInputHandler.enabled = true;
            playerInput.enabled = true;
            //playerStateMachine.enabled = true;
        }
    }
}
