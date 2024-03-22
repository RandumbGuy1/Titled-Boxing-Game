using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class BoxingEnemy : MonoBehaviour
{
    public FrameInput EnemyFrameInput { get; private set; } = new FrameInput();
    public UnityEvent<FrameInput> OnFrameInput;

    [SerializeField] Transform target;
    [SerializeField] Transform orientation;

    void Update()
    {
        EnemyFrameInput.SetInput(
            Vector3.zero,
            Vector2.zero,
            false,
            false,
            false,
            -1,
            false,
            -1,
            -1);

        OnFrameInput?.Invoke(EnemyFrameInput);

        orientation.LookAt(target);
    }
}
