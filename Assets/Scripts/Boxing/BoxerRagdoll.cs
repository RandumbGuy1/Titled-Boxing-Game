using UnityEngine;

public class BoxerRagdoll : MonoBehaviour
{
    BoxingController boxer;

    void Start()
    {
        boxer = GetComponent<BoxingController>();
    }

    public void Ragdoll()
    {
        boxer.Disable();
    }
}
