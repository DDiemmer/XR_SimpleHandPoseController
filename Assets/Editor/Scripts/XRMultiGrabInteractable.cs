using Support;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UserController;

public class XRMultiGrabInteractable : XRGrabInteractableManyPosesCustom
{
	/// <summary>
	/// The <see cref="XRBaseInteractable"/> on the subobject that will simulate the second grab point.
	/// </summary>
	public XRBaseInteractable secondGrabInteractable;
	/// <summary>
	/// Is the rotation will be guided by the second hand ? 
	/// </summary>
	public bool allowRotation = true;
	/// <summary>
	/// Is the attachment point in the same position as the contact point? 
	/// </summary>
	public bool usingOffsetGrab = false;
	/// <summary>
	/// Is the attachment point in the same rotation as the hand on the contact ? 
	/// </summary>
	public bool usingOffSetRotation = false;
	/// <summary>
	/// Stores the local position of the attachment position of the first <see cref="XRBaseInteractor"/>. 
	/// </summary>
	private Vector3 interactorlocalPosition = Vector3.zero;
	/// <summary>
	/// Stores the position of the attachment position of the first <see cref="XRBaseInteractor"/>. 
	/// </summary>
	private Vector3 interactorPosition = Vector3.zero;
	/// <summary>
	/// Stores the local rotation of the attachment position of the first <see cref="XRBaseInteractor"/>. 
	/// </summary>
	private Quaternion interactorlocalRotation = Quaternion.identity;
	/// <summary>
	/// Stores the position of the attachment position of the second <see cref="XRBaseInteractor"/>. 
	/// </summary>
	private Vector3 secondInteractorPosition;
	/// <summary>
	/// A <see cref="Transform"/> to set the second grab position as guide direction to use when scale object while keep relative attach position.
	/// </summary>
	public Transform graspSecAttachPositionGuide;

	/// <summary>
	/// Stores the <see cref="XRBaseInteractor"/> of the second interactor.
	/// </summary>
	private XRBaseInteractor secondInteractor;
	/// <summary>
	/// Stores the <see cref="XRBaseInteractor"/> of the first interactor.
	/// </summary>
	private XRBaseInteractor firstInteractor;
	/// <summary>
	/// Stores the initial attach rotation.
	/// </summary>
	private Quaternion initialAttachRotation;
	/// <summary>
	/// Is second hand is taking the object.
	/// </summary>
	private bool isSecondhandGrab = false;
	/// <summary>
	/// A <see cref="Transform"/> to set the grab position as guide direction to use when scale object while keep relative attach position.
	/// </summary>
	public Transform graspAttachPositionGuide;

	/// <summary>
	/// Is the second grab interactor a distance grab? 
	/// </summary>
	private bool isDistanceSecondGrab = false;
	/// <summary>
	/// Stores the direction of the ray when the second grab is grabbed by distance.
	/// </summary>
	private Transform secondRayFixedAttachmentPoint;

	//scale variables
	#region Scale Variables
	/// <summary>
	/// Is the object will be scaled when grabbed by two hands? 
	/// </summary>
	public bool allowRescale = true;
	/// <summary>
	/// How many times the object can be scaled from their original size.
	/// </summary>
	public float maxMagnitudeScale = 10f;
	/// <summary>
	/// Stores the original size of the object.
	/// </summary>
	public Vector3 originalSize = Vector3.one;
	/// <summary>
	/// Stores the size of the object after the last released grab.
	/// </summary>
	protected Vector3 lastGrabSize = Vector3.zero;
	/// <summary>
	/// The start distance between the hands.
	/// </summary>
	private float startDistance;
	#endregion

