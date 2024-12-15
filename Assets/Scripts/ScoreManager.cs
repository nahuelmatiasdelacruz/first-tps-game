using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager SharedInstance;
    [Tooltip("Current game score")]

    public int amount;

    void Awake()
    {
        if (SharedInstance == null)
        {
            SharedInstance = this;
        }else
        {
            Debug.LogWarning("ScoreManager already have an instance. Should be destroyed: ", gameObject);
            Destroy(gameObject);
        }
    }
}
