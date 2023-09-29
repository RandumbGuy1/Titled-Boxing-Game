using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GloveCollision : MonoBehaviour
{
    [SerializeField] ParticleSystem hitSpark;

    void OnCollisionEnter(Collision col)
    {
        Instantiate(hitSpark, col.GetContact(0).point, Quaternion.identity);
    }
}
