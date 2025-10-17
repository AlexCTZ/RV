using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.InputSystem;

[RequireComponent(typeof(XRGrabInteractable))]
public class LockableGrabbable : MonoBehaviour
{
    private XRGrabInteractable grabInteractable;
    private bool isLocked = false;
    private bool isHovered = false;

    [Header("Input")]
    [Tooltip("Action Input System (ex: bouton A ou X)")]
    public InputActionReference toggleLockAction;

    [Header("Layers")]
    [Tooltip("Layer autorisé quand l’objet est déverrouillé")]
    public string unlockedLayer = "Default";
    public string noneLayer = "Nothing";

    private void Awake()
    {
        grabInteractable = GetComponent<XRGrabInteractable>();

        // On écoute les événements de survol
        grabInteractable.hoverEntered.AddListener(OnHoverEnter);
        grabInteractable.hoverExited.AddListener(OnHoverExit);
    }

    private void OnEnable()
    {
        if (toggleLockAction != null)
            toggleLockAction.action.performed += OnToggleLockPressed;
    }

    private void OnDisable()
    {
        if (toggleLockAction != null)
            toggleLockAction.action.performed -= OnToggleLockPressed;
    }

    private void OnDestroy()
    {
        grabInteractable.hoverEntered.RemoveListener(OnHoverEnter);
        grabInteractable.hoverExited.RemoveListener(OnHoverExit);
    }

    private void OnHoverEnter(HoverEnterEventArgs args)
    {
        isHovered = true;
    }

    private void OnHoverExit(HoverExitEventArgs args)
    {
        isHovered = false;
    }

    private void OnToggleLockPressed(InputAction.CallbackContext ctx)
    {
        Debug.Log("input activé");
        if (!isHovered) return; // On toggle seulement si on est en hover

        isLocked = !isLocked;

        if (isLocked)
        {
            // Retire toutes les couches d’interaction (impossible à grab)
            grabInteractable.interactionLayers = InteractionLayerMask.GetMask(noneLayer);
        }
        else
        {
            // Restaure la couche normale
            grabInteractable.interactionLayers = InteractionLayerMask.GetMask(unlockedLayer);
        }

        Debug.Log($"{gameObject.name} → {(isLocked ? "Verrouillé" : "Déverrouillé")}");
    }
}
