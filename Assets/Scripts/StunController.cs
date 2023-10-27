using UnityEngine.Events;
using UnityEngine;

public class StunController : MonoBehaviour
{
    private float stunAmount = 0f;
    public bool InStun => stunAmount > 0f;

    public UnityEvent<Vector3, float> OnPlayerStun;

    void Update()
    {
        stunAmount -= Time.deltaTime;
    }

    public void Stun(Vector3 dir, float amount)
    {
        stunAmount = amount;
        OnPlayerStun?.Invoke(dir, amount);
    }
}
