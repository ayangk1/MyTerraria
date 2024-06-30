using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class PlayerTouching : MonoBehaviour
{
    
    [SerializeField] private Vector3[] check;
    [SerializeField] private LayerMask layer;

    [SerializeField]
    private bool isGround;
    public bool IsGround
    {
        get { return isGround; }
        set
        {
            if (value != isGround)
            {
                isGround = value;
            }
        }
    }
    public bool isStep;
    public ContactFilter2D contactFilter;
    private PolygonCollider2D capsuleCollider;
    public RaycastHit2D[] raycastHits = new RaycastHit2D[5];

    void Start()
    {
        capsuleCollider = GetComponent<PolygonCollider2D>();
        contactFilter.SetLayerMask(LayerMask.GetMask("Ground"));
        
    }
    private void FixedUpdate()
    {
        IsGround = capsuleCollider.Cast(Vector2.down, contactFilter, raycastHits, 0.25f) > 0;
        isStep = Physics2D.OverlapCircle(transform.position + new Vector3(check[0].x, check[0].y, 0), check[0].z, layer) 
                 || Physics2D.OverlapCircle(transform.position + new Vector3(check[1].x, check[1].y, 0), check[1].z, layer);
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position + new Vector3(check[0].x, check[0].y, 0), check[0].z);
        Gizmos.DrawWireSphere(transform.position + new Vector3(check[1].x, check[1].y, 0), check[1].z);
        
        Gizmos.DrawLine(transform.position + new Vector3(check[0].x, check[0].y, 0) , transform.position + new Vector3(check[1].x, check[1].y, 0));
    }
}
