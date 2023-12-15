using UnityEngine;

public class BoxerRagdoll : MonoBehaviour
{
    IBoxer boxer;

    void Start()
    {
        boxer = GetComponent<IBoxer>();
    }

    public void Ragdoll()
    {
        boxer.Disable();
    }
}
