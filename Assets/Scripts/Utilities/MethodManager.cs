using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MethodManager : SingletonMonoBehaviour<MethodManager>
{
    public void FindAllChildrenRecursive(GameObject parent,ref List<GameObject> children)
    {
        Debug.Log(123123);
        for (int i = 0; i < parent.transform.childCount; i++)
        {
            GameObject child = parent.transform.GetChild(i).gameObject;
            children.Add(child);
            
            FindAllChildrenRecursive(child,ref children); // 递归查找子物体的子物体
        }
    } 
}
