using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class FollowTranform : NetworkBehaviour
{
    private Transform targetTransform;

    public void SetTargetTransform(Transform targetTransform)
    {
        this.targetTransform = targetTransform;
    }

    public void ResetTargetTranform(Transform parent)
    {
        targetTransform = null;
        transform.localRotation = Quaternion.Euler(0, 0, 0);

        if (IsServer)
        {
            UpdatePoestionToClientRpc(parent.transform.position);
        }
    }
    private void LateUpdate()
    {
        if (targetTransform == null) return;

        transform.position = targetTransform.position;
        transform.rotation = targetTransform.rotation;
    }

    [ClientRpc]
    private void UpdatePoestionToClientRpc(Vector3 poestion)
    {
        transform.position = poestion;
    }
}
