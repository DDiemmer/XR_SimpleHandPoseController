using NaughtyAttributes;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
using UnityEngine.XR.Interaction.Toolkit;

namespace UserController
{
    public class XRGrabInteractionCustom : XRGrabInteractable
    {
        public GameObject prefabGrabHandController;
        [ReadOnly]
        public GameObject handControlerSimulate;
        private HandControllerSimulate controllerSimulate;
        public GrabbingType grabbingType = GrabbingType.None;
        [Range(0.0025f, 1f)]
        public float animateFrame = 1f;
        public Transform handAttachPoint;
        [ReadOnly]
        public Vector3 attachRelativePositionLH;
        [ReadOnly]
        public Vector3 attachRelativeRotationLH;
        [ReadOnly]
        public Vector3 attachRelativePositionRH;
        [ReadOnly]
        public Vector3 attachRelativeRotationRH;

        public bool leftHand = true;
        [Range(-180f, 180f)]
        public float rotateX = 0f;
        [Range(-180f, 180f)]
        public float rotateY = 0f;
        [Range(-180f, 180f)]
        public float rotateZ = 0f;

        public bool rightHandFixerAxisX = false;
        public bool rightHandFixerAxisY = true;
        public bool rightHandFixerAxisZ = false;

        public Vector3 offSetGraspPosition = Vector3.zero;
        public Vector3 offSetGraspPositionRH = Vector3.zero;
        private GameObject attachPointLH;
        private GameObject attachPointRH;
        [ReadOnly]
        public Vector3 originalLocalPoint;

        public bool isReady = false;
        //Todo: Debug have issues on some rotation 
        [Header("Caution !its  have some issues with some rotations")]
        public bool debug = true;

        private GameObject handDebug;

        private void Start()
        {
            if (attachRelativePositionLH != null && attachRelativePositionLH != Vector3.zero)
            {
                if (handControlerSimulate != null)
                {
                    if (!isReady && debug)
                    {
                        handControlerSimulate.SetActive(true);
                        GameObject handClone = Instantiate(handControlerSimulate);
                        handClone.transform.SetParent(transform, true);
                        handControlerSimulate.SetActive(false);
                        handDebug = handClone;
                    }
                    else if (isReady)
                    {
                        Destroy(handControlerSimulate);
                    }
                }
                attachPointLH = CreateAttachPoint(attachRelativePositionLH, attachRelativeRotationLH, "attachPointLeftH", this.transform);
                attachTransform = attachPointLH.transform;
            }

            attachPointRH = CreateAttachPoint(attachRelativePositionRH, attachRelativeRotationRH, "attachPointRightH", this.transform);
        }

        public GameObject CreateAttachPoint(Vector3 attachRelativePosition, Vector3 attachRelativeRotation, string objName, Transform parent)
        {
            GameObject attachPoint;
            if (attachRelativePosition != null && attachRelativePosition != Vector3.zero)
            {
                attachPoint = Instantiate(new GameObject(), parent, false);
                attachPoint.name = objName;
                if (attachRelativeRotation != null)
                {
                    attachPoint.transform.rotation = Quaternion.Euler(attachRelativeRotation);
                }
                attachPoint.transform.position += (attachRelativePosition);
                return attachPoint;
            }
            return null;
        }

        public void SetDebugHand(bool _leftHand)
        {
            if (_leftHand == handDebug.transform.localScale.z > 0)
                return;

            handDebug.transform.localScale = new Vector3(handDebug.transform.localScale.x, handDebug.transform.localScale.y, handDebug.transform.localScale.z * -1);
            Vector3 angle = handDebug.transform.localRotation.eulerAngles;
            handDebug.transform.localRotation = Quaternion.Euler(rightHandFixerAxisX ? angle.x + 180 : rotateX, rightHandFixerAxisY ? angle.y + 180 : rotateY, rightHandFixerAxisZ ? angle.z + 180 : rotateZ);
        }

