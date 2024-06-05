using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerInteraction : MonoBehaviour
{
    [Header("Interaction Settings")]
    [SerializeField] private LayerMask InteractionObstruction;
    [SerializeField] private LayerMask Interactables;
    [SerializeField] private float interactionRange;
    [SerializeField] private float interactionRadius;

    [Header("Refrences")]
    [SerializeField] private bool showUI = true;
    [SerializeField] private Player player;
    [SerializeField] private TextMeshProUGUI interactionKeyBindText;
    [SerializeField] private TextMeshProUGUI interactionText;
    [SerializeField] private GameObject interactionUI;

    private IInteractable interactable;

    void Awake()
    {
        player.Keys.OnInteractInput += CheckForInteractable;
    }

    private void CheckForInteractable(bool interact)
    {
        Ray ray = player.PlayerCam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
        if (Physics.SphereCast(ray, interactionRadius, out var hit, interactionRange + (player.transform.position - player.PlayerCam.transform.position).magnitude, Interactables, QueryTriggerInteraction.Ignore))
        {
            GameObject currentleyLookingAt = hit.transform.gameObject;

            if (Physics.Linecast(player.PlayerCam.transform.position, currentleyLookingAt.transform.position, InteractionObstruction))
            {
                ResetInteraction();
                return;
            }

            if (interactable == null)
            {
                IInteractable interactableTemp = hit.transform.GetComponent<IInteractable>();

                if (interactableTemp == null) return;

                interactable = interactableTemp;
                interactable.OnStartHover(player);
                interactable.OutlineHoverStart();
            }
            else if (currentleyLookingAt != interactable.gameObject)
            {
                ResetInteraction();
                return;
            }

            string text = interactable.GetDescription(player);
            if (text == null)
            {
                ResetInteraction(false);
                return;
            }

            if (showUI) interactionUI.SetActive(true);

            interactionText.text = text;
            interactionKeyBindText.text = player.Keys.InteractKey.ToString();

            if (interact)
            {
                interactable.OnInteract(player);
                interactable.OutlineHoverEnd();
            }

        }
        else ResetInteraction();
    }

    private void ResetInteraction(bool noInteraction = true)
    {
        interactionUI.SetActive(false);
        interactionText.text = "";

        if (interactable == null || !noInteraction) return;

        interactable.OnEndHover(player);
        interactable.OutlineHoverEnd();
        interactable = null;
    }
}
