using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class XRGrabInteractablePreventDoubleGrab : XRGrabInteractable
{
    public override bool IsSelectableBy(IXRSelectInteractor interactor)
    {
        // If already selected by someone else, block the new interactor
        if (isSelected && interactor != firstInteractorSelecting)
            return false;

        return base.IsSelectableBy(interactor);
    }
}