	/// <summary>
	/// <inheritdoc/>
	/// </summary>
	private void Start()
	{
		secondGrabInteractable.selectEntered.AddListener(OnSecondHandGrab);
		secondGrabInteractable.selectExited.AddListener(OnSecondHandRelease);
		secondGrabInteractable.gameObject.transform.ResetLocalTransform();
		//should be smaller than main
		secondGrabInteractable.transform.localScale = Vector3.one * 0.98f;
		secondGrabInteractable.enabled = false;

		originalSize = transform.localScale;

		Helper.SetDetectCollisions(secondGrabInteractable.gameObject, false, false);
		secondGrabInteractable.colliders.ForEach(x => x.enabled = false);
	}
	/// <summary>
	/// Changes the second <see cref="XRBaseInteractable"/>.
	/// </summary>
	/// <param name="_secondGrabInteractable">The second <see cref = "XRBaseInteractable" /> to replace the current one.</param>
	public void ChangeSecondInteractable(XRBaseInteractable _secondGrabInteractable)
	{

		secondGrabInteractable.selectEntered.RemoveAllListeners();
		secondGrabInteractable.selectExited.RemoveAllListeners();

		secondGrabInteractable = _secondGrabInteractable;
		secondGrabInteractable.selectEntered.AddListener(OnSecondHandGrab);
		secondGrabInteractable.selectExited.AddListener(OnSecondHandRelease);

		Helper.SetDetectCollisions(secondGrabInteractable.gameObject, true, false);
		secondGrabInteractable.colliders.ForEach(x => x.enabled = false);
		secondGrabInteractable.enabled = false;
	}
	/// <summary>
	/// <inheritdoc/>
	/// </summary>
	/// <param name="args"><inheritdoc/></param>
	protected override void OnSelectEntered(SelectEnterEventArgs args)
	{
		base.OnSelectEntered(args);

		secondGrabInteractable.enabled = true;
		Helper.SetDetectCollisions(secondGrabInteractable.gameObject, true, false);

		secondGrabInteractable.colliders.ForEach(x => x.enabled = true);
		initialAttachRotation = args.interactor.attachTransform.localRotation;
		firstInteractor = args.interactor;
		StoreInteractor(args.interactor);
		if (usingOffsetGrab || usingOffSetRotation)
			MatchAttachment();
	}
	/// <summary>
	/// <inheritdoc/>
	/// </summary>
	/// <param name="args"><inheritdoc/></param>
	protected override void OnSelectExiting(SelectExitEventArgs args)
	{
		base.OnSelectExiting(args);

		if (allowRotation)
			args.interactor.attachTransform.localRotation = initialAttachRotation;

		ResetAttachmentPoint(args.interactor);
		ClearInteractor();

		secondInteractor = null;
		isSecondhandGrab = false;
		secondGrabInteractable.enabled = false;
		Helper.SetDetectCollisions(secondGrabInteractable.gameObject, false, false);
		secondGrabInteractable.colliders.ForEach(x => x.enabled = false);
	}
	/// <summary>
	/// The method is called when the second interactor grab the object.
	/// </summary>
	/// <param name="args">The <see cref="SelectEnterEventArgs"/> passed by event.</param>
	public void OnSecondHandGrab(SelectEnterEventArgs args)
	{
		secondInteractor = args.interactor;
		isSecondhandGrab = true;
		StoreSecondInteractor(args);
	}
	/// <summary>
	/// The method is called when the second interactor released the object.
	/// </summary>
	/// <param name="args">The <see cref="SelectEnterEventArgs"/> passed by event.</param>
	public void OnSecondHandRelease(SelectExitEventArgs args)
	{
		secondGrabInteractable.gameObject.transform.ResetLocalTransform();
		//should be smaller than main
		secondGrabInteractable.transform.localScale = Vector3.one * 0.98f;
		secondInteractor = null;
		isSecondhandGrab = false;
	}
	/// <summary>
	/// <inheritdoc/>
	/// </summary>
	/// <param name="interactor"><inheritdoc/></param>
	/// <returns><inheritdoc/></returns>
	public override bool IsSelectableBy(XRBaseInteractor interactor)
	{
		bool isAlreadyGrabbed = selectingInteractor && !interactor.Equals(selectingInteractor);
		return base.IsSelectableBy(interactor) && !isAlreadyGrabbed;
	}

