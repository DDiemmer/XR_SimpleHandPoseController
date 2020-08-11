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
        public bool leftHand = true;
        public GrabbingType grabbingType = GrabbingType.None;
        [Range(0.0025f, 1f)]
        public float animateFrame = 1f;
        public Transform handAttachPoint;
        [ReadOnly]
        public Vector3 attachRelativePosition;
        [ReadOnly]
        public Vector3 attachRelativeRotation;
        [Range(-180f, 180f)]
        public float rotateX = 0f;
        [Range(-180f, 180f)]
        public float rotateY = 0f;
        [Range(-180f, 180f)]
        public float rotateZ = 0f;
        //todo:  create booleans to choose rotate for reverse hand ... 


        public Vector3 offSetGraspPosition = Vector3.zero;
        private GameObject attachPoint;
        [ReadOnly]
        public Vector3 originalLocalPoint;

        public bool isReady = false;
        public bool Debug = true;
        private void Start()
        {
            if (attachRelativePosition != null && attachRelativePosition != Vector3.zero)
            {
                if (handControlerSimulate != null)
                {
                    if (!isReady && Debug)
                    {
                        GameObject handClone = Instantiate(handControlerSimulate);
                        handClone.transform.SetParent(transform, true);
                        handControlerSimulate.SetActive(false);
                    }
                }
                attachPoint = Instantiate(new GameObject(), this.transform, false);
                attachPoint.name = "attachPoint";
                if (attachRelativeRotation != null)
                {
                    attachPoint.transform.rotation = Quaternion.Euler(attachRelativeRotation);
                }
                attachPoint.transform.position += (attachRelativePosition);
                attachTransform = attachPoint.transform;
            }
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (Application.isPlaying || isReady)
            {
                handControlerSimulate.SetActive(false);
                return;
            }
            handControlerSimulate.SetActive(true);

            if (prefabGrabHandController == null)
                prefabGrabHandController = (GameObject)AssetDatabase.LoadAssetAtPath("Assets/UserController/Prefabs/HandSimulateControllAnimation.prefab", typeof(GameObject));

            if (prefabGrabHandController != null && handControlerSimulate == null)
            {
                handControlerSimulate = Instantiate(prefabGrabHandController);
                handControlerSimulate.hideFlags = HideFlags.HideInHierarchy;
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
                handControlerSimulate.transform.position = transform.position + offSetGraspPosition;
                return;
            }

            handAttachPoint.localPosition = originalLocalPoint + offSetGraspPosition;
            handControlerSimulate.transform.SetParent(null);
            handControlerSimulate.transform.localScale = new Vector3(0.85f, 0.85f, 0.85f);
            handControlerSimulate.transform.rotation = Quaternion.Euler(Vector3.zero);
            // relatives values
            Vector3 dir = (handControlerSimulate.transform.position - handAttachPoint.position) + offSetGraspPosition;

            Vector3 rotationA = new Vector3(rotateX, rotateY, rotateZ);
            attachRelativeRotation = rotationA;
            handControlerSimulate.transform.rotation = Quaternion.Euler(rotationA);

            dir = (handControlerSimulate.transform.position - handAttachPoint.position);
            attachRelativePosition = dir;
            handControlerSimulate.transform.position = transform.position + dir;
            handControlerSimulate.transform.localScale = new Vector3(handControlerSimulate.transform.localScale.x, handControlerSimulate.transform.localScale.y, handControlerSimulate.transform.localScale.z * (leftHand ? 1 : -1));
            if (!leftHand)
            {
                dir = (handControlerSimulate.transform.position - handAttachPoint.position);
                handControlerSimulate.transform.position = transform.position + dir;
            }
        }
#endif
    }
}