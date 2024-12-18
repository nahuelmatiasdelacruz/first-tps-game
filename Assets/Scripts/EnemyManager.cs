using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EnemyManager : MonoBehaviour
{
    public static EnemyManager SharedInstance;

    private List<Enemy> _enemies;

    public UnityEvent onEnemyAmountChange;
    
    public int EnemyCount
    {
        get => _enemies.Count;
    }
    
    
    void Awake()
    {
        if (SharedInstance == null)
        {
            SharedInstance = this;
            _enemies = new List<Enemy>();
        }
        else
        {
            Destroy(this);
        }
    }

    public void AddEnemy(Enemy enemy)
    {
        _enemies.Add(enemy);
        onEnemyAmountChange.Invoke();
    }

    public void RemoveEnemy(Enemy enemy)
    {
        _enemies.Remove(enemy);
        onEnemyAmountChange.Invoke();
    }
}
