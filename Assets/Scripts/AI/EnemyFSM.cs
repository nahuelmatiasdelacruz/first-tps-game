using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

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

    private Transform baseTransform;

    public float baseAttackDistance = 4, playerAttackDistance = 5;

    private Sight _sight;

    private NavMeshAgent _agent;

    private void Awake()
    {
        _sight = GetComponent<Sight>();
        baseTransform = GameObject.Find("Base").transform;
        _agent = GetComponentInParent<NavMeshAgent>();
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
        _agent.isStopped = false;
        _agent.SetDestination(baseTransform.position);
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
        _agent.isStopped = true;
    }

    void ChasePlayer()
    {
        if (_sight.detectedTarget == null)
        {
            currentState = EnemyState.GoToBase;
            return;
        }
        _agent.isStopped = false;
        _agent.SetDestination(_sight.detectedTarget.transform.position);
        float distanceToPlayer = Vector3.Distance(transform.position, _sight.detectedTarget.transform.position);
        if(distanceToPlayer < playerAttackDistance)
        {
            currentState = EnemyState.AttackPlayer;
        }
    }

    void AttackPlayer()
    {
        if (_sight.detectedTarget == null)
        {
            currentState = EnemyState.GoToBase;
            return;
        }
        _agent.isStopped = true;

        float distanceToPlayer = Vector3.Distance(transform.position, _sight.detectedTarget.transform.position);
        if(distanceToPlayer > (playerAttackDistance * 1.2f))
        {
            currentState = EnemyState.ChasePlayer;
        }
    }
    void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, playerAttackDistance);

        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, baseAttackDistance);
    }
}
