using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoDestroy : MonoBehaviour
{
    [Tooltip("Time before destroy the object")]
    public float hideDelay;
    void OnEnable()
    {
        Invoke("HideObject",hideDelay);
    }

    void HideObject()
    {
        gameObject.SetActive(false);
    }
}
