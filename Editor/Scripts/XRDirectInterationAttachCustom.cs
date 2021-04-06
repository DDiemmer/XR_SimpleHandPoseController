using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class XRDirectInterationAttachCustom : XRDirectInteractor, IInteractableCustom
{
    public Transform defaultAttach;
    public Transform fingerTipTransform;

    public Transform GetDefaultAttach()
    {
        return defaultAttach;
    }

    public Transform GetFingertipAttach()
    {
        return fingerTipTransform;
    }

    public void UpdateAttachTransform(Transform attachReference)
    {
        if (attachTransform != null)
        {
            attachTransform.position = attachReference.position;
            attachTransform.rotation = attachReference.rotation;
            attachTransform.localScale = attachReference.localScale;
        }
    }
}