using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "ItemEventSO",menuName = "Event/ItemEventSO")]
public class ItemEventSO : ScriptableObject
{
    public UnityAction<Item> OnEventRaised;
    
    public void RaiseEvent(Item item)
    {
        OnEventRaised?.Invoke(item);
    }
    
    
}
