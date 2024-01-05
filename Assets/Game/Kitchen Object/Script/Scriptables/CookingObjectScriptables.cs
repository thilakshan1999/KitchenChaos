using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Cooking Object", menuName = "Kitchen PLA/Cooking Object")]
public class CookingObjectScriptables : ScriptableObject
{
    public KitchenObjectScriptables uncookedKitchenObjectSO;
    public KitchenObjectScriptables cookedKitchenObjectSO;
    public KitchenObjectScriptables burnedKitchenObjectSO;
    public float cookingTime;
    public float burningTime;
}
