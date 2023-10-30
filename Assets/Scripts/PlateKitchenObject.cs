using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlateKitchenObject : KitchenObject {


    public event EventHandler<OnIngredientAddedEventArgs> OnIngredientAdded;
    public class OnIngredientAddedEventArgs : EventArgs {
        public KitchenObjectSO kitchenObjectSO;
    }


    [SerializeField] private List<KitchenObjectSO> validKitchenObjectSOList;


    private List<KitchenObjectSO> kitchenObjectSOList;


    protected override void Awake() {
        base.Awake();
        kitchenObjectSOList = new List<KitchenObjectSO>();
    }

    public bool TryAddIngredient(KitchenObjectSO kitchenObjectSO) {
        if (!validKitchenObjectSOList.Contains(kitchenObjectSO)) {
            // Not a valid ingredient
            return false;
        }
        if (kitchenObjectSOList.Contains(kitchenObjectSO)) {
            // Already has this type
            return false;
        } else {
            AddIngredientServerRPC(KitchenGameMultiplayer.Instance.GetKitchenObjectSOIndex(kitchenObjectSO));

            return true;
        }
    }

    [ServerRpc(RequireOwnership =false)]
    private void AddIngredientServerRPC(int kitchenIndex)
    {
        AddIngredientClientRPC(kitchenIndex);
    }

    [ClientRpc]
    private void AddIngredientClientRPC(int kitchenIndex)
    {
        KitchenObjectSO KSO = KitchenGameMultiplayer.Instance.GetKitchenObjectSOFromIndex(kitchenIndex);
        kitchenObjectSOList.Add(KSO);

        OnIngredientAdded?.Invoke(this, new OnIngredientAddedEventArgs
        {
            kitchenObjectSO = KSO
        });
    }

    public List<KitchenObjectSO> GetKitchenObjectSOList() {
        return kitchenObjectSOList;
    }

}