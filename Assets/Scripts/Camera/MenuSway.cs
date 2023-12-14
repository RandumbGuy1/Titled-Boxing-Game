using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuSway : MonoBehaviour
{
    [SerializeField] private Vector3 settingsPos;    [SerializeField] private Vector3 settingsRot;
    [SerializeField] CameraIdleSway idleSway;
    public bool InSettings { get; set; } = false;

    private Vector3 startPos;
    private Vector3 startRot;
    void Start()
    {
        startPos = transform.localPosition;
        startRot = transform.eulerAngles;
    }

    // Update is called once per frame
    void Update()
    {
        idleSway.IdleCameraSway();
        transform.position = Vector3.Lerp(transform.position, InSettings ? settingsPos : startPos, Time.deltaTime * 8f);
        transform.rotation = Quaternion.Slerp(transform.rotation, InSettings ? Quaternion.Euler(settingsRot + idleSway.HeadSwayOffset) : Quaternion.Euler(startRot + idleSway.HeadSwayOffset), Time.deltaTime * 8f);
    }
}
