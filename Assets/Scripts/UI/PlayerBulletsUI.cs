using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class PlayerBulletsUI : MonoBehaviour
{
    private TextMeshProUGUI _text;
    public PlayerShooting targetShooting;
    void Awake()
    {
        _text = GetComponent<TextMeshProUGUI>();
    }

    void Update()
    {
        _text.text = "BULLETS: " + targetShooting.bulletsCount;
    }
}
