using NaughtyAttributes;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
using UnityEngine.XR.Interaction.Toolkit;

namespace UserController
{
    public class XRGrabInteractableManyPosesCustom : XRGrabInteractable
    {
        public bool autoFindCollisor = false;
        public List<HandGrabPose> handGrabPoses;
        public GrabbingType grabbingType = GrabbingType.None;
        public bool positionOfParent = true;
        [Range(0.0025f, 1f)]
        public float animateFrame = 1f;

        private void Start()
        {
            if (autoFindCollisor)
            {
                handGrabPoses = new List<HandGrabPose>();
                handGrabPoses = new List<HandGrabPose>(GetComponentsInChildren<HandGrabPose>());
            }
        }
        public void SetHandDiffs(bool _leftHand, Vector3 handPosition)
        {
            HandGrabPose closestHandPose = null;
            attachTransform = this.gameObject.transform;
            float lastDist = float.MaxValue;
            //get closest distance from hand 
            foreach (HandGrabPose item in handGrabPoses.FindAll(p => p.leftHand == _leftHand))
            {
                Vector3 positionCompair = positionOfParent ? item.gameObject.transform.parent.position : item.gameObject.transform.position;
                item.SetDebugHand(false);
                float dist = Vector3.Distance(handPosition, positionCompair);
                if (dist < lastDist)
                {
                    closestHandPose = item;
                    lastDist = dist;
                }
            }
            if (closestHandPose != null)
            {
                attachTransform = closestHandPose.GetAttachPoint();
                grabbingType = closestHandPose.grabbingType;
                animateFrame = closestHandPose.animateFrame;
                closestHandPose.SetDebugHand(true);
            }
            else
            {
                attachTransform = this.gameObject.transform;
                grabbingType = grabbingType != GrabbingType.SimpleFingerTip ? GrabbingType.None : grabbingType;
                animateFrame = 0f;
            }
        }
    }
}