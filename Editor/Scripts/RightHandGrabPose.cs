using NaughtyAttributes;
using UnityEngine;
using UserController;

public class RightHandGrabPose : HandGrabPose
{
	private void Start()
	{
		leftHand = false;
		InitializePoint();
	}
	protected override string GetName()
	{
		return "RightHand";
	}

#if UNITY_EDITOR
	protected override void GetPrefab()
	{
		if (prefabGrabHandController == null)
			prefabGrabHandController = HandSimulatorPrefabs.Instance.RightHandSimulator;
	}
#endif
}
