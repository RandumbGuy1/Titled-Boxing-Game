using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoxingBot : MonoBehaviour
{
    [SerializeField] private Rigidbody rb;
    [SerializeField] Transform[] handPositions = new Transform[2];
    [SerializeField] GloveCollision[] gloves = new GloveCollision[2];
    [SerializeField] private float speed;
    [SerializeField] private float standingRange;
    [SerializeField] private Transform target;

    private float[] elapsed = new float[2];
    private float[] randomTimes = new float[2];

    void FixedUpdate()
    {
        for (int i = 0; i < gloves.Length && i < handPositions.Length; i++)
        {
            gloves[i].DetectCollisions(transform);
            gloves[i].HandleGloves(handPositions[i], transform.forward);
        }

        Vector3 toTarget = target.position - transform.position;
        toTarget.y = 0f;

        if (toTarget.sqrMagnitude > standingRange * standingRange)
        {
            rb.MovePosition(transform.position + toTarget * Time.fixedDeltaTime * speed);
        }
        else
        {
            rb.MovePosition(transform.position + toTarget * Time.fixedDeltaTime * speed * 0.3f);

            for (int i = 0; i < gloves.Length && i < handPositions.Length; i++)
            {
                elapsed[i] += Time.fixedDeltaTime;

                if (elapsed[i] > randomTimes[i] && gloves[i].CanPunch)
                {
                    elapsed[i] = 0f;
                    randomTimes[i] = Random.Range(0.2f, 1f);
                    gloves[i].SetGlove(true);
                }
            }
        }

        rb.rotation = Quaternion.LookRotation(toTarget);
    }
}
