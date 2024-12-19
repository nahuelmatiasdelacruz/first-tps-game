using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Life : MonoBehaviour
{
    private float _amount;
    
    public float maxLife = 100.0f;
    
    public UnityEvent onDeath;

    private void Awake()
    {
        _amount = maxLife;
    }

    public float Amount
    {
        get => _amount;
        set
        {
            _amount = value;
            if (_amount <= 0)
            {
                onDeath.Invoke();
            }
        }
    }
}