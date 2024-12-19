using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class WavesUI : MonoBehaviour
{
    private TextMeshProUGUI _text;
    
    private void Start()
    {
        _text = GetComponent<TextMeshProUGUI>();
        WaveManager.SharedInstance.onWaveChange.AddListener(RefreshText);
        RefreshText();
    }

    private void RefreshText()
    {
        _text.text = "Wave: " + (WaveManager.SharedInstance.TotalWaves - WaveManager.SharedInstance.WaveCount) + "/" + WaveManager.SharedInstance.TotalWaves;
    }
}
