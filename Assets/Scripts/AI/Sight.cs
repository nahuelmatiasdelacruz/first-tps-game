using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sight : MonoBehaviour
{
    public float distance;
    public float angle;

    public LayerMask targetLayers;
    public LayerMask obstacleLayers;

    void Update()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, distance,targetLayers);

        foreach (Collider collider in colliders)
        {
            Vector3 directionToCollider = Vector3.Normalize(collider.bounds.center - transform.position);

            float angleToCollider = Vector3.Angle(transform.forward, directionToCollider);
            
            
            
        }
    }
    
}
