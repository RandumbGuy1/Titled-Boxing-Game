using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collectable : MonoBehaviour
{
    [SerializeField] GameObject pickupEffect;
    [SerializeField] AudioClip pickupSound;

    public void Pickup()
    {
        AudioManager.Instance.PlayOnce(pickupSound);
        Instantiate(pickupEffect, transform.position, Quaternion.identity);

        gameObject.SetActive(false);
    }
}
