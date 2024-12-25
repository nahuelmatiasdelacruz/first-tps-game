using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sight : MonoBehaviour
{
    public float distance;
    public float angle;

    public LayerMask targetLayers;
    public LayerMask obstacleLayers;

    public Collider detectedTarget;

    void Update()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, distance,targetLayers);

        detectedTarget = null;

        foreach (Collider collider in colliders)
        {
            Vector3 directionToCollider = Vector3.Normalize(collider.bounds.center - transform.position);

            float angleToCollider = Vector3.Angle(transform.forward, directionToCollider);

            if (angleToCollider < angle)
            {
                if (!Physics.Linecast(transform.position, collider.bounds.center, obstacleLayers))
                {
                    // Se guarda la referencia del objetivo detectado
                    detectedTarget = collider;
                    break;
                }
            }
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(transform.position, distance);
    }
}
