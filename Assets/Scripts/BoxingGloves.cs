using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoxingGloves : MonoBehaviour
{
    //Glove gfx and positioning
    [SerializeField] int handCount = 0;
    [SerializeField] Transform[] handPositions = new Transform[2];
    [SerializeField] GloveCollision[] gloves = new GloveCollision[2];

    [SerializeField] float[] punchRanges = new float[2];
    [SerializeField] float[] punchForwardTimes = new float[2];
    [SerializeField] float[] punchBackTimes = new float[2];
    [SerializeField] float[] punchDelays = new float[2];

    //Player Refrencce
    [SerializeField] PlayerRef player;

    //Data to modify during runtime
    Vector3[] startHandPositions = new Vector3[2];
    bool[] glovesActive = new bool[2];
    float[] punchElapsed = new float[2];

    void Start()
    {
        //Call whenever mouse button is pressed
        player.PlayerInput.OnMouseButtonDownInput += HandlePunching;

        //Save local positions so the hands always return back
        for (int i = 0; i < handPositions.Length; i++) startHandPositions[i] = handPositions[i].localPosition;
    }

    void HandlePunching(int button)
    {
        for (int i = 0; i < handCount; i++)
        {
            punchElapsed[i] += Time.deltaTime;
            gloves[i].Active = glovesActive[i];
            gloves[i].GloveOwner = this;
            gloves[i].GloveIndex = i;

            if (glovesActive[i])
            {
                if (punchElapsed[i] >= punchForwardTimes[i])
                {
                    SetGloveInactive(i);
                    continue;
                }

                Vector3 startPos = handPositions[i].position;
                Vector3 endPos = handPositions[i].position + player.Orientation.forward * punchRanges[i];

                gloves[i].transform.position = Vector3.Lerp(startPos, endPos, EaseInQuad(punchElapsed[i] / punchForwardTimes[i]));
                continue;
            }

            Vector3 endPos2 = handPositions[i].position;
            Vector3 startPos2 = handPositions[i].position + player.Orientation.forward * punchRanges[i];

            gloves[i].transform.position = Vector3.Lerp(startPos2, endPos2, punchElapsed[i] / punchBackTimes[i]);
        }      

        if (button >= 2 || button < 0 || punchElapsed[button] < punchBackTimes[button] + punchDelays[button]) return;

        glovesActive[button] = true;
        punchElapsed[button] = 0f;
    }

    public void SetGloveInactive(int i)
    {
        glovesActive[i] = false;
        punchElapsed[i] = 0f;
    }

    float EaseInQuad(float x)
    {
        return x * x;
    }

    float EaseOutQuad(float x)
    {
        return 1 - (1 - x) * (1 - x);
    }
}
