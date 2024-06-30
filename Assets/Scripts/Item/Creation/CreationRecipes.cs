using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CreationRecipes",menuName = "Item/CreationRecipes")]
public class CreationRecipes : ScriptableObject
{
    [Header("outItem-outItemCount-ItemName-Count")]
    public string[] recipes;
    
   
}