        public void SetHandDiffs(bool _leftHand)
        {
            if (_leftHand)
                attachTransform = attachPointLH.transform;
            else
                attachTransform = attachPointRH.transform;
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            Debug.LogWarning("Deprecated");
            if (Application.isPlaying || isReady)
            {
                if (handControlerSimulate != null)
                    handControlerSimulate.SetActive(false);
                return;
            }
            if (handControlerSimulate != null)
                handControlerSimulate.SetActive(true);

            if (prefabGrabHandController == null)
                prefabGrabHandController = HandSimulatorPrefabs.Instance.LeftHandSimulator;

            if (prefabGrabHandController != null && handControlerSimulate == null)
            {
                handControlerSimulate = Instantiate(prefabGrabHandController);
                //handControlerSimulate.hideFlags = HideFlags.HideInHierarchy;
                handControlerSimulate.transform.localScale = new Vector3(handControlerSimulate.transform.localScale.x, handControlerSimulate.transform.localScale.y, handControlerSimulate.transform.localScale.z * (leftHand ? 1 : -1));
                controllerSimulate = handControlerSimulate.GetComponent<HandControllerSimulate>();
                handControlerSimulate.transform.position = transform.position;
            }
            else if (handControlerSimulate != null)
            {
                controllerSimulate = handControlerSimulate.GetComponent<HandControllerSimulate>();
                handControlerSimulate.transform.SetParent(null);
            }
            if (controllerSimulate != null)
            {
                controllerSimulate.SetVariables(grabbingType, animateFrame);
                // UpdatePosition();
            }
        }

        [Button]
        public void NextGraspPoint()
        {
            if (controllerSimulate != null)
            {
                if (handAttachPoint != null)
                    handAttachPoint.localPosition = originalLocalPoint;

                handAttachPoint = controllerSimulate.GetNextGraspPoint();
                originalLocalPoint = handAttachPoint.localPosition;
            }

            UpdatePosition();
        }
        [Button]
        public void UpdatePosition()
        {
            if (Application.isPlaying || isReady)
                return;
            if (handControlerSimulate != null)
                handControlerSimulate.SetActive(true);

            if (handAttachPoint == null)
            {
                NextGraspPoint();
                return;
            }

            handAttachPoint.localPosition = originalLocalPoint + offSetGraspPosition;
            handControlerSimulate.transform.SetParent(null);
            handControlerSimulate.transform.localScale = new Vector3(0.85f, 0.85f, 0.85f);
            handControlerSimulate.transform.rotation = Quaternion.Euler(Vector3.zero);
            // relatives values
            Vector3 dir = (handControlerSimulate.transform.position - handAttachPoint.position) + offSetGraspPosition;

            Vector3 rotationA = new Vector3(rotateX, rotateY, rotateZ);
            attachRelativeRotationLH = rotationA;
            handControlerSimulate.transform.rotation = Quaternion.Euler(rotationA);

            dir = (handControlerSimulate.transform.position - handAttachPoint.position);
            attachRelativePositionLH = dir;
            //to right hand attach
            Vector3 offsetFixAxis = offSetGraspPositionRH;
            offsetFixAxis.y = offsetFixAxis.y * -1;
            Vector3 dirToRH = (handControlerSimulate.transform.position - (handAttachPoint.position + offsetFixAxis));
            dirToRH.y = dirToRH.y * -1;
            attachRelativePositionRH = -dirToRH;
            attachRelativeRotationRH = new Vector3(rightHandFixerAxisX ? rotateX + 180 : rotateX, rightHandFixerAxisY ? rotateY + 180 : rotateY, rightHandFixerAxisZ ? rotateZ + 180 : rotateZ);

            handControlerSimulate.transform.position = transform.position + dir;
            handControlerSimulate.transform.localScale = new Vector3(handControlerSimulate.transform.localScale.x, handControlerSimulate.transform.localScale.y, handControlerSimulate.transform.localScale.z * (leftHand ? 1 : -1));
            if (!leftHand)
            {
                dir = (handControlerSimulate.transform.position - (handAttachPoint.position + offSetGraspPositionRH));
                dir.y = dir.y * -1;
                handControlerSimulate.transform.position = transform.position - dir;// - new Vector3(0, 0, offSetGraspPosition.z * 2);
                handControlerSimulate.transform.rotation = Quaternion.Euler(rightHandFixerAxisX ? rotateX + 180 : rotateX, rightHandFixerAxisY ? rotateY + 180 : rotateY, rightHandFixerAxisZ ? rotateZ + 180 : rotateZ);
            }
        }
#endif
    }
}
