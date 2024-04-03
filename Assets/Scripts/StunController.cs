using UnityEngine.Events;
using UnityEngine;

public class StunController : MonoBehaviour
{
    [SerializeField] private BoxingController boxer;
    private float stunAmount = 0f;
    public bool InStun => stunAmount > 0f;

    public UnityEvent<float> OnPlayerStun;

    void Update()
    {
        stunAmount -= Time.deltaTime;

        if (stunAmount < 0f)
        {
            boxer.SetMoveState(BoxerMoveState.Moving);
        }
    }

    public void Stun(float amount)
    {
        stunAmount = amount;
        boxer.SetMoveState(BoxerMoveState.Stunned);

        OnPlayerStun?.Invoke(amount);
    }
}
