using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Cutting Object", menuName = "Kitchen PLA/Cutting Object")]
public class CuttingObjectScriptable : ScriptableObject
{
    public KitchenObjectScriptables inputKitchenObjectSO;
    public KitchenObjectScriptables outputKitchenObjectSO;
    public int cuttingCount;
}
