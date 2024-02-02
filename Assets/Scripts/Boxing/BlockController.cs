using UnityEngine.UI;
using UnityEngine;

public class BlockController : MonoBehaviour
{
    [SerializeField] private float blockEndLag = 0.3f;
    [SerializeField] Camera cam;
    [SerializeField] Image shieldDisplay;

    private float blockElapsed = 0f;

    public bool Blocking { get; set; } = false;
    public bool JustStoppedBlocking => blockElapsed < blockEndLag;

    void Update()
    {
        blockElapsed += Time.deltaTime;

        if (!shieldDisplay) return;

        shieldDisplay.rectTransform.position = cam.WorldToScreenPoint(transform.position);
        shieldDisplay.color = Color.Lerp(shieldDisplay.color, Blocking ? Color.white : Color.clear, Time.deltaTime * 10f);
    }

    public void SetBlock(FrameInput input)
    {
        bool press = input.BlockInput;

        if (JustStoppedBlocking) return;
        if (!press && Blocking) blockElapsed = 0f;
        Blocking = press;
    }
}