	/// <summary>
	/// <inheritdoc/>
	/// </summary>
	/// <param name="updatePhase"><inheritdoc/></param>
	public override void ProcessInteractable(XRInteractionUpdateOrder.UpdatePhase updatePhase)
	{
		base.ProcessInteractable(updatePhase);

		if (secondInteractor && selectingInteractor)
		{
			if (allowRotation)
			{
				//todo: it'is not so simple :/
				//selectingInteractor.attachTransform.rotation = Quaternion.LookRotation(secondInteractor.attachTransform.position - selectingInteractor.attachTransform.position, selectingInteractor.attachTransform.up);
			}
			if (allowRescale)
			{
				float handDistance = 0;
				if (!isDistanceSecondGrab)
				{
					handDistance = Vector3.Distance(secondInteractor.transform.position, selectingInteractor.transform.position);
					ScaleByGrasping(Mathf.Abs(handDistance));
				}
				else
				{
					//center 
					Vector3 cAdj = (secondRayFixedAttachmentPoint.position - transform.position);
					Vector3 cOp = (secondRayFixedAttachmentPoint.up);
					float angleFirst = Vector3.Angle(cAdj, cOp);
					float distanceGrab = (Mathf.Abs(Mathf.Tan(angleFirst * Mathf.Deg2Rad) * cAdj.magnitude));

					handDistance = distanceGrab;
					if (!(handDistance is float.NaN) && angleFirst < 90)
						ScaleByGrasping(Mathf.Abs(handDistance));
				}
			}
		}
		MatchAttachment();
	}
	/// <summary>
	/// Calculates the size of the object by the distance between the hands.
	/// </summary>
	/// <param name="handDistance">Distance between hands.</param>
	protected void ScaleByGrasping(float handDistance)
	{
		float percent = (handDistance / (startDistance));

		Vector3 newScale = (lastGrabSize * percent);

		if (newScale.magnitude <= maxMagnitudeScale)
		{
			transform.localScale = newScale;
		}
		else
		{
			transform.localScale = Vector3.one * (maxMagnitudeScale / Vector3.one.magnitude);
		}
	}
	/// <summary>
	/// Stores the main interaction variables.
	/// </summary>
	/// <param name="interactor">The <see cref="XRBaseInteractor"/> that contains the attachTransform to store.</param>
	private void StoreInteractor(XRBaseInteractor interactor)
	{
		interactorPosition = interactor.attachTransform.position;
		interactorlocalPosition = interactor.attachTransform.localPosition;
		interactorlocalRotation = interactor.attachTransform.localRotation;

		bool hasAttach = attachTransform != null;

		if (usingOffsetGrab || usingOffSetRotation)
		{
			if (usingOffSetRotation)
				graspAttachPositionGuide.rotation = hasAttach ? attachTransform.rotation : transform.rotation;
			else
			{
				graspAttachPositionGuide.rotation = firstInteractor.attachTransform.rotation;

				//todo: verify it later 
				//new UnityTask(Helper.WaitUntilEndOfFrame(() =>
				//{
				graspAttachPositionGuide.rotation = firstInteractor.attachTransform.rotation;

				//}, 5));
			}

			if (usingOffsetGrab)
			{
				graspAttachPositionGuide.position = (transform.position - (transform.position - interactorPosition));

				Vector3 position = hasAttach ? attachTransform.position : transform.position;
				firstInteractor.attachTransform.position = position + (firstInteractor.transform.position - graspAttachPositionGuide.position);
			}
			else
				graspAttachPositionGuide.position = firstInteractor.attachTransform.position;

			//todo: verify it later 
			//new UnityTask(Helper.WaitUntilEndOfFrame(() =>
			//{
			graspAttachPositionGuide.position = firstInteractor.attachTransform.position;
			//}, 5));

			firstInteractor.attachTransform.rotation = hasAttach ? attachTransform.rotation : transform.rotation;
		}
		else
		{
			//todo: verify it later 
			//new UnityTask(Helper.WaitUntilEndOfFrame(() =>
			//{
			graspAttachPositionGuide.position = firstInteractor.attachTransform.position;
				graspAttachPositionGuide.rotation = firstInteractor.attachTransform.rotation;
			//}, 5));
		}

		//todo: verify it later 
		//XRRayWithTriggerInteractor xrRay = (interactor as XRRayWithTriggerInteractor);
		//if (xrRay && xrRay.rayDistanceActive && xrRay.rayTipAttachTransform.parent == transform)
		//{
		//	graspAttachPositionGuide.position = transform.position;
		//}
	}
	/// <summary>
	/// Stores the main interaction variables of the second grab interactor.
	/// </summary>
	/// <param name="interactor">The <see cref="XRBaseInteractor"/> that contains the attachTransform to store.</param>
	private void StoreSecondInteractor(SelectEnterEventArgs args)
	{
		secondInteractorPosition = args.interactor.attachTransform.position;

		graspSecAttachPositionGuide.position = (args.interactable.transform.position - (args.interactable.transform.position - secondInteractorPosition));
		graspSecAttachPositionGuide.rotation = args.interactable.transform.rotation;

		args.interactor.attachTransform.rotation = args.interactable.transform.rotation;
		args.interactor.attachTransform.position = args.interactable.transform.position;

		startDistance = Vector3.Distance(graspAttachPositionGuide.position, graspSecAttachPositionGuide.position);
		isDistanceSecondGrab = false;
		
		lastGrabSize = transform.localScale;

		////todo: verify it later 
		//XRRayWithTriggerInteractor xrRay = (args.interactor as XRRayWithTriggerInteractor);
		//if (xrRay && xrRay.rayDistanceActive && xrRay.rayTipAttachTransform.parent == args.interactable.transform)
		//{
		//	graspSecAttachPositionGuide.position = xrRay.rayTipAttachTransform.position;
		//	secondRayFixedAttachmentPoint = xrRay.fingerTipTransform;
		//	isDistanceSecondGrab = true;
		//	startDistance = Vector3.Distance(transform.position, graspSecAttachPositionGuide.position);
		//}
	}

	/// <summary>
	/// Sets attachTransform position on the contact position.
	/// </summary>
	private void MatchAttachment()
	{
		if ((usingOffsetGrab || usingOffSetRotation) && attachTransform != null)
		{
			bool hasAttach = attachTransform != null;
			firstInteractor.attachTransform.position = attachTransform.position + (firstInteractor.transform.position - graspAttachPositionGuide.position);
		}
	}
	/// <summary>
	/// Resets the main interaction variables. 
	/// </summary>
	/// <param name="interactor">The <see cref="XRBaseInteractor"/> that contains the attachTransform to store.</param>
	private void ResetAttachmentPoint(XRBaseInteractor interactor)
	{
		interactor.attachTransform.localPosition = interactorlocalPosition;
		interactor.attachTransform.localRotation = interactorlocalRotation;
	}
	/// <summary>
	/// Clears the stored interactions.
	/// </summary>
	private void ClearInteractor()
	{
		interactorlocalPosition = Vector3.zero;
		interactorlocalRotation = Quaternion.identity;
	}

}
