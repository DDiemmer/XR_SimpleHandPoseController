
using UnityEngine;

public interface IInteractableCustom
{
    Transform GetDefaultAttach();
    Transform GetFingertipAttach();
    void UpdateAttachTransform(Transform attachReference);

}
