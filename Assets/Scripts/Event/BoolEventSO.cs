using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "BoolEventSO",menuName = "Event/BoolEventSO")]
public class BoolEventSO : ScriptableObject
{
    public UnityAction<bool> OnEventRaised;

    // delegate + event == Action;
    public Action<bool, int> OnDes;
    public delegate void NoticeEventHander(bool m_bool,int sdassd);
    public static event NoticeEventHander OnNotice;
    
    public void RaiseEvent(bool m_bool)
    {
        OnEventRaised?.Invoke(m_bool);
        
    }

    
}
