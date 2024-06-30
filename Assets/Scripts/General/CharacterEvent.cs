using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterEvent : MonoBehaviour
{
    public BoolEventSO walkEvent;
    public BoolEventSO JumpEvent;
    public BoolEventSO interactEvent;
    public VoidEventSO hitEvent;
    public ItemEventSO attackEvent;
    public VoidEventSO loadDataEvent;
}
