using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Recipie", menuName = "Kitchen PLA/Recipie")]
public class RecipieScriptables : ScriptableObject
{
    public string foodName;
    public List<KitchenObjectScriptables> ingredientsList;
}
