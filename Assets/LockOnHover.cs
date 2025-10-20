using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.InputSystem;
using Unity.VisualScripting;

[RequireComponent(typeof(XRGrabInteractable))]
public class LockableGrabbable : MonoBehaviour
{
    private HoverOnlyInteractable grabInteractable;
    private bool isLocked = false;
    private bool isHovered = false;
    private Rigidbody rigidbody;

    [Header("Input")]
    [Tooltip("Action Input System (ex: bouton A ou X)")]
    public InputActionReference toggleLockAction;

    

    private void Awake()
    {
        rigidbody = GetComponent<Rigidbody>();
        grabInteractable = GetComponent<HoverOnlyInteractable>();

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
            grabInteractable.isLocked = true;
            
        }
        else
        {
            grabInteractable.isLocked = false;

        }

        Debug.Log($"{gameObject.name} → {(isLocked ? "Verrouillé" : "Déverrouillé")}");
    }
}
