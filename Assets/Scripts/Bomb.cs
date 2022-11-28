using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;

public class Bomb : NetworkBehaviour //, IDamageable
{
    //Animations
    [SerializeField] private float explodeDelay;
    [SerializeField] private NetworkVariable<int> damage;
    [SerializeField] private int distance;
    [SerializeField] private Collider trigger;

    private void Start() => Invoke(nameof(Explode), explodeDelay);

    private void Explode()
    {
        var position = transform.position;
        Raycast(position, Vector3.forward, distance);
        Raycast(position, Vector3.back, distance);
        Raycast(position, Vector3.right, distance);
        Raycast(position, Vector3.left, distance);
        Destroy(gameObject);
    }
    
    private void Raycast(Vector3 pos, Vector3 dir, int dist)
    {
        if (Physics.Raycast(pos, dir, out var hit, dist))
        {
            DoDamage(damage.Value, hit.transform.GetComponent<NetworkObject>());
        }
    }

    private void DoDamage(int damage, NetworkObjectReference hit)
    {
        if (hit.TryGet(out NetworkObject obj))
        {
            DoDamageServerRpc(damage, obj.NetworkObjectId);
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void DoDamageServerRpc(int damage, ulong ID)
    {
        GetNetworkObject(ID).GetComponent<IDamageable>().TakeDamage(damage);
    }


    //public void TakeDamage(int amount) => Explode();

    private void OnTriggerExit(Collider other) => trigger.isTrigger = false;
}