using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class EnemiesUI : MonoBehaviour
{
    private TextMeshProUGUI _text;
    
    void Awake()
    {
        _text = GetComponent<TextMeshProUGUI>();
        EnemyManager.SharedInstance.onEnemyAmountChange.AddListener(RefreshText);
    }

    private void RefreshText()
    {
        _text.text = "Enemies: " + EnemyManager.SharedInstance.EnemyCount;
    }
}
