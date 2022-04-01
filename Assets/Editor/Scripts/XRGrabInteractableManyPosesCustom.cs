using NaughtyAttributes;
using System.Collections;
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

		private bool isOnInteraction = false;
		private List<HandLPController> lPControllers;
		HandGrabPose lastHandPose;

		private void Start()
		{
			if (autoFindCollisor)
			{
				handGrabPoses = new List<HandGrabPose>();
				handGrabPoses = new List<HandGrabPose>(GetComponentsInChildren<HandGrabPose>());
			}
			lPControllers = new List<HandLPController>();
			onHoverEntered.AddListener(OnEnterHandInteraction);
			onHoverExited.AddListener(OnExitHandInteraction);

		}

		protected override void OnSelectEntering(SelectEnterEventArgs args)
		{
			base.OnSelectEntering(args);

			HandLPController hand = args.interactor.gameObject.transform.GetComponentInChildren<HandLPController>();
			if (hand)
			{
				SetHandDiffs(hand.isLeftHand, hand.transform.position);
			}
		}

		protected override void OnSelectExiting(SelectExitEventArgs args)
		{
			base.OnSelectExiting(args);
			StartCoroutine(UpdateHandInteractions());
		}

		private void OnEnterHandInteraction(XRBaseInteractor xRBaseInteractor)
		{
			HandLPController hand = xRBaseInteractor.gameObject.transform.GetComponentInChildren<HandLPController>();
			if (hand)
			{
				isOnInteraction = true;
				lPControllers.Add(hand);
				//SetHandDiffs(hand.isLeftHand, xRBaseInteractor.transform.position);
				if (lPControllers.Count == 1)
					StartCoroutine(UpdateHandInteractions());
			}
		}

		private void OnExitHandInteraction(XRBaseInteractor xRBaseInteractor)
		{
			HandLPController hand = xRBaseInteractor.gameObject.transform.GetComponentInChildren<HandLPController>();
			if (hand)
			{
				foreach (HandGrabPose handGrabPose in handGrabPoses.FindAll(p => p.leftHand == hand.isLeftHand))
				{
					handGrabPose.SetDebugHand(false);
				}
				lPControllers.Remove(hand);
			}
			isOnInteraction = lPControllers.Count == 0;
			StopCoroutine(UpdateHandInteractions());
		}

		private IEnumerator UpdateHandInteractions()
		{
			if (isSelected)
			{
				foreach (HandGrabPose handGrabPose in handGrabPoses)
				{
					handGrabPose.SetDebugHand(false);
				}
			}
			else
			{

				foreach (var lPcontroller in lPControllers)
				{
					foreach (HandGrabPose handGrabPose in handGrabPoses.FindAll(p => p.leftHand == lPcontroller.isLeftHand))
					{
						GetAndActiveClosestHand(lPcontroller.isLeftHand, lPcontroller.transform.position);
					}
				}
				yield return new WaitForSeconds(0.3f);

				if (isOnInteraction && lPControllers.Count > 0)
					StartCoroutine(UpdateHandInteractions());
			}
			yield return null;
		}

		public void SetHandDiffs(bool _leftHand, Vector3 handPosition)
		{
			//if (this.isSelected)
			//	return;
			attachTransform = this.gameObject.transform;
			HandGrabPose closestHandPose = GetAndActiveClosestHand(_leftHand, handPosition);
			lastHandPose = closestHandPose;
			if (closestHandPose != null)
			{
				attachTransform = closestHandPose.GetAttachPoint();
				grabbingType = closestHandPose.grabbingType;
				animateFrame = closestHandPose.animateFrame;
			}
			else
			{
				attachTransform = this.gameObject.transform;
				grabbingType = grabbingType != GrabbingType.SimpleFingerTip ? GrabbingType.None : grabbingType;
				animateFrame = 0f;
			}
		}

		private HandGrabPose GetAndActiveClosestHand(bool _leftHand, Vector3 handPosition)
		{
			HandGrabPose closestHandPose = null;
			attachTransform = this.gameObject.transform;
			float lastDist = float.MaxValue;
			//get closest distance from hand 
			foreach (HandGrabPose item in handGrabPoses.FindAll(p => p.leftHand == _leftHand))
			{
				Vector3 positionCompair = positionOfParent ? item.gameObject.transform.parent.position : item.gameObject.transform.position;
				item.SetDebugHand(false);
				positionCompair += item.GetRelativeAttachPosition();

				float dist = Vector3.Distance(handPosition, positionCompair);
				if (dist < lastDist)
				{
					closestHandPose = item;
					lastDist = dist;
				}
			}
			if (closestHandPose)
				closestHandPose.SetDebugHand(true);

			return closestHandPose;
		}
	}
}