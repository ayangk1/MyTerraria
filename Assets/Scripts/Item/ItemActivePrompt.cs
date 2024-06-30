using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemActivePrompt : MonoBehaviour
{
    public Vector2 pos;

    public void Init(Vector2 newPos)
    {
        float rangeX = Random.Range(-0.1f, 0.1f);
        float rangeY = Random.Range(-0.1f, 0);
        pos = newPos + new Vector2(rangeX, rangeY);
    }

    private void Update()
    {
        Vector2 screenPoint = RectTransformUtility.WorldToScreenPoint(Camera.main, pos);
        transform.position = screenPoint;
        StartCoroutine(Disable());
    }

    private IEnumerator Disable()
    {
        yield return new WaitForSeconds(1);
        ObjectPool.Instance.ReturnPool(transform.gameObject,5);
    }
}
