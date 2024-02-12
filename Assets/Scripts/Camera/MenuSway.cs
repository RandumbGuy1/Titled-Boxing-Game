using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuSway : MonoBehaviour
{
    [SerializeField] private Transform pivot;
    [SerializeField] private float rotateSpeed;
    [SerializeField] private Vector3 settingsPos;    [SerializeField] private Vector3 settingsRot;

    [Space(10)]

    [SerializeField] private Vector3 levelSelectPos;    [SerializeField] private Vector3 levelSelectRot;

    [SerializeField] CameraIdleSway idleSway;
    public MainMenuState MenuState { get; set; }

    private Vector3 startPos;
    private Vector3 startRot;
    void Start()
    {
        startPos = transform.localPosition;
        startRot = transform.localEulerAngles;
    }

    // Update is called once per frame
    void Update()
    {
        idleSway.IdleCameraSway();

        switch (MenuState)
        {
            case MainMenuState.Settings:
                transform.localPosition = Vector3.Lerp(transform.localPosition, settingsPos, Time.deltaTime * 8f);
                transform.localRotation = Quaternion.Slerp(transform.localRotation, Quaternion.Euler(settingsRot + idleSway.HeadSwayOffset), Time.deltaTime * 8f);
                break;
            case MainMenuState.LevelSelect:
                transform.localPosition = Vector3.Lerp(transform.localPosition, levelSelectPos, Time.deltaTime * 8f);
                transform.localRotation = Quaternion.Slerp(transform.localRotation, Quaternion.Euler(levelSelectRot + idleSway.HeadSwayOffset), Time.deltaTime * 8f);
                break;
            default:
                pivot.Rotate(new Vector3(0, rotateSpeed, 0) * Time.deltaTime);
                transform.localPosition = Vector3.Lerp(transform.localPosition, startPos, Time.deltaTime * 8f);
                transform.localRotation = Quaternion.Slerp(transform.localRotation, Quaternion.Euler(startRot + idleSway.HeadSwayOffset), Time.deltaTime * 8f);
                break;
        }

    }
}
