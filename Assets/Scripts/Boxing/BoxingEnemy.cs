using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class BoxingEnemy : MonoBehaviour
{
    public FrameInput EnemyFrameInput { get; private set; } = new FrameInput();
    public UnityEvent<FrameInput> OnFrameInput;

    [SerializeField] float distance;
    [SerializeField] Transform target;
    [SerializeField] Transform orientation;

    void Update()
    {
        EnemyFrameInput.SetInput(
            orientation.forward,
            Vector2.one,
            false,
            false,
            false,
            -1,
            false,
            -1,
            -1);

        float random = Random.Range(0, 100);

        float multi = Mathf.Clamp(Vector3.Distance(orientation.position, target.position) - distance, -1f, 1f);
        Vector3 movDir = orientation.forward * multi;

        if (random > 80)
        {
            //Left Punch
            EnemyFrameInput.SetInput(
            movDir,
            Vector2.one,
            false,
            false,
            false,
            -1,
            false,
            0,
            -1);
        }
        else if (random > 4f)
        {
            //Right Punch
            EnemyFrameInput.SetInput(
            movDir,
            Vector2.one,
            false,
            false,
            false,
            -1,
            false,
            1,
            -1);
        }
        else if (random > 2f)
        {
            //Roll
            EnemyFrameInput.SetInput(
            movDir,
            Vector2.one,
            false,
            false,
            true,
            -1,
            false,
            -1,
            -1);
        }
        else if (random > 0.1f)
        {
            //Slip Left
            EnemyFrameInput.SetInput(
            movDir,
            Vector2.one,
            false,
            false,
            false,
            0,
            false,
            -1,
            -1);
        }
        else
        {
            //Slip Right
            EnemyFrameInput.SetInput(
            movDir,
            Vector2.one,
            false,
            false,
            false,
            1,
            false,
            -1,
            -1);
        }

        OnFrameInput?.Invoke(EnemyFrameInput);
        orientation.LookAt(target);
    }
}
