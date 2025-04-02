using System.Collections;
using Unity.Netcode;
using UnityEngine;

public class TestAbility : EnemyAbility
{
    [SerializeField] GameObject attackPrefab;
    public float DashForce = 10f;

    GameObject attackInstance;

    public override void Activate(EnemyStateMachine owner)
    {
        //if (!owner.CanAttack) { return; }

        AttackServerRpc(owner.transform.position, owner.transform.rotation);
    }

    [ServerRpc]
    void AttackServerRpc(Vector2 ownerPos, Quaternion ownerRot)
    {
        if (attackInstance == null)
        {
            Debug.Log("rpc");

            // Spawn the prefab in normally (on the server)
            GameObject instance = attackInstance = Instantiate(attackPrefab, ownerPos, ownerRot);
            // Get the Network Object component
            NetworkObject instanceNet = instance.GetComponent<NetworkObject>();
            // Replicate the object to all clients and give ownership to the client
            instanceNet.SpawnWithOwnership(OwnerClientId);
            attackInstance = instance;

            Rigidbody2D _RB = attackInstance.GetComponent<Rigidbody2D>();
            _RB.AddForce(Vector2.right * DashForce, ForceMode2D.Impulse);
        }
    }
}
