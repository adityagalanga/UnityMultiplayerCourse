using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class KitchenGameMultiplayer : NetworkBehaviour
{
    public static KitchenGameMultiplayer Instance { get; private set; }
    [SerializeField] private KitchenObjectListSO kitchenObjectListSO;
    private void Awake()
    {
        Instance = this;
    }


    public void SpawnKitchenObject(KitchenObjectSO kitchenObjectSO, IKitchenObjectParent kitchenObjectParent)
    {
        SpawnKitchenObjectServerRPC(GetKitchenObjectSOIndex(kitchenObjectSO), kitchenObjectParent.GetNetworkObject());
    }

    [ServerRpc(RequireOwnership = false)]
    private void SpawnKitchenObjectServerRPC(int kitchenObjectSOIndex, NetworkObjectReference kitchenObjectParentNetworkObjectReference)
    {
        KitchenObjectSO kitchenObjectSO = GetKitchenObjectSOFromIndex(kitchenObjectSOIndex);
        Transform kitchenObjectTransform = Instantiate(kitchenObjectSO.prefab);

        NetworkObject kitchenObjectNetworkObject = kitchenObjectTransform.GetComponent<NetworkObject>();
        kitchenObjectNetworkObject.Spawn(true);

        KitchenObject kitchenObject = kitchenObjectTransform.GetComponent<KitchenObject>();

        kitchenObjectParentNetworkObjectReference.TryGet(out NetworkObject kitchenObjectParentNetworkObject);
        IKitchenObjectParent kitchenObjectParent = kitchenObjectParentNetworkObject.GetComponent<IKitchenObjectParent>();

        kitchenObject.SetKitchenObjectParent(kitchenObjectParent);
    }

    private int GetKitchenObjectSOIndex(KitchenObjectSO kitchenObject)
    {
        return kitchenObjectListSO.kitchenObjectSOList.IndexOf(kitchenObject);
    }

    private KitchenObjectSO GetKitchenObjectSOFromIndex(int kitchenObjectIndex)
    {
        return kitchenObjectListSO.kitchenObjectSOList[kitchenObjectIndex];
    }

    public void DestroyKitchenObject(KitchenObject kitchenObject)
    {
        DestroyKitchenObjectServerRPC(kitchenObject.NetworkObject);
    }

    [ServerRpc(RequireOwnership =false)]
    private void DestroyKitchenObjectServerRPC(NetworkObjectReference networkObjectReference)
    {
        networkObjectReference.TryGet(out NetworkObject kitchenNetworkObject);
        KitchenObject kitchenObject =  kitchenNetworkObject.GetComponent<KitchenObject>();

        CleareKitchenObjectOnParentClientRPC(networkObjectReference);
        kitchenObject.DestroySelf();
    }

    [ClientRpc]
    private void CleareKitchenObjectOnParentClientRPC(NetworkObjectReference networkObjectReference)
    {
        networkObjectReference.TryGet(out NetworkObject kitchenNetworkObject);
        KitchenObject kitchenObject = kitchenNetworkObject.GetComponent<KitchenObject>();
        kitchenObject.ClearKitchenObjectOnParent();
    }
}
