using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using InputManager;
using System.Runtime.CompilerServices;

namespace UserController
{
    public class HandLPController : MonoBehaviour
    {
        public Animator anim;

        public AxisHandler triggerHandler = null;
        public ButtonHandler ThumbTouch = null;
        public ButtonHandler GripTouch = null;
        public ButtonHandler TriggerTouch = null;

        private XRBaseControllerInteractor xRBaseController;
        private bool isOnInteractableEvent = false;
        private bool isOnSelectedEvent = false;
        private GrabbingType grabType = GrabbingType.HandGrab;
        private float animateGrabFrame;
        // Start is called before the first frame update
        void Start()
        {
            if (anim == null)
                anim = GetComponent<Animator>();

            xRBaseController = GetComponentInParent<XRBaseControllerInteractor>();
            if (xRBaseController != null)
            {
                xRBaseController.onHoverEnter.AddListener((XRBaseInteractor) => { isOnInteractableEvent = true; OnSelectedEnter(XRBaseInteractor); });
                xRBaseController.onHoverExit.AddListener((XRBaseInteractor) => { if (!isOnSelectedEvent) isOnInteractableEvent = false; });
                xRBaseController.onSelectEnter.AddListener((XRBaseInteractor) => { isOnInteractableEvent = true; isOnSelectedEvent = true; OnSelectedEnter(XRBaseInteractor); });
                xRBaseController.onSelectExit.AddListener((XRBaseInteractor) => { isOnSelectedEvent = false; OnSelectedEnter(XRBaseInteractor); });
            }

            triggerHandler.OnValueChange += OnTrigger;

            ThumbTouch.OnButtonDown += ThumbButtonDown;
            ThumbTouch.OnButtonUp += ThumbButtonUp;

            GripTouch.OnButtonDown += GripButtonDown;
            GripTouch.OnButtonUp += GripButtonUp;

            TriggerTouch.OnButtonDown += TriggerTouchButtonDown;
            TriggerTouch.OnButtonUp += TriggerTouchButtonUp;

        }

        private void OnSelectedEnter(XRBaseInteractable xRBaseInteractor)
        {
            var go = xRBaseInteractor.gameObject;
            if (xRBaseInteractor.GetType() == typeof(XRGrabInteractionCustom))
            {
                grabType = (xRBaseInteractor as XRGrabInteractionCustom).grabbingType;
                animateGrabFrame = (xRBaseInteractor as XRGrabInteractionCustom).animateFrame;
            }
        }

        private void ThumbButtonDown(XRController controller)
        {
            anim.SetBool("ThumbTouch", true);
        }
        private void ThumbButtonUp(XRController controller)
        {
            anim.SetBool("ThumbTouch", false);
        }
        private void GripButtonDown(XRController controller)
        {
            anim.SetBool("Grip", true);
        }
        private void GripButtonUp(XRController controller)
        {
            anim.SetBool("Grip", false);
        }
        private void TriggerTouchButtonDown(XRController controller)
        {
            if (!isOnInteractableEvent)
            {
                anim.SetBool("TriggerTouch", true);
                anim.SetFloat("TriggerFingerGrab", 0f);
                anim.SetFloat("TriggerHandGrab", 0f);
            }
        }
        private void TriggerTouchButtonUp(XRController controller)
        {
            anim.SetBool("TriggerTouch", false);
            anim.SetFloat("TriggerFingerGrab", 0f);
            anim.SetFloat("TriggerHandGrab", 0f);
        }
        private void OnTrigger(XRController controller, float value)
        {
            if (!isOnInteractableEvent && !isOnSelectedEvent && value > 0.002f)
                return;

            float maximumValue = animateGrabFrame;
            float normValue = Mathf.Clamp(value, 0f, maximumValue);

            switch (grabType)
            {
                case GrabbingType.None:
                    anim.SetBool("TriggerTouch", false);
                    anim.SetFloat("TriggerHandGrab", 0f);
                    anim.SetFloat("TriggerFingerGrab", 0f);
                    break;
                case GrabbingType.FingerGrab:
                    anim.SetBool("TriggerTouch", false);
                    anim.SetFloat("TriggerHandGrab", 0f);
                    anim.SetFloat("TriggerFingerGrab", normValue);
                    break;
                case GrabbingType.HandGrab:
                    anim.SetBool("TriggerTouch", false);
                    anim.SetFloat("TriggerFingerGrab", 0f);
                    anim.SetFloat("TriggerHandGrab", normValue);
                    break;

            }
        }

    }
}