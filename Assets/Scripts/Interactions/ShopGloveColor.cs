using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopGloveColor : IInteractable
{
    public override PlayerRef Player => null;
    bool bought = false;
    [SerializeField] Color gloveColor;
    [SerializeField] int price = 20;
    [SerializeField] AudioClip buyClip;
    [SerializeField] MeshRenderer meshRenderer;

    void Start()
    {
        meshRenderer.material.color = gloveColor;
        InteractText.enabled = false;
        InteractText.text += "\n<size=5>$" + price;
    }

    public override string GetDescription(Player player)
    {
        return bought ? null : "Buy for $" + price;
    }

    public override void OnInteract(Player player)
    {
        if (GameManager.Instance.CoinCount < price) return;

        GameManager.Instance.CoinCount -= 20;
        AudioManager.Instance.PlayOnce(buyClip, transform.position);
        bought = true;
        InteractText.enabled = false;

        GameManager.Instance.GloveColor = gloveColor;

        OutlineHoverEnd();
    }

    public override void OnStartHover(Player player)
    {

    }

    public override void OnEndHover(Player player)
    {

    }

    void Update()
    {
        if (!bought) return;
        transform.localScale = Vector3.Lerp(transform.localScale, Vector3.one * 0.0001f, Time.deltaTime * 10f);

        if (transform.localScale.x <= 0.001f) gameObject.SetActive(false);
    }
}
