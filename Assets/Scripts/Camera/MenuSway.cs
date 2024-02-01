using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuSway : MonoBehaviour
{
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
        startRot = transform.eulerAngles;
    }

    // Update is called once per frame
    void Update()
    {
        idleSway.IdleCameraSway();

        switch (MenuState)
        {
            case MainMenuState.Settings:
                transform.position = Vector3.Lerp(transform.position, settingsPos, Time.deltaTime * 8f);
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(settingsRot + idleSway.HeadSwayOffset), Time.deltaTime * 8f);
                break;
            case MainMenuState.LevelSelect:
                transform.position = Vector3.Lerp(transform.position, levelSelectPos, Time.deltaTime * 8f);
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(levelSelectRot + idleSway.HeadSwayOffset), Time.deltaTime * 8f);
                break;
            default:
                transform.position = Vector3.Lerp(transform.position, startPos, Time.deltaTime * 8f);
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(startRot + idleSway.HeadSwayOffset), Time.deltaTime * 8f);
                break;
        }

    }
}
