using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConstantRotate : MonoBehaviour
{
    [SerializeField] private Vector3 rotateEuler;
    private float randomMultiplier = 1f;
    public void Start()
    {
        randomMultiplier = Random.Range(1f, 1.1f);
    }

    void Update()
    {
        transform.Rotate(rotateEuler * Time.deltaTime * randomMultiplier);
    }
}
