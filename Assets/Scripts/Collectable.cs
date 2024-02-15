using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collectable : MonoBehaviour
{
    [SerializeField] int coinValue;
    [SerializeField] GameObject pickupEffect;
    [SerializeField] AudioClip pickupSound;

    public void Pickup()
    {
        AudioManager.Instance.PlayOnce(pickupSound);
        GameManager.Instance.CoinCount += coinValue;

        Instantiate(pickupEffect, transform.position, Quaternion.identity);

        gameObject.SetActive(false);
    }
}
