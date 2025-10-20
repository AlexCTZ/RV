using UnityEngine.XR.Interaction.Toolkit;

public class HoverOnlyInteractable : XRGrabInteractable
{
    public bool isLocked = false;
    public override bool IsSelectableBy(IXRSelectInteractor interactor)
    {
        if (isLocked)
        {
            return false;
        }
        return base.IsSelectableBy(interactor);
    }
}