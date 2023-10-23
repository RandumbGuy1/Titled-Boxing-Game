using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class FadeOutText : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI effectText;
    [SerializeField] private float speed = 5f;

    void Update()
    {
        effectText.color = Color.Lerp(effectText.color, Color.clear, Time.deltaTime * speed);
    }
}
