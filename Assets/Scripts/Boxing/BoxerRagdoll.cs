using UnityEngine;

public class BoxerRagdoll : MonoBehaviour
{
    BoxingController boxer;
    [SerializeField] GameObject explosion;

    void Start()
    {
        boxer = GetComponent<BoxingController>();
    }

    public void Ragdoll()
    {
        boxer.Disable();
        if (explosion) Instantiate(explosion, transform.position, Quaternion.identity);
    }
}
