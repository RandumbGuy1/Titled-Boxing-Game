using UnityEngine.UI;
using UnityEngine;

public class BlockController : MonoBehaviour
{
    [SerializeField] private BoxingController boxer;
    [SerializeField] private float blockEndLag = 0.3f;
    [SerializeField] Camera cam;
    [SerializeField] Image shieldDisplay;

    private float blockElapsed = 0f;

    public bool JustStoppedBlocking => blockElapsed < blockEndLag;

    void Update()
    {
        blockElapsed += Time.deltaTime;

        if (!shieldDisplay) return;

        shieldDisplay.rectTransform.position = cam.WorldToScreenPoint(transform.position);
        shieldDisplay.color = Color.Lerp(shieldDisplay.color, boxer.Blocking ? Color.white : Color.clear, Time.deltaTime * 10f);
    }

    public void SetBlock(FrameInput input)
    {
        bool blocking = input.BlockInput;

        if (JustStoppedBlocking) return;

        switch (boxer.AttackState)
        {
            case BoxerAttackState.Idle:
                if (blocking) boxer.SetAttackState(BoxerAttackState.Blocking);
                break;
            case BoxerAttackState.Blocking:
                if (blocking) return;

                boxer.SetAttackState(BoxerAttackState.Idle);
                blockElapsed = 0f;
                
                break;
            default:
                break;
        }
    }

    public void DisableBlock()
    {
        boxer.SetAttackState(BoxerAttackState.Idle);
        blockElapsed = 0f;
    } 
}
