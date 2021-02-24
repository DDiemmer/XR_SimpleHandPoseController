using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class XRDirectInterationAttachCustom : XRDirectInteractor
{
    public Transform defaultAttach;
    public Transform fingerTipTransform;

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
