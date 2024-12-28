using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Sight))]
public class EnemyFSM : MonoBehaviour
{
    public enum EnemyState
    {
        GoToBase,
        AttackBase,
        ChasePlayer,
        AttackPlayer,
    }

    public EnemyState currentState;

    public Transform baseTransform;

    public float baseAttackDistance;

    private Sight _sight;

    private void Awake()
    {
        _sight = GetComponent<Sight>();
    }

    void Update()
    {
        switch (currentState)
        {
            case EnemyState.GoToBase:
                GoToBase();
                break;
            case EnemyState.AttackBase:
                AttackBase();
                break;
            case EnemyState.ChasePlayer:
                ChasePlayer();
                break;
            case EnemyState.AttackPlayer:
                AttackPlayer();
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(currentState), currentState, "Unknown enemy state encountered");
        }
    }

    void GoToBase()
    {
        if(_sight.detectedTarget != null)
        {
            currentState = EnemyState.ChasePlayer;
        }
        float distanceToBase = Vector3.Distance(transform.position, baseTransform.position);
        if(distanceToBase < baseAttackDistance)
        {
            currentState = EnemyState.AttackBase;
        }
    }

    void AttackBase()
    {
        print("Attack base");
    }

    void ChasePlayer()
    {
        print("Chase player");
    }

    void AttackPlayer()
    {
        print("Attack player");
    }

}
