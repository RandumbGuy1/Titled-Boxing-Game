using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CoinCounter : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI billCountText;
    [SerializeField] TextMeshProUGUI coinCountText;

    void Update()
    {
        billCountText.text = (GameManager.Instance.CoinCount / 50).ToString();
        coinCountText.text = (GameManager.Instance.CoinCount % 50).ToString();
    }
}
