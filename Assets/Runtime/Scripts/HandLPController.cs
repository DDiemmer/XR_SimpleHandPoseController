using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using InputManager;
using NaughtyAttributes;

namespace UserController
{
    public class HandLPController : MonoBehaviour
    {
        public Animator anim;
        [Required]
        public AxisHandler triggerHandler = null;
        [Required]
        public ButtonHandler ThumbTouch = null;
        [Required]
        public ButtonHandler GripTouch = null;
        [Required]
        public ButtonHandler TriggerTouch = null;

        private IInteractableCustom interactable;

        private bool isLeftHand = false;

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
                xRBaseController.onHoverExit.AddListener((XRBaseInteractor) => { if (!isOnSelectedEvent) { isOnInteractableEvent = false; OnHoverExit(); } });
                xRBaseController.onSelectEnter.AddListener((XRBaseInteractor) => { isOnInteractableEvent = true; isOnSelectedEvent = true; OnSelectedEnter(XRBaseInteractor); });
                xRBaseController.onSelectExit.AddListener((XRBaseInteractor) => { isOnSelectedEvent = false; OnHoverExit(); });
            }

            XRController rController = GetComponentInParent<XRController>();
            if (rController != null)
                isLeftHand = rController.controllerNode == UnityEngine.XR.XRNode.LeftHand;

            GripTouch.OnButtonDown += GripButtonDown;
            GripTouch.OnButtonUp += GripButtonUp;

            triggerHandler.OnValueChange += OnTrigger;

            ThumbTouch.OnButtonDown += ThumbButtonDown;
            ThumbTouch.OnButtonUp += ThumbButtonUp;

            TriggerTouch.OnButtonDown += TriggerTouchButtonDown;
            TriggerTouch.OnButtonUp += TriggerTouchButtonUp;

            interactable = GetComponentInParent<IInteractableCustom>();
        }
        private void OnDestroy()
	    {   
		    GripTouch.OnButtonDown -= GripButtonDown;
		    GripTouch.OnButtonUp -= GripButtonUp;
		    triggerHandler.OnValueChange -= OnTrigger;
		    ThumbTouch.OnButtonDown -= ThumbButtonDown;
		    ThumbTouch.OnButtonUp -= ThumbButtonUp;
		    TriggerTouch.OnButtonDown -= TriggerTouchButtonDown;
		    TriggerTouch.OnButtonUp -= TriggerTouchButtonUp;
	    }
        private void OnHoverExit()
        {
            if (interactable != null)
            {
                //change attach to finger attach position 
                interactable.UpdateAttachTransform(interactable.GetDefaultAttach());
            }
        }
        private void OnSelectedEnter(XRBaseInteractable xRBaseInteractor)
        {
            if (xRBaseInteractor.GetType() == typeof(XRGrabInteractionCustom))
            {
                grabType = (xRBaseInteractor as XRGrabInteractionCustom).grabbingType;
                animateGrabFrame = (xRBaseInteractor as XRGrabInteractionCustom).animateFrame;
                (xRBaseInteractor as XRGrabInteractionCustom).SetHandDiffs(isLeftHand);
                if ((xRBaseInteractor as XRGrabInteractionCustom).debug)
                {
                    (xRBaseInteractor as XRGrabInteractionCustom).SetDebugHand(isLeftHand);
                }
            }
            else if (xRBaseInteractor.GetType() == typeof(XRGrabInteractableManyPosesCustom))
            {
                //todo: get the collision position
                Vector3 handPos = this.transform.position;
                XRGrabInteractableManyPosesCustom grabInt = (xRBaseInteractor as XRGrabInteractableManyPosesCustom);
                grabInt.SetHandDiffs(isLeftHand, handPos);
                grabType = grabInt.grabbingType;
                animateGrabFrame = grabInt.animateFrame;
                if (grabType == GrabbingType.SimpleFingerTip && interactable != null)
                {
                    //change attach to finger attach position 
                    interactable.UpdateAttachTransform(interactable.GetFingertipAttach());
                }
            }
            else
            {
                XRSimpleGrabPresets xRSimpleGrab = xRBaseInteractor.gameObject.GetComponent<XRSimpleGrabPresets>();
                if (xRSimpleGrab != null)
                {
                    grabType = xRSimpleGrab.grabbingType;
                    animateGrabFrame = xRSimpleGrab.animateFrame;
                }
                else
                    grabType = GrabbingType.None; animateGrabFrame = 0f;
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
                case GrabbingType.SimpleFingerTip:
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
