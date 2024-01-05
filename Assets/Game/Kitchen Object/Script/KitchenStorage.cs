using System.Collections;
using Unity.Netcode;
using UnityEngine;

public class KitchenStorage : NetworkBehaviour
{
    #region VARIABLE
    [SerializeField] private KitchenObjectScriptables kitchenObjectSO;
    private KitchenObject KitchenObjectScript;
    #endregion

    #region CALL FUNCTION
    internal void GetKitchenObject(IKitchenObjectCarrier kitchenObjectCarrier)
    {
        CheckKitchenObjectAvailabilityInServerRpc(kitchenObjectCarrier.GetNetworkObject());
    }
    internal KitchenObjectScriptables GetKitchenObjectSO()
    {
        return kitchenObjectSO;
    }

    #endregion

    #region NET CODE

    [ServerRpc (RequireOwnership =false)]
    private void SpawnKitchenObjectInServerRpc(NetworkObjectReference kictendObjectCarrierNetworkObjectReference)
    {
        GameObject kitchenObject = Instantiate(kitchenObjectSO.kitchenObject, this.transform);
        kitchenObject.GetComponent<KitchenObject>().storage = this.transform;
        KitchenObjectScript = kitchenObject.GetComponent<KitchenObject>();
        kitchenObject.GetComponent<NetworkObject>().Spawn(true);

        kictendObjectCarrierNetworkObjectReference.TryGet(out NetworkObject kitchenObjectCarrierNetworkObject);
        IKitchenObjectCarrier kitchenObjectCarrier = kitchenObjectCarrierNetworkObject.GetComponent<IKitchenObjectCarrier>();
        kitchenObjectCarrier.AttachKitchenObjectWithCarrier(kitchenObject.GetComponent<KitchenObject>());
    }


    [ServerRpc(RequireOwnership = false)]
    private void CheckKitchenObjectAvailabilityInServerRpc(NetworkObjectReference kictendObjectCarrierNetworkObjectReference)
    {
        if (transform.childCount != 0 )
        {
            GameObject kitchenObject = null;
            for(int i = 0; i < transform.childCount; i++)
            {
                if (!transform.GetChild(i).GetComponent<KitchenObject>().GetIsActive())
                {
                    kitchenObject = transform.GetChild(i).gameObject;
                    break;
                }
            }
            if(kitchenObject != null){
                kictendObjectCarrierNetworkObjectReference.TryGet(out NetworkObject kitchenObjectCarrierNetworkObject);
                IKitchenObjectCarrier kitchenObjectCarrier = kitchenObjectCarrierNetworkObject.GetComponent<IKitchenObjectCarrier>();
                kitchenObjectCarrier.AttachKitchenObjectWithCarrier(kitchenObject.GetComponent<KitchenObject>());
            }
            else
            {
                SpawnKitchenObjectInServerRpc(kictendObjectCarrierNetworkObjectReference);
            }
        }
        else
        {
            SpawnKitchenObjectInServerRpc(kictendObjectCarrierNetworkObjectReference);
        }
    }
    #endregion
}